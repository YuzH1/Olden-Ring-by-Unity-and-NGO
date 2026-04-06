using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class LanLobbyUIBinder : MonoBehaviour
    {
        [Header("Discovery")]
        [SerializeField] private LanDiscoveryListener discoveryListener;

        [Header("UI")]
        [SerializeField] private Transform listRoot;
        [SerializeField] private GameObject listItemPrefab;
        [SerializeField] private TMP_Text statusText;

        [Header("Join")]
        [SerializeField] private float joinTimeoutSeconds = 10f;

        private readonly List<GameObject> spawnedItems = new List<GameObject>();
        private Coroutine joinRoutine;
        private bool isJoining;
        private string pendingHostAddress;
        private ushort pendingHostPort;

        private void OnEnable()
        {
            if (discoveryListener != null)
            {
                discoveryListener.OnHostListChanged += OnHostListChanged;
                RebuildList(discoveryListener.Hosts);
            }

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
            }
        }

        private void OnDisable()
        {
            if (discoveryListener != null)
            {
                discoveryListener.OnHostListChanged -= OnHostListChanged;
            }

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            }

            if (joinRoutine != null)
            {
                StopCoroutine(joinRoutine);
                joinRoutine = null;
            }

            isJoining = false;
        }

        private void OnHostListChanged(IReadOnlyList<LanDiscoveryListener.LanHostInfo> hosts)
        {
            RebuildList(hosts);
        }

        public void TryJoinHost(LanDiscoveryListener.LanHostInfo host)
        {
            if (host == null)
            {
                SetStatus("未选择主机");
                return;
            }

            if (NetworkManager.Singleton == null)
            {
                SetStatus("NetworkManager 不存在");
                return;
            }

            if (isJoining)
            {
                SetStatus("正在连接中，请稍候...");
                return;
            }

            if (joinRoutine != null)
            {
                StopCoroutine(joinRoutine);
            }

            joinRoutine = StartCoroutine(JoinHostRoutine(host));
        }

        private IEnumerator JoinHostRoutine(LanDiscoveryListener.LanHostInfo host)
        {
            NetworkManager networkManager = NetworkManager.Singleton;
            UnityTransport transport = networkManager.GetComponent<UnityTransport>();
            if (transport == null)
            {
                SetStatus("未找到 UnityTransport 组件");
                joinRoutine = null;
                yield break;
            }

            isJoining = true;
            pendingHostAddress = host.HostAddress;
            pendingHostPort = host.GamePort;

            if (networkManager.IsClient || networkManager.IsServer || networkManager.ShutdownInProgress)
            {
                networkManager.Shutdown();

                float shutdownWait = 3f;
                float shutdownTimer = 0f;
                while ((networkManager.IsClient || networkManager.IsServer || networkManager.ShutdownInProgress) && shutdownTimer < shutdownWait)
                {
                    shutdownTimer += Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            transport.SetConnectionData(host.HostAddress, host.GamePort);

            bool started = networkManager.StartClient();
            if (started)
            {
                SetStatus($"正在连接 {host.HostAddress}:{host.GamePort}...");

                float timer = 0f;
                while (timer < joinTimeoutSeconds)
                {
                    if (!isJoining)
                    {
                        joinRoutine = null;
                        yield break;
                    }

                    timer += Time.unscaledDeltaTime;
                    yield return null;
                }

                if (isJoining)
                {
                    SetStatus("连接超时，请检查端口/防火墙/版本配置");
                    Debug.LogWarning($"[LAN UI] Join timeout: {pendingHostAddress}:{pendingHostPort}");
                    networkManager.Shutdown();
                    isJoining = false;
                }
            }
            else
            {
                SetStatus("StartClient 失败");
                Debug.LogWarning($"[LAN UI] StartClient returned false for {host.HostAddress}:{host.GamePort}");
                isJoining = false;
            }

            joinRoutine = null;
        }

        private void OnClientConnected(ulong clientId)
        {
            if (!isJoining || NetworkManager.Singleton == null)
            {
                return;
            }

            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                return;
            }

            isJoining = false;
            SetStatus($"加入成功 {pendingHostAddress}:{pendingHostPort}");
            Debug.Log($"[LAN UI] Connected as client id {clientId}");
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (NetworkManager.Singleton == null)
            {
                return;
            }

            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                return;
            }

            if (!isJoining)
            {
                return;
            }

            isJoining = false;
            string reason = TryGetDisconnectReason(NetworkManager.Singleton);
            if (string.IsNullOrWhiteSpace(reason))
            {
                SetStatus("加入失败：连接被断开");
            }
            else
            {
                SetStatus($"加入失败：{reason}");
            }

            Debug.LogWarning($"[LAN UI] Disconnected while joining {pendingHostAddress}:{pendingHostPort}, reason={reason}");
        }

        private static string TryGetDisconnectReason(NetworkManager networkManager)
        {
            try
            {
                var property = typeof(NetworkManager).GetProperty("DisconnectReason");
                if (property == null)
                {
                    return string.Empty;
                }

                object value = property.GetValue(networkManager, null);
                return value as string ?? string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void RebuildList(IReadOnlyList<LanDiscoveryListener.LanHostInfo> hosts)
        {
            ClearSpawnedItems();

            if (listRoot == null || listItemPrefab == null)
            {
                SetStatus("请先绑定列表根节点和预制体");
                return;
            }

            if (hosts == null || hosts.Count == 0)
            {
                SetStatus("未发现可加入主机");
                return;
            }

            for (int i = 0; i < hosts.Count; i++)
            {
                LanDiscoveryListener.LanHostInfo host = hosts[i];
                GameObject item = Instantiate(listItemPrefab, listRoot);
                spawnedItems.Add(item);

                Button button = item.GetComponent<Button>();
                TMP_Text text = item.GetComponentInChildren<TMP_Text>();

                if (text != null)
                {
                    text.text = $"{host.RoomName}  ({host.CurrentPlayers}/{host.MaxPlayers})\n{host.HostAddress}:{host.GamePort}";
                }

                if (button != null)
                {
                    LanDiscoveryListener.LanHostInfo capture = host;
                    button.onClick.AddListener(() => TryJoinHost(capture));
                }
            }

            SetStatus($"发现 {hosts.Count} 个主机");
        }

        private void ClearSpawnedItems()
        {
            for (int i = 0; i < spawnedItems.Count; i++)
            {
                if (spawnedItems[i] != null)
                {
                    Destroy(spawnedItems[i]);
                }
            }

            spawnedItems.Clear();
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }

            Debug.Log($"[LAN UI] {message}");
        }
    }
}
