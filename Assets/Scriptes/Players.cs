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

        [SyncVar] public string matchID;


        [SyncVar] public int playerIndex;

        [SyncVar] public Match currentMatch;


        NetworkMatchChecker networkMatchChecker;

        GameObject playerLobbyUI;

        private void Awake()
        {
            networkMatchChecker = GetComponent<NetworkMatchChecker>();
        }

        public override void OnStartClient()
        {
            if (isLocalPlayer)
            {
                localPlayer = this;
            }
            else
            {
                playerLobbyUI = UILobby.instance.SpawnPlayerUiPrefab(this);
            }
        }

        public override void OnStopClient()
        {
            ClientDisconnect();
        }

        public override void OnStopServer()
        {
            ServerDisconnect();
        }

        public void HostGame(bool publicMatch)
        {
            string matchID = MatchMaker.GetRandomMathID();
            CmdHostGame(matchID, publicMatch);
        }


        //===========HOST MATCH==============
        [Command]
        void CmdHostGame(string _matchID, bool publicMatch)
        {
            matchID = _matchID;
            if (MatchMaker.instance.HostGameBool(_matchID, gameObject, publicMatch, out playerIndex))
            {
                Debug.Log($"<color=green>Game Hosted successfully</color>");
                networkMatchChecker.matchId = _matchID.ToGuid();
                TargetHostGame(true, _matchID, playerIndex);
            }
            else
            {
                Debug.Log($"<color=red>Game Hosted failed</color>");
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
                Debug.Log($"<color=green>Game Joined successfully</color>");
                networkMatchChecker.matchId = _matchID.ToGuid();
                TargetJoinGame(true, _matchID, playerIndex);
            }
            else
            {
                Debug.Log($"<color=red>Game Joined failed</color>");
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


        //===========SEARCH MATCH==============

        public void SearchGame()
        {
            CmdSearchGame();
        }

        [Command]
        void CmdSearchGame()
        {
            if (MatchMaker.instance.SearchGameBool(gameObject, out playerIndex, out matchID))
            {
                Debug.Log($"<color=green>Game Found</color>");
                networkMatchChecker.matchId = matchID.ToGuid();
                TargetHostGame(true, matchID, playerIndex);
            }
            else
            {
                Debug.Log($"<color=red>Game  Not Found</color>");
                TargetSearchGame(false, matchID, playerIndex);
            }
        }

        [TargetRpc]
        void TargetSearchGame(bool success, string _matchID, int _playerIndex)
        {
            playerIndex = _playerIndex;
            matchID = _matchID;
            Debug.Log($"MatchID: {matchID} == {_matchID}");
            UILobby.instance.SearchSuccess(success, _matchID);
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
            
            Debug.Log($"<color=red>Game Beginning</color>");
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


        //===========DISCONNECTED MATCH==============
        public void DisconnectGame()
        {
            CmdDisconnectGame();
        }


        [Command]
        void CmdDisconnectGame()
        {
            ServerDisconnect();
        }

        void ServerDisconnect()
        {
            MatchMaker.instance.DisconnectGame(this, matchID);
            networkMatchChecker.matchId = string.Empty.ToGuid();
            RpcDisconnectGame();
        }


        [ClientRpc]
        void RpcDisconnectGame()
        {
            ClientDisconnect();
        }
        void ClientDisconnect()
        {
            if (playerLobbyUI != null) Destroy(playerLobbyUI);
        }
    }

}
