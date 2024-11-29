using System;
using UnityEngine;

namespace MixedMultiplayerExample
{
  public class GameManager : MonoBehaviour
  {
    private enum State
    {
      Created,
      Starting,
      Menu,
      Connecting,
      ServerRunning,
      InGame,
    }

    [SerializeField]
    private ConnectionUi connectionUi;
    [SerializeField]
    private ConnectionStatusUi connectionStatusUi;
    [SerializeField]
    private JoinHelpUi joinHelpUi;
    [SerializeField]
    private Camera menuCamera;

    private ConnectionManager connectionManager;
    private PlayerManager playerManager;
    private State state = State.Created;
    private Action deactivateState;

    private void Awake()
    {
      connectionManager = GetComponent<ConnectionManager>();
      playerManager = GetComponent<PlayerManager>();

      ChangeState(State.Starting, onExit: () =>
      {
        connectionUi.gameObject.SetActive(false);
        connectionStatusUi.gameObject.SetActive(false);
        joinHelpUi.gameObject.SetActive(false);
        menuCamera.gameObject.SetActive(true);
        playerManager.DisableLocalPlayerJoin();
      });
    }

    private void Start()
    {
      ActivateMenuState();
    }

    private void ActivateMenuState()
    {
      ChangeState(State.Menu, () =>
      {
        connectionUi.OnStartClientClicked += StartClient;
        connectionUi.OnStartHostClicked += StartHost;
        connectionUi.OnStartServerClicked += StartServer;
        connectionUi.gameObject.SetActive(true);
      }, () =>
      {
        connectionUi.OnStartClientClicked -= StartClient;
        connectionUi.OnStartHostClicked -= StartHost;
        connectionUi.OnStartServerClicked -= StartServer;
        connectionUi.gameObject.SetActive(false);
      });
    }

    private void StartClient()
    {
      ActivateConnectingState();
      connectionManager.StartClient();
    }

    private void StartHost()
    {
      ActivateConnectingState();
      connectionManager.StartHost();
    }

    private void StartServer()
    {
      ActivateServerState();
      connectionManager.StartServer();
    }

    private void ActivateConnectingState()
    {
      ChangeState(State.Connecting, () =>
      {
        connectionManager.OnGameConnected += ActivateInGameState;
      }, () =>
      {
        connectionManager.OnGameConnected -= ActivateInGameState;
      });
    }

    private void ActivateInGameState()
    {
      ChangeState(State.InGame, () =>
      {
        connectionStatusUi.gameObject.SetActive(true);
        SetJoinHelpVisibility();

        playerManager.OnPlayerJoined += SetJoinHelpVisibility;
        playerManager.OnPlayerLeft += SetJoinHelpVisibility;
        playerManager.EnableLocalPlayerJoin();
      }, () =>
      {
        connectionStatusUi.gameObject.SetActive(false);

        playerManager.OnPlayerJoined -= SetJoinHelpVisibility;
        playerManager.OnPlayerLeft -= SetJoinHelpVisibility;
        playerManager.DisableLocalPlayerJoin();
      });
    }

    private void SetJoinHelpVisibility()
    {
      var visible = playerManager.LocalPlayerCount == 0;
      menuCamera.gameObject.SetActive(visible);
      joinHelpUi.gameObject.SetActive(visible);
    }

    private void ActivateServerState()
    {
      ChangeState(State.InGame, () =>
      {
        connectionStatusUi.gameObject.SetActive(true);
      }, () =>
      {
        connectionStatusUi.gameObject.SetActive(false);
      });
    }

    private void ChangeState(State state, Action onEnter = default, Action onExit = default)
    {
      if (this.state == state)
      {
        Debug.LogError($"Changed back to same state: {state}");
        return;
      }

      deactivateState?.Invoke();

      this.state = state;
      deactivateState = onExit;

      onEnter?.Invoke();
    }
  }
}
