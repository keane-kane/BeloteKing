using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


namespace Belote2d
{
    public class UILobby : MonoBehaviour
    {

        public static UILobby instance;

        [Header("Host Join")]
        [SerializeField] InputField joinMatchInput;
        [SerializeField] Button joinButton;
        [SerializeField] Button hostButton;
        [SerializeField] Canvas lobbyCanvas;

        [Header("Lobby")]
        [SerializeField] Transform UIPlayerParent;

        [SerializeField] GameObject UIPlayerPrefab;

        [SerializeField] Text matchIDText;
        [SerializeField] GameObject beginGameButton;



        void Start()
        {
            instance = this;
        }
        public void Host()
        {
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;

            Players.localPlayer.HostGame();
        }

        public void HostSuccess(bool success, string _matchID)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
                SpawnPlayerUiPrefab(Players.localPlayer);
                matchIDText.text = _matchID;
                beginGameButton.SetActive(true);
            }
            else
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }

        public void Join()
        {
            joinMatchInput.interactable = false;
            joinButton.interactable = false;
            hostButton.interactable = false;
            Players.localPlayer.JoinGame(joinMatchInput.text);
        }

        public void JoinSuccess(bool success, string _matchID)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;

                SpawnPlayerUiPrefab(Players.localPlayer);
                matchIDText.text = _matchID;
            }
            else
            {
                joinMatchInput.interactable = true;
                joinButton.interactable = true;
                hostButton.interactable = true;
            }
        }


        public void SpawnPlayerUiPrefab(Players player)
        {
            GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
            newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
            newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);
        }


        public void BeginGame()
        {
            Players.localPlayer.BeginGame();
        }
    }

}
