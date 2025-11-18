using FMODUnity;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR


[CustomEditor(typeof(AudioEventScriptable))]
public class AudioEventInspector : Editor
{
    AudioEventScriptable value;

    private void OnEnable()
    {
        value = (AudioEventScriptable)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        //switch (value.ownerType)
        //{
        //    case SFXType.Player:
        //        value.PlayerSFXType = (PlayerSFX)EditorGUILayout.EnumPopup("Player SFX Type", value.PlayerSFXType);
        //        break;
        //    case SFXType.WarriorGolem:
        //        break;
        //    case SFXType.Main:
        //        break;
        //    case SFXType.Lobby:
        //        break;
        //    case SFXType.Title:
        //    case SFXType.Ambience:
        //    case SFXType.Background:
        //    default:
        //        break;
        //}
        

        EditorGUILayout.Space();
        GUILine(4);
        EditorGUILayout.Space();

        base.OnInspectorGUI();

        EditorGUILayout.EndVertical();
    }

    void GUILine(int lineHeight = 1)
    {
        EditorGUILayout.Space();
        Rect rect = EditorGUILayout.GetControlRect(false, lineHeight);
        rect.height = lineHeight;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        EditorGUILayout.Space();
    }
}
#endif