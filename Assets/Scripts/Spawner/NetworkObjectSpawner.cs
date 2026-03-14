using UnityEngine;
using Unity.Netcode;

namespace SG
{
    public class NetworkObjectSpawner : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] GameObject networkObject;
        [SerializeField] GameObject instantiatedGameObject;


        private void Start()
        {
            WorldObjectManager.instance.SpawnNetworkObject(this);
            gameObject.SetActive(false);
        }

        public void AttemptToSpawnObject()
        {
            if (networkObject != null)
            {
                instantiatedGameObject = Instantiate(networkObject);
                instantiatedGameObject.transform.position = transform.position;
                instantiatedGameObject.transform.rotation = transform.rotation;
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
