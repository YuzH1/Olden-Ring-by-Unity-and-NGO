using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

namespace SG
{
    public class WorldObjectManager : MonoBehaviour
    {
        public static WorldObjectManager instance;

        [Header("Network Objects")]
        [SerializeField] List<NetworkObjectSpawner> networkObjectSpawners;
        [SerializeField] List<GameObject> spawnedInObjects;

        [Header("Fog Doors")]
        public List<FogDoorInteractable> fogDoors;



        // 1.创建一个脚本来管理雾门的逻辑
        // 2.作为网络对象生成雾门（在游戏开始时）
        // 3.创建一个通用对象生成器
        // 4.当雾门生成之后，黄他们加入list中
        // 5.从list中选择合适的雾门，当boss实例化时

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
        public void SpawnNetworkObject(NetworkObjectSpawner networkObjectSpawner)
        {
            if(NetworkManager.Singleton.IsServer)
            {
                networkObjectSpawners.Add(networkObjectSpawner);
                networkObjectSpawner.AttemptToSpawnObject();
            }
        }
    
        public void AddFogDoorToList(FogDoorInteractable fogDoor)
        {
            if(!fogDoors.Contains(fogDoor))
            {
                fogDoors.Add(fogDoor);
            }
            
        }

        public void RemoveFogDoorFromList(FogDoorInteractable fogDoor)
        {
            if(fogDoors.Contains(fogDoor))
            {
                fogDoors.Remove(fogDoor);
            }
        }
    
    }
}
