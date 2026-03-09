using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WorldGameSessionManager : MonoBehaviour
    {
        public static WorldGameSessionManager Instance;

        [Header("Active Players In Session")]
        public List<PlayerManager> players = new List<PlayerManager>(); //当前游戏会话中的玩家列表
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        public void AddPlayerToActivePlayersList(PlayerManager player)
        {
            //检查玩家是否已经在列表中，如果没有，则添加到列表中
            if(!players.Contains(player))
            {
                players.Add(player);
            }

            //检查列表中是否有null项，如果有，移除它们
            for(int i = players.Count - 1; i > -1; i--)
            {
                if(players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }

        public void RemovePlayerFromActivePlayersList(PlayerManager player)
        {
            if(players.Contains(player))
            {
                players.Remove(player);
            }

            //检查列表中是否有null项，如果有，移除它们
            for(int i = players.Count - 1; i > -1; i--)
            {
                if(players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }
    }
    
}
