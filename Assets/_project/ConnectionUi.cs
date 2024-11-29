using System;
using UnityEngine;

namespace MixedMultiplayerExample
{
  public class ConnectionUi : MonoBehaviour
  {
    public event Action OnStartHostClicked;
    public event Action OnStartClientClicked;
    public event Action OnStartServerClicked;

    private void OnGUI()
    {
      GUILayout.BeginArea(new Rect(10, 10, 300, 300));

      StartButtons();

      GUILayout.EndArea();
    }

    private void StartButtons()
    {
      if (GUILayout.Button("Host"))
      {
        OnStartHostClicked?.Invoke();
      }
      if (GUILayout.Button("Client"))
      {
        OnStartClientClicked?.Invoke();
      }
      if (GUILayout.Button("Server"))
      {
        OnStartServerClicked?.Invoke();
      }
    }
  }
}
