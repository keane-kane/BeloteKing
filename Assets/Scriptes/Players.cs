using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace Belote2d
{
    public class Players : NetworkBehaviour
    {
        public static Players localPlayer;

        [SyncVar]  public string matchID;


        [SyncVar] public int playerIndex;


        NetworkMatchChecker networkMatchChecker;

      
        private void Start()
        {
            networkMatchChecker = GetComponent<NetworkMatchChecker>();


            if (isLocalPlayer)
            {
                localPlayer = this;
            }
            else
            {
                UILobby.instance.SpawnPlayerUiPrefab(this);
            }

        }

        public void HostGame()
        {
            string matchID = MatchMaker.GetRandomMathID();
            CmdHostGame(matchID);
        }

        //===========HOST MATCH==============
        [Command]
        void CmdHostGame(string _matchID)
        {
            matchID = _matchID;
            if (MatchMaker.instance.HostGameBool(_matchID, gameObject, out playerIndex))
            {
                Debug.Log($"<color= green >Game Hosted successfully</color>");
                networkMatchChecker.matchId = _matchID.ToGuid();
                TargetHostGame(true, _matchID, playerIndex);
            }
            else
            {
                Debug.Log($"<color = red>Game Hosted failed</color>");
                TargetHostGame(false, _matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetHostGame(bool success, string _matchID, int _playerIndex)
        {
            playerIndex = _playerIndex;
            matchID = _matchID;
            Debug.Log($"MatchID: {matchID} == {_matchID}");
            UILobby.instance.HostSuccess(success, _matchID);
        }


        //===========JOIN MATCH==============


        public void JoinGame(string _inputField)
        {
            CmdJoinGame(_inputField);
        }


        [Command]
        void CmdJoinGame(string _matchID)
        {
            matchID = _matchID;
            if (MatchMaker.instance.JoinGameBool(_matchID, gameObject, out playerIndex))
            {
                Debug.Log($"<color= green >Game Joined successfully</color>");
                networkMatchChecker.matchId = _matchID.ToGuid();
                TargetJoinGame(true, _matchID, playerIndex);
            }
            else
            {
                Debug.Log($"<color = red>Game Joined failed</color>");
                TargetJoinGame(false, _matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetJoinGame(bool success, string _matchID, int _playerIndex)
        {
            playerIndex = _playerIndex;
            matchID = _matchID;
            Debug.Log($"MatchID: {matchID} == {_matchID}");
            UILobby.instance.JoinSuccess(success, _matchID);
        }


        //===========BEGIN MATCH==============


        public void BeginGame()
        {
            CmdBeginGame();
        }


        [Command]
        void CmdBeginGame()
        {
            MatchMaker.instance.BeginGame(matchID);   
            
            Debug.Log($"<color = red>Game Beginning</color>");
        }

        public void StarGame()
        {
            TargetBeginGame();

        }

        [TargetRpc]
        void TargetBeginGame()
        {
            Debug.Log($"MatchID: {matchID} | Beginning");
            // additive load game scene
            SceneManager.LoadScene(2, LoadSceneMode.Additive);
        }
    }

}
