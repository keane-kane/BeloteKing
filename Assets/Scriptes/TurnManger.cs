using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace Belote2d
{
    public class TurnManger : NetworkBehaviour
    {
        List<Players> players = new List<Players>();
        public void AddPlayer(Players player)
        {
            players.Add(player);
        }
        
    }

}
