using System;
using Unity.Netcode;
using UnityEngine;

namespace MixedMultiplayerExample
{
  public class ConnectionManager : MonoBehaviour
  {
    public event Action OnGameConnected;
    public event Action OnGameDisconnected;

    private NetworkManager networkManager;

    public void StartClient()
    {
      networkManager.StartClient();
    }

    public void StartHost()
    {
      networkManager.StartHost();
    }

    public void StartServer()
    {
      networkManager.StartServer();
    }

    private void Awake()
    {
      networkManager = FindFirstObjectByType<NetworkManager>();
    }

    private void OnEnable()
    {
      networkManager.OnClientConnectedCallback += ClientConnected;
      networkManager.OnClientDisconnectCallback += ClientDisconnected;
    }

    private void OnDisable()
    {
      networkManager.OnClientConnectedCallback -= ClientConnected;
      networkManager.OnClientDisconnectCallback -= ClientDisconnected;
    }

    private void ClientConnected(ulong id)
    {
      if (IsLocalClient(id))
      {
        OnGameConnected?.Invoke();
      }
    }

    private void ClientDisconnected(ulong id)
    {
      if (IsLocalClient(id))
      {
        OnGameDisconnected?.Invoke();
      }
    }

    private bool IsLocalClient(ulong id)
    {
      return networkManager.LocalClientId == id;
    }
  }
}
