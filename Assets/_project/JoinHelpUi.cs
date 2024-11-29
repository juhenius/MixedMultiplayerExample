using UnityEngine;

namespace MixedMultiplayerExample
{
  public class JoinHelpUi : MonoBehaviour
  {
    private GUIStyle centeredStyle;

    private void OnGUI()
    {
      float width = 400;
      float height = 150;
      float x = (Screen.width - width) / 2;
      float y = (Screen.height - height) / 2;

      GUILayout.BeginArea(new Rect(x, y, width, height));

      centeredStyle ??= new(GUI.skin.label);
      centeredStyle.alignment = TextAnchor.MiddleCenter;
      centeredStyle.fontSize = 20;
      centeredStyle.normal.textColor = Color.white;
      centeredStyle.wordWrap = true;

      GUILayout.Label("Press any key on the keyboard", centeredStyle);
      GUILayout.Label("or the start button on a controller", centeredStyle);
      GUILayout.Label("to join.", centeredStyle);

      GUILayout.EndArea();
    }
  }
}
