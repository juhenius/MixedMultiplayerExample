using UnityEditor;
using UnityEngine;
using Unity.Cinemachine.Editor;
using System.Text.RegularExpressions;

namespace MixedMultiplayerExample
{
  [CustomPropertyDrawer(typeof(PlayerCameraChannelAttribute))]
  public class PlayerCameraChannelDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      var labelText = ResolveLabel(label.text);

      if (property.propertyType != SerializedPropertyType.Integer)
      {
        EditorGUI.LabelField(position, labelText, "Use with integer.");
        return;
      }

      var settings = CinemachineChannelNames.InstanceIfExists;
      if (settings == null)
      {
        EditorGUI.LabelField(position, labelText, "Make sure CinemachineChannelNames exists.");
        return;
      }

      var currentValue = property.intValue;
      property.intValue = EditorGUI.Popup(position, labelText, currentValue, settings.ChannelNames);
    }

    private string ResolveLabel(string text)
    {
      return Regex.Replace(
          text,
          @"Element (\d+)",
          (match) => $"Player {int.Parse(match.Groups[1].Value) + 1}");
    }
  }
}
