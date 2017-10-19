using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CloseTrigger))]
public class CloseTriggerEditor : Editor
{
    int toolBar;

    public override void OnInspectorGUI()
    {
        CloseTrigger closetrigger = target as CloseTrigger;

        GUIStyle style = new GUIStyle()
        {
            richText = true
        };
        string[] menuOptions = new string[2];
        menuOptions[0] = "Trigger";
        menuOptions[1] = "Gizmo";

        EditorGUILayout.Space();
        toolBar = GUILayout.Toolbar(toolBar, menuOptions);

        switch (toolBar)
        {
            case 0:
                EditorGUIUtility.labelWidth = 70;

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Player Requirements</b>", style);

                EditorGUILayout.BeginHorizontal();
                closetrigger.HasTag = EditorGUILayout.Toggle("Tag", closetrigger.HasTag);
                if (closetrigger.HasTag)
                    closetrigger.Tag = EditorGUILayout.TagField("", closetrigger.Tag);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                closetrigger.HasName = EditorGUILayout.Toggle("Name", closetrigger.HasName);
                if (closetrigger.HasName)
                    closetrigger.Name = EditorGUILayout.TextField("", closetrigger.Name);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                closetrigger.IsLookingAt = EditorGUILayout.Toggle("Looking At", closetrigger.IsLookingAt);
                if (closetrigger.IsLookingAt)
                    closetrigger.Object = (GameObject)EditorGUILayout.ObjectField("", closetrigger.Object, typeof(GameObject), true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                closetrigger.HasPressed = EditorGUILayout.Toggle("Pressed", closetrigger.HasPressed);
                if (closetrigger.HasPressed)
                    closetrigger.Character = EditorGUILayout.TextField("", closetrigger.Character);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                closetrigger.HasScript = EditorGUILayout.Toggle("Script", closetrigger.HasScript);
                if (closetrigger.HasScript)
                    closetrigger.ScriptName = EditorGUILayout.TextField("", closetrigger.ScriptName);
                EditorGUILayout.EndHorizontal();

                if (closetrigger.HasPressed && closetrigger.Character == null || closetrigger.HasPressed && closetrigger.Character == "")
                    EditorGUILayout.HelpBox("The character field has been left empty.", MessageType.Warning);
                if (closetrigger.IsLookingAt && closetrigger.Object == null)
                    EditorGUILayout.HelpBox("The object field has been left empty.", MessageType.Warning);

                EditorGUILayout.Space();
                GUI.color = Color.green;
                if (GUILayout.Button("Add Close Trigger"))
                {
                    DoorPro doorpro = GameObject.Find(closetrigger.transform.parent.transform.parent.name).GetComponent<DoorPro>();

                    GameObject CloseTrigger = new GameObject("Close Trigger");
                    GameObject RotationParent = closetrigger.transform.parent.gameObject;

                    ResetTransform(CloseTrigger, doorpro);
                    SetParentChild(RotationParent, CloseTrigger);
                    CloseTrigger.AddComponent<CloseTrigger>();
                    CloseTrigger.GetComponent<CloseTrigger>().ID = closetrigger.ID;
                }
                EditorGUILayout.Space();
                break;

            case 1:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Trigger Zone</b>", style);
                closetrigger.DrawGizmo = EditorGUILayout.Toggle("Draw Trigger Zone", closetrigger.DrawGizmo);

                if (closetrigger.DrawGizmo)
                {
                    closetrigger.CustomGizmoColor = EditorGUILayout.ColorField("Color", closetrigger.CustomGizmoColor);
                    closetrigger.CustomGizmoColorAlpha = EditorGUILayout.Slider("Opacity", closetrigger.CustomGizmoColorAlpha, 0, 1);
                    closetrigger.CustomGizmoColor.a = closetrigger.CustomGizmoColorAlpha;
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Wireframe</b>", style);
                closetrigger.DrawWire = EditorGUILayout.Toggle("Draw Wireframe", closetrigger.DrawWire);

                if (closetrigger.DrawWire)
                {
                    closetrigger.CustomWireColor = EditorGUILayout.ColorField("Color", closetrigger.CustomWireColor);
                    closetrigger.CustomWireColorAlpha = EditorGUILayout.Slider("Opacity", closetrigger.CustomWireColorAlpha, 0, 1);
                    closetrigger.CustomWireColor.a = closetrigger.CustomWireColorAlpha;
                }
                EditorGUILayout.Space();
                break;
            default: break;
        }

        if (Event.current.type == EventType.Repaint)
        {
            if (closetrigger.gameObject.GetComponent<BoxCollider>() == null) closetrigger.gameObject.AddComponent<BoxCollider>();
            closetrigger.gameObject.GetComponent<BoxCollider>().isTrigger = true;
        }
    }

    public static void ResetTransform(GameObject obj, DoorPro DoorPro)
    {
        obj.transform.position = DoorPro.gameObject.transform.position;
        obj.transform.localScale = Vector3.one;
        obj.transform.rotation = DoorPro.gameObject.transform.rotation;
    }

    public static void SetParentChild(GameObject Parent, GameObject Trigger)
    {
        Parent.transform.parent = Selection.activeGameObject.transform.parent.transform;
        Trigger.transform.parent = Parent.transform;
    }
}
