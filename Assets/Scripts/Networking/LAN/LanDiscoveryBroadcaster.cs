using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace SG
{
    public class LanDiscoveryBroadcaster : MonoBehaviour
    {
        private const string ProtocolId = "SG_LAN_V1";

        [Header("LAN Discovery")]
        [SerializeField] private int discoveryPort = 47777;
        [SerializeField] private ushort gamePort = 7777;
        [SerializeField] private float broadcastInterval = 1f;
        [SerializeField] private bool autoBroadcastWhenHostStarted = true;
        [SerializeField] private bool persistAcrossScenes = true;

        [Header("Room Info")]
        [SerializeField] private string roomName = "Olden Ring Host";
        [SerializeField] private int advertisedMaxPlayers = 4;

        private UdpClient udpClient;
        private Coroutine broadcastRoutine;

        private void Awake()
        {
            if (!persistAcrossScenes)
            {
                return;
            }

            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Debug.LogWarning("[LAN] LanDiscoveryBroadcaster should be on a root GameObject to persist across scenes.");
            }
        }

        [Serializable]
        private class DiscoveryPacket
        {
            public string protocol;
            public string roomName;
            public string hostAddress;
            public ushort gamePort;
            public int currentPlayers;
            public int maxPlayers;
            public string buildId;
            public long timestampUnixMs;
        }

        private void OnEnable()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            }

            if (autoBroadcastWhenHostStarted && NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
            {
                StartBroadcast();
            }
        }

        private void OnDisable()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            }

            StopBroadcast();
        }

        private void Update()
        {
            if (!autoBroadcastWhenHostStarted)
            {
                return;
            }

            if (broadcastRoutine == null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
            {
                StartBroadcast();
            }
        }

        public void StartBroadcast()
        {
            if (broadcastRoutine != null)
            {
                return;
            }

            try
            {
                udpClient = new UdpClient();
                udpClient.EnableBroadcast = true;
                broadcastRoutine = StartCoroutine(BroadcastLoop());
            }
            catch (Exception ex)
            {
                // Debug.LogError($"[LAN] Failed to start broadcaster: {ex.Message}");
                StopBroadcast();
            }
        }

        public void StopBroadcast()
        {
            if (broadcastRoutine != null)
            {
                StopCoroutine(broadcastRoutine);
                broadcastRoutine = null;
            }

            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }
        }

        private void OnServerStarted()
        {
            if (!autoBroadcastWhenHostStarted)
            {
                return;
            }

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
            {
                StartBroadcast();
            }
        }

        private IEnumerator BroadcastLoop()
        {
            var endpoint = new IPEndPoint(IPAddress.Broadcast, discoveryPort);

            while (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
            {
                DiscoveryPacket packet = BuildPacket();
                string json = JsonUtility.ToJson(packet);
                byte[] data = Encoding.UTF8.GetBytes(json);

                try
                {
                    udpClient.Send(data, data.Length, endpoint);
                }
                catch (Exception ex)
                {
                    // Debug.LogWarning($"[LAN] Discovery broadcast failed: {ex.Message}");
                }

                yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, broadcastInterval));
            }

            StopBroadcast();
        }

        private DiscoveryPacket BuildPacket()
        {
            int currentPlayers = 1;
            int maxPlayers = Mathf.Max(1, advertisedMaxPlayers);
            ushort resolvedGamePort = gamePort;

            if (NetworkManager.Singleton != null)
            {
                currentPlayers = NetworkManager.Singleton.ConnectedClientsList != null
                    ? NetworkManager.Singleton.ConnectedClientsList.Count
                    : 1;

                UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                if (transport != null)
                {
                    resolvedGamePort = transport.ConnectionData.Port;
                }
            }

            return new DiscoveryPacket
            {
                protocol = ProtocolId,
                roomName = string.IsNullOrWhiteSpace(roomName) ? "Host" : roomName,
                hostAddress = GetLocalIPv4(),
                gamePort = resolvedGamePort,
                currentPlayers = currentPlayers,
                maxPlayers = maxPlayers,
                buildId = Application.version,
                timestampUnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }

        private static string GetLocalIPv4()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPAddress[] addresses = Dns.GetHostAddresses(hostName);
                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return address.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                // Debug.LogWarning($"[LAN] Failed to resolve local IPv4: {ex.Message}");
            }

            return "127.0.0.1";
        }
    }
}
