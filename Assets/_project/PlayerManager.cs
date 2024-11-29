using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MixedMultiplayerExample
{
  public class PlayerManager : NetworkBehaviour
  {
    public event Action OnPlayerJoined;
    public event Action OnPlayerLeft;

    public int LocalPlayerCount => localPlayers.Count;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField, PlayerCameraChannel]
    private List<int> cameraChannels = new() { 1, 2, 3, 4 };

    private PlayerInputManager playerInputManager;
    private readonly Dictionary<int, LocalPlayer> localPlayers = new();

    private void Awake()
    {
      playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
      playerInputManager.onPlayerJoined += AddLocalPlayer;
    }

    private void OnDisable()
    {
      playerInputManager.onPlayerJoined -= AddLocalPlayer;
    }

    public void EnableLocalPlayerJoin()
    {
      playerInputManager.EnableJoining();
    }

    public void DisableLocalPlayerJoin()
    {
      playerInputManager.DisableJoining();
    }

    private void AddLocalPlayer(PlayerInput playerInput)
    {
      var localPlayer = playerInput.GetComponent<LocalPlayer>();
      localPlayers.Add(playerInput.playerIndex, localPlayer);
      localPlayer.SetCameraChannel(GetAvailableCameraChannel());
      localPlayer.OnLeaveGameRequested += RemoveLocalPlayer;

      SpawnPlayerRpc(playerInput.playerIndex);
    }

    private void RemoveLocalPlayer(LocalPlayer localPlayer)
    {
      localPlayers.Remove(localPlayer.PlayerIndex);
      ReleaseCameraChannel(localPlayer.CameraChannel);
      Destroy(localPlayer.gameObject);

      if (localPlayer.NetworkPlayer != null)
      {
        var playerNetworkObject = localPlayer.NetworkPlayer.GetComponent<NetworkObject>();
        DespawnPlayerRpc(playerNetworkObject);
      }

      OnPlayerLeft?.Invoke();
    }

    [Rpc(SendTo.Server)]
    private void SpawnPlayerRpc(int playerIndex, RpcParams rpcParams = default)
    {
      GameObject playerInstance = Instantiate(playerPrefab);
      var playerNetworkObject = playerInstance.GetComponent<NetworkObject>();
      playerNetworkObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId);

      ConnectLocalPlayerRpc(
          playerIndex,
          playerNetworkObject,
          RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.Server)]
    private void DespawnPlayerRpc(NetworkObjectReference networkObjectRef, RpcParams rpcParams = default)
    {
      if (networkObjectRef.TryGet(out NetworkObject playerNetworkObject))
      {
        var isPlayer = playerNetworkObject.GetComponent<NetworkPlayer>() != null;
        var isOwner = playerNetworkObject.OwnerClientId == rpcParams.Receive.SenderClientId;
        if (isPlayer && isOwner)
        {
          playerNetworkObject.Despawn();
          Destroy(playerNetworkObject.gameObject);
        }
      }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void ConnectLocalPlayerRpc(int localPlayerIndex, NetworkObjectReference networkObjectRef, RpcParams rpcParams = default)
    {
      var localPlayer = localPlayers[localPlayerIndex];
      if (networkObjectRef.TryGet(out NetworkObject playerNetworkObject))
      {
        var networkPlayer = playerNetworkObject.GetComponent<NetworkPlayer>();
        localPlayer.ConnectPlayer(networkPlayer);

        OnPlayerJoined?.Invoke();
      }
      else
      {
        RemoveLocalPlayer(localPlayer);
      }
    }

    private int GetAvailableCameraChannel()
    {
      var channelIndex = cameraChannels.First();
      cameraChannels.RemoveAt(0);
      return channelIndex;
    }

    private void ReleaseCameraChannel(int cameraChannel)
    {
      cameraChannels.Add(cameraChannel);
    }
  }
}
