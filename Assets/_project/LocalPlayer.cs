using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MixedMultiplayerExample
{
  public class LocalPlayer : MonoBehaviour
  {
    public event Action<LocalPlayer> OnLeaveGameRequested;
    public int PlayerIndex => PlayerInput.playerIndex;
    public PlayerInput PlayerInput => GetComponent<PlayerInput>();

    public NetworkPlayer NetworkPlayer { get; private set; }

    public int CameraChannel { get; private set; }

    public void SetCameraChannel(int cameraChannel)
    {
      CameraChannel = cameraChannel;
      var outputChannel = (OutputChannels)(1 << cameraChannel);
      GetComponentInChildren<CinemachineCamera>().OutputChannel = outputChannel;
      GetComponentInChildren<CinemachineBrain>().ChannelMask = outputChannel;
    }

    public void ConnectPlayer(NetworkPlayer networkPlayer)
    {
      NetworkPlayer = networkPlayer;
      networkPlayer.SetInput(PlayerInput.actions);

      GetComponentInChildren<CinemachineCamera>().Target.TrackingTarget = networkPlayer.transform;
    }

    public void LeaveGame()
    {
      OnLeaveGameRequested?.Invoke(this);
    }

    private void OnEnable()
    {
      PlayerInput.actions.FindAction("LeaveGame", throwIfNotFound: true)
        .performed += HandleLeaveGame;
    }

    private void OnDisable()
    {
      PlayerInput.actions.FindAction("LeaveGame", throwIfNotFound: true)
        .performed -= HandleLeaveGame;
    }

    private void HandleLeaveGame(InputAction.CallbackContext context)
    {
      LeaveGame();
    }
  }
}
