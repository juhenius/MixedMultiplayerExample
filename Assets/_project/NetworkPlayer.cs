using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MixedMultiplayerExample
{
  public class NetworkPlayer : NetworkBehaviour
  {
    private InputActionMap playerActions;
    private InputAction moveAction;
    private NetworkVariable<Color> color = new();

    public override void OnNetworkSpawn()
    {
      if (IsServer)
      {
        color.Value = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        SetColor(color.Value);

        var offset = Random.insideUnitCircle * 5f;
        transform.position = new Vector3(offset.x, 0f, offset.y);
      }
      else
      {
        color.OnValueChanged += ChangeColor;
        SetColor(color.Value);
      }
    }

    public override void OnNetworkDespawn()
    {
      color.OnValueChanged -= ChangeColor;
    }

    public void SetInput(InputActionAsset actions)
    {
      playerActions = actions.FindActionMap("Player", throwIfNotFound: true);
      moveAction = playerActions.FindAction("Move", throwIfNotFound: true);

      if (gameObject.activeInHierarchy)
      {
        playerActions.Enable();
      }
    }

    private void OnEnable()
    {
      if (playerActions != null)
      {
        playerActions.Enable();
      }
    }

    private void OnDisable()
    {
      if (playerActions != null)
      {
        playerActions.Disable();
      }
    }

    private void Update()
    {
      if (IsOwner)
      {
        var move = moveAction.ReadValue<Vector2>();
        transform.position += new Vector3(move.x, 0f, move.y) * Time.deltaTime;
      }
    }

    private void ChangeColor(Color previous, Color current)
    {
      SetColor(current);
    }

    private void SetColor(Color value)
    {
      foreach (var renderer in GetComponentsInChildren<Renderer>())
      {
        renderer.material.color = value;
      }
    }
  }
}
