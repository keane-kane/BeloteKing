using System.Security.Cryptography;
using UnityEngine;
using System.Text;
using System;
using Mirror;

namespace Belote2d
{

    [System.Serializable]
    public class Match
    {
        public string matchID;

        public bool publicMatch;
        public bool inMatch;
        public bool matchFull;

        public SyncListGameObject players = new SyncListGameObject();

        public Match(string matchID, GameObject player)
        {
            this.matchID = matchID;
            players.Add(player);
        }

        public Match()
        {

        }
    }

    [System.Serializable]
    public class SyncListGameObject : SyncList<GameObject>
    {

    }
    
    [System.Serializable]
    public class SyncListMatch : SyncList<Match>
    {

    }

    public class MatchMaker : NetworkBehaviour
    {

        public static MatchMaker instance;


        public SyncListMatch matches = new SyncListMatch();

        public SyncList<string> matchIDs = new SyncList<string>();

        [SerializeField] GameObject turnMangerPrefab;

        void Start()
        {
            instance = this;
        }
        public bool HostGameBool(string _matchID, GameObject _player,bool publicMatch, out int playerIndex)
        {
            playerIndex = -1;
            if (!matchIDs.Contains(_matchID))
            {
                matchIDs.Add(_matchID);
                //matches.Add(new Match(_matchID, _player));
                Match match = new Match(_matchID, _player);
                match.publicMatch = publicMatch;
                matches.Add(match);
                Debug.Log($"Match generated");
                _player.GetComponent<Players>().currentMatch = match;
                playerIndex = 1;
                return true;

            }
            else
            {
                Debug.Log($"Match ID already exists");
                return false;
            }

        }

        public bool JoinGameBool(string _matchID, GameObject _player, out int playerIndex)
        {
            playerIndex = -1;
            if (matchIDs.Contains(_matchID))
            {
                for(int i = 0; i < matches.Count; i++)
                {
                    if (matches[i].matchID == _matchID)
                    {
                        matches[i].players.Add(_player);
                        _player.GetComponent<Players>().currentMatch = matches[i];
                        playerIndex = matches[i].players.Count;
                        break;
                    }
                }
                Debug.Log($"Match Joined");
                return true;

            }
            else
            {
                Debug.Log($"Match ID does not exist");
                return false;
            }
        }

        public bool SearchGameBool(GameObject player,out int playerIndex, out string matchID)
        {
            playerIndex = -1;
            matchID = string.Empty;

            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].publicMatch && !matches[i].matchFull && !matches[i].inMatch)
                {
                    matchID = matches[i].matchID;
                    if(JoinGameBool(matchID, player, out playerIndex))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public void BeginGame(string _matchID)
        {
            GameObject newTurnManager = Instantiate(turnMangerPrefab);
            NetworkServer.Spawn(newTurnManager);

            newTurnManager.GetComponent<NetworkMatchChecker>().matchId = _matchID.ToGuid();
            TurnManger turnManger = newTurnManager.GetComponent<TurnManger>();

            for (int i = 0; i < matches.Count; i++)
            {
                if (matches[i].matchID == _matchID)
                {
                    foreach (var player in matches[i].players)
                    {
                        Players _player = player.GetComponent<Players>();
                        turnManger.AddPlayer(_player);
                        _player.StarGame();
                    }
                    break;
                }
            }
        }


        public void DisconnectGame(Players player, string _matchID)
        {
            for(int i = 0; i < matches.Count; i++)
            {
                if(matches[i].matchID == _matchID)
                {
                    int playerIndex = matches[i].players.IndexOf(player.gameObject);
                    matches[i].players.RemoveAt(playerIndex);
                    Debug.Log($"Player disconnected from the match {_matchID} | {matches[i].players.Count} player rmaining");

                }
                if(matches[i].players.Count == 0)
                {
                    Debug.Log($"No more players in  the Match. | Terminating {_matchID} ");
                    matches.RemoveAt(i);
                    matchIDs.Remove(_matchID);

                }
                break;
            }
        }

        public static string GetRandomMathID()
        {
            string _id = string.Empty;
            for(int i = 0; i < 5; i++)
            {
                int random = UnityEngine.Random.Range(0, 36) ;
                if(random < 26)
                {
                    _id += (char)(random + 65);
                }
                else
                {
                    _id += (random - 26).ToString();
                }
            }
            Debug.Log($"Random Match ID: {_id}");
            return _id;
        }


    }

    public static class MatchExtensions
    {
        public static Guid ToGuid(this string id)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBytes = Encoding.Default.GetBytes(id);
            byte[] hashBytes = provider.ComputeHash(inputBytes);

            return new Guid(hashBytes);
        }

    }
       

}
