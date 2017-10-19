using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DetectionPro))]
public class DetectionProEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DetectionPro DetectionPro = target as DetectionPro;

        GUIStyle style = new GUIStyle()
        {
            richText = true
        };

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("<b>UI Settings</b>", style);
        DetectionPro.LookingAtPrefab = (GameObject)EditorGUILayout.ObjectField("Looking at", DetectionPro.LookingAtPrefab, typeof(GameObject), true);
        DetectionPro.InTriggerZoneLookingAtPrefab = (GameObject)EditorGUILayout.ObjectField("In zone", DetectionPro.InTriggerZoneLookingAtPrefab, typeof(GameObject), true);
        DetectionPro.CrosshairPrefab = (GameObject)EditorGUILayout.ObjectField("Crosshair Prefab", DetectionPro.CrosshairPrefab, typeof(GameObject), true);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("<b>Raycast Settings</b>", style);

        DetectionPro.Reach = EditorGUILayout.FloatField("Reach", DetectionPro.Reach);
        DetectionPro.DebugRay = EditorGUILayout.Toggle("Debug Ray", DetectionPro.DebugRay);
        if (DetectionPro.DebugRay)
        {
            DetectionPro.DebugRayColor = EditorGUILayout.ColorField("Color", DetectionPro.DebugRayColor);
            DetectionPro.DebugRayColorAlpha = EditorGUILayout.Slider("Opacity", DetectionPro.DebugRayColorAlpha, 0, 1);
            DetectionPro.DebugRayColor.a = DetectionPro.DebugRayColorAlpha;
        }
        EditorGUILayout.Space();
    }

    static class Styles
    {
        internal static GUIStyle helpbox;

        static Styles()
        {
            helpbox = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };
        }

        static GUIContent IconContent(string icon, string text)
        {
            GUIContent cached = EditorGUIUtility.IconContent(icon);
            return new GUIContent(text, cached.image);
        }
    }
}
