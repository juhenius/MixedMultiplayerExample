using Unity.Netcode;
using UnityEngine;

namespace MixedMultiplayerExample
{
  public class ConnectionStatusUi : MonoBehaviour
  {
    private void OnGUI()
    {
      GUILayout.BeginArea(new Rect(10, 10, 300, 300));

      StatusLabels();

      GUILayout.EndArea();
    }

    private void StatusLabels()
    {
      var networkManager = NetworkManager.Singleton;
      var mode = networkManager.IsHost ? "Host" : networkManager.IsServer ? "Server" : "Client";
      GUILayout.Label("Transport: " + networkManager.NetworkConfig.NetworkTransport.GetType().Name);
      GUILayout.Label("Mode: " + mode);
    }
  }
}
