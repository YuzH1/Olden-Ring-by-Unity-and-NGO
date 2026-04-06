using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace SG
{
    public class LanDiscoveryListener : MonoBehaviour
    {
        private const string ProtocolId = "SG_LAN_V1";

        [Header("LAN Discovery")]
        [SerializeField] private int discoveryPort = 47777;
        [SerializeField] private float staleTimeoutSeconds = 8f;
        [SerializeField] private bool ignoreBuildIdMismatch = false;

        public event Action<IReadOnlyList<LanHostInfo>> OnHostListChanged;

        private UdpClient udpClient;
        private readonly Dictionary<string, LanHostInfo> hostByKey = new Dictionary<string, LanHostInfo>();
        private readonly List<string> staleKeys = new List<string>();
        private readonly List<LanHostInfo> hostCache = new List<LanHostInfo>();

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

        [Serializable]
        public class LanHostInfo
        {
            public string HostAddress;
            public ushort GamePort;
            public string RoomName;
            public int CurrentPlayers;
            public int MaxPlayers;
            public string BuildId;
            public float LastSeenRealtime;

            public string Key => $"{HostAddress}:{GamePort}";
        }

        public IReadOnlyList<LanHostInfo> Hosts => hostCache;

        private void OnEnable()
        {
            StartListening();
        }

        private void OnDisable()
        {
            StopListening();
        }

        private void Update()
        {
            if (udpClient == null)
            {
                return;
            }

            bool changed = PollPackets();
            changed |= RemoveStaleHosts();

            if (changed)
            {
                RebuildCacheAndNotify();
            }
        }

        private void StartListening()
        {
            if (udpClient != null)
            {
                return;
            }

            try
            {
                udpClient = new UdpClient(discoveryPort);
                udpClient.EnableBroadcast = true;
                udpClient.Client.Blocking = false;
            }
            catch (Exception ex)
            {
                // Debug.LogError($"[LAN] Failed to start listener on {discoveryPort}: {ex.Message}");
                StopListening();
            }
        }

        private void StopListening()
        {
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }

            if (hostByKey.Count > 0)
            {
                hostByKey.Clear();
                RebuildCacheAndNotify();
            }
        }

        private bool PollPackets()
        {
            bool changed = false;
            while (udpClient != null && udpClient.Available > 0)
            {
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                byte[] bytes;

                try
                {
                    bytes = udpClient.Receive(ref sender);
                }
                catch (Exception)
                {
                    break;
                }

                string json = Encoding.UTF8.GetString(bytes);
                DiscoveryPacket packet;
                try
                {
                    packet = JsonUtility.FromJson<DiscoveryPacket>(json);
                }
                catch (Exception)
                {
                    continue;
                }

                if (packet == null || packet.protocol != ProtocolId)
                {
                    continue;
                }

                if (!ignoreBuildIdMismatch && !string.Equals(packet.buildId, Application.version, StringComparison.Ordinal))
                {
                    continue;
                }

                // Prefer sender endpoint to avoid wrong advertised NIC/VPN addresses.
                string hostAddress = sender.Address.ToString();
                if (string.IsNullOrWhiteSpace(hostAddress))
                {
                    hostAddress = packet.hostAddress;
                }

                string key = $"{hostAddress}:{packet.gamePort}";
                LanHostInfo info = new LanHostInfo
                {
                    HostAddress = hostAddress,
                    GamePort = packet.gamePort,
                    RoomName = string.IsNullOrWhiteSpace(packet.roomName) ? "Host" : packet.roomName,
                    CurrentPlayers = Mathf.Max(0, packet.currentPlayers),
                    MaxPlayers = Mathf.Max(1, packet.maxPlayers),
                    BuildId = packet.buildId,
                    LastSeenRealtime = Time.realtimeSinceStartup
                };

                if (!hostByKey.TryGetValue(key, out LanHostInfo oldInfo) || HasDifferentData(oldInfo, info))
                {
                    hostByKey[key] = info;
                    changed = true;
                }
                else
                {
                    oldInfo.LastSeenRealtime = info.LastSeenRealtime;
                }
            }

            return changed;
        }

        private bool RemoveStaleHosts()
        {
            if (hostByKey.Count == 0)
            {
                return false;
            }

            float now = Time.realtimeSinceStartup;
            staleKeys.Clear();
            foreach (KeyValuePair<string, LanHostInfo> pair in hostByKey)
            {
                if (now - pair.Value.LastSeenRealtime > staleTimeoutSeconds)
                {
                    staleKeys.Add(pair.Key);
                }
            }

            if (staleKeys.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < staleKeys.Count; i++)
            {
                hostByKey.Remove(staleKeys[i]);
            }

            return true;
        }

        private void RebuildCacheAndNotify()
        {
            hostCache.Clear();
            foreach (KeyValuePair<string, LanHostInfo> pair in hostByKey)
            {
                hostCache.Add(pair.Value);
            }

            hostCache.Sort((a, b) => string.Compare(a.RoomName, b.RoomName, StringComparison.Ordinal));
            OnHostListChanged?.Invoke(hostCache);
        }

        private static bool HasDifferentData(LanHostInfo left, LanHostInfo right)
        {
            return !string.Equals(left.RoomName, right.RoomName, StringComparison.Ordinal)
                   || left.CurrentPlayers != right.CurrentPlayers
                   || left.MaxPlayers != right.MaxPlayers
                   || !string.Equals(left.BuildId, right.BuildId, StringComparison.Ordinal);
        }
    }
}
