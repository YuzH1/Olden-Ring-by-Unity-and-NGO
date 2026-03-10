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

        [Header("DEBUG")]
        [SerializeField] bool respawnCharacters = false;
        [SerializeField] bool despawnCharacters = false;

        [Header("Characters")]
        [SerializeField] GameObject[] aiCharacters;
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

        private void Start()
        {
            if(NetworkManager.Singleton.IsServer)
            {
                //生成所有AI在场景里
                StartCoroutine(WaitForSceneToLoadThenSpawnCharacters());
            }
        }

        private void Update()
        {
            if (respawnCharacters)
            {
                respawnCharacters = false;
                SpawnAllCharacters();
            }

            if (despawnCharacters)
            {
                despawnCharacters = false;
                DespawnAllCharacters();
            }
        }

        private IEnumerator WaitForSceneToLoadThenSpawnCharacters()
        {
            while(!SceneManager.GetActiveScene().isLoaded)
            {
                yield return null;
            }

            SpawnAllCharacters();
        }

        private void SpawnAllCharacters()
        {
            //在场景里生成所有AI
            foreach (GameObject character in aiCharacters)
            {
                GameObject instantiatedCharacter = Instantiate(character);
                instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
                spawnedCharacters.Add(instantiatedCharacter);
            }

        }

        private void DespawnAllCharacters()
        {
            foreach (GameObject character in spawnedCharacters)
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
