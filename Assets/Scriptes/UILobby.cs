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
        [SerializeField] List<Selectable> lobbySelectalbes = new List<Selectable>();
        /*[SerializeField] Button joinButton;
        [SerializeField] Button hostButton;*/
        [SerializeField] Canvas lobbyCanvas;

        [SerializeField] Canvas searchCanvas;

        [Header("Lobby")]
        [SerializeField] Transform UIPlayerParent;

        [SerializeField] GameObject UIPlayerPrefab;

        [SerializeField] Text matchIDText;

        [SerializeField] GameObject beginGameButton;


        GameObject playerLobbyUI;
        bool searching = false;

        void Start()
        {
            instance = this;
        }
        public void HostPrivate()
        {
            joinMatchInput.interactable = false;
            lobbySelectalbes.ForEach(x => x.interactable = false);

            Players.localPlayer.HostGame(false);
        } 
        public void HostPublic()
        {
            joinMatchInput.interactable = false;
            lobbySelectalbes.ForEach(x => x.interactable = false);

            Players.localPlayer.HostGame(true);
        }

        public void HostSuccess(bool success, string _matchID)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
                playerLobbyUI = SpawnPlayerUiPrefab(Players.localPlayer);
                matchIDText.text = _matchID;
                beginGameButton.SetActive(true);
            }
            else
            {
                joinMatchInput.interactable = true;
                lobbySelectalbes.ForEach(x => x.interactable = true);
            }
        }

        public void Join()
        {
            joinMatchInput.interactable = false;
            lobbySelectalbes.ForEach(x => x.interactable = false);
            Players.localPlayer.JoinGame(joinMatchInput.text);
        }

        public void JoinSuccess(bool success, string _matchID)
        {
            if (success)
            {
                lobbyCanvas.enabled = true;
                beginGameButton.SetActive(false);

                playerLobbyUI = SpawnPlayerUiPrefab(Players.localPlayer);
                matchIDText.text = _matchID;
            }
            else
            {
                joinMatchInput.interactable = true;
                lobbySelectalbes.ForEach(x => x.interactable = true);
            }
        }


        public GameObject SpawnPlayerUiPrefab(Players player)
        {
            GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParent);
            newUIPlayer.GetComponent<UIPlayer>().SetPlayer(player);
            newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);

            return newUIPlayer;
        }


        public void BeginGame()
        {
            Players.localPlayer.BeginGame();
        }

        public void SearchGame()
        {
            Debug.Log($"Searching match");
            searchCanvas.enabled = true;
            StartCoroutine(SearchingForGame());
        }


        IEnumerator SearchingForGame()
        {
            searching = true;
           /* WaitForSeconds checkEveryFewSeconds = new WaitForSeconds(1);
            while (searching)
            {
                yield return checkEveryFewSeconds;
                if(searching)
                Players.localPlayer.SearchGame();
            }*/

            float curentTime = -1;
            while (searching)
            {
                if (curentTime > 0)
                {
                    curentTime -= Time.deltaTime;
                }
                else
                {
                    curentTime = -1;
                    Players.localPlayer.SearchGame();

                }
            }
            yield return null;
        }

        public void SearchSuccess(bool success, string _matchID)
        {
            if (success)
            {
                searchCanvas.enabled = false;
                JoinSuccess(success, _matchID);
                searching = false;

            }
        }


        public void SearchCancel()
        {
            searching = false;
            lobbySelectalbes.ForEach(x => x.interactable = true);
        }


        public void DisconncectLobby()
        {
            if (playerLobbyUI != null) Destroy(playerLobbyUI);
            Players.localPlayer.DisconnectGame();

            lobbyCanvas.enabled = false;
            lobbySelectalbes.ForEach(x => x.enabled = true);
            beginGameButton.SetActive(false);
        }
    }

}
