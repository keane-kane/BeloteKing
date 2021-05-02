using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


namespace Belote2d
{ 
    public class UIPlayer : MonoBehaviour
    {

        [SerializeField] Text text;

        private Players player;

        public void SetPlayer (Players player)
        {
            this.player = player;
            text.text = "Player " + player.playerIndex.ToString();
        }
    }

}
