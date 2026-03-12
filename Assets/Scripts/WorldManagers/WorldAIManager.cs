using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace SG
{
    public class WorldAIManager : MonoBehaviour
    {
        public static WorldAIManager instance;



        

        [Header("Characters")]
        [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
        [SerializeField] List<GameObject> spawnedCharacters;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

 

        public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
        {
            if(NetworkManager.Singleton.IsServer)
            {
                aiCharacterSpawners.Add(aiCharacterSpawner);
                aiCharacterSpawner.AttemptToSpawnCharacter();
            }
        }

        private void DespawnAllCharacters()
        {
            foreach (var character in spawnedCharacters)
            {
                character.GetComponent<NetworkObject>().Despawn();
                
            }
        }

        private void DisableAllCharacters()
        {
            //对象池实现时可以改成Disable而不是Despawn
            //用来暂时禁用对象，同步在网络上
            //可以用来隐藏一些距离玩家较远的敌人
            //角色可以被划分在不同的区域中，当玩家进入某个区域时启用该区域的角色，离开时禁用
            foreach (GameObject character in spawnedCharacters)
            {
                character.SetActive(false);
            }
        }



    }
}
