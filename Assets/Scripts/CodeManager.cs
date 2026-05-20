using Unity.Netcode;
using UnityEngine;

namespace Code
{
    public class CodeManager : MonoBehaviour
    {
        private NetworkManager m_NetworkManager;
        private const int MaxPlayers = 6;

        private void Awake()
        {
            m_NetworkManager = GetComponent<NetworkManager>();
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!m_NetworkManager.IsClient && !m_NetworkManager.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
                SubmitNewPosition();
            }

            GUILayout.EndArea();
        }

        private void StartButtons()
        {
            if (GUILayout.Button("Host")) m_NetworkManager.StartHost();
            if (GUILayout.Button("Client")) m_NetworkManager.StartClient();
            if (GUILayout.Button("Server")) m_NetworkManager.StartServer();
        }

        private void StatusLabels()
        {
            var mode = m_NetworkManager.IsHost ?
                "Host" : m_NetworkManager.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " + m_NetworkManager.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
            GUILayout.Label($"Jugadores: {m_NetworkManager.ConnectedClientsIds.Count}/{MaxPlayers}");
        }

        private void SubmitNewPosition()
        {
            if (GUILayout.Button(m_NetworkManager.IsServer ? "Move" : "Request Position Change"))
            {
                if (m_NetworkManager.IsServer && !m_NetworkManager.IsClient)
                {
                    foreach (ulong uid in m_NetworkManager.ConnectedClientsIds)
                        m_NetworkManager.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<CodePlayer>().Move();
                }
                else
                {
                    var playerObject = m_NetworkManager.SpawnManager.GetLocalPlayerObject();
                    var player = playerObject.GetComponent<CodePlayer>();
                    player.Move();
                }
            }
        }
    }
}