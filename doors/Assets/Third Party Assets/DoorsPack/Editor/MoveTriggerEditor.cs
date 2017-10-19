using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveTrigger))]
public class MoveTriggerEditor : Editor
{
    int toolBar;

    public override void OnInspectorGUI()
    {
        MoveTrigger movetrigger = target as MoveTrigger;

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
                movetrigger.HasTag = EditorGUILayout.Toggle("Tag", movetrigger.HasTag);
                if (movetrigger.HasTag)
                    movetrigger.Tag = EditorGUILayout.TagField("", movetrigger.Tag);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                movetrigger.HasName = EditorGUILayout.Toggle("Name", movetrigger.HasName);
                if (movetrigger.HasName)
                    movetrigger.Name = EditorGUILayout.TextField("", movetrigger.Name);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                movetrigger.IsLookingAt = EditorGUILayout.Toggle("Looking At", movetrigger.IsLookingAt);
                if (movetrigger.IsLookingAt)
                    movetrigger.Object = (GameObject)EditorGUILayout.ObjectField("", movetrigger.Object, typeof(GameObject), true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                movetrigger.HasPressed = EditorGUILayout.Toggle("Pressed", movetrigger.HasPressed);
                if (movetrigger.HasPressed)
                    movetrigger.Character = EditorGUILayout.TextField("", movetrigger.Character);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                movetrigger.HasScript = EditorGUILayout.Toggle("Script", movetrigger.HasScript);
                if (movetrigger.HasScript)
                    movetrigger.ScriptName = EditorGUILayout.TextField("", movetrigger.ScriptName);
                EditorGUILayout.EndHorizontal();

                if (movetrigger.HasPressed && movetrigger.Character == null || movetrigger.HasPressed && movetrigger.Character == "")
                    EditorGUILayout.HelpBox("The character field has been left empty.", MessageType.Warning);
                if (movetrigger.IsLookingAt && movetrigger.Object == null)
                    EditorGUILayout.HelpBox("The object field has been left empty.", MessageType.Warning);

                EditorGUILayout.Space();
                GUI.color = Color.green;
                if (GUILayout.Button("Add Move Trigger"))
                {
                    DoorPro doorpro = GameObject.Find(movetrigger.transform.parent.transform.parent.name).GetComponent<DoorPro>();

                    GameObject MoveTrigger = new GameObject("Move Trigger");
                    GameObject RotationParent = movetrigger.transform.parent.gameObject;

                    ResetTransform(MoveTrigger, doorpro);
                    SetParentChild(RotationParent, MoveTrigger);
                    MoveTrigger.AddComponent<MoveTrigger>();
                    MoveTrigger.GetComponent<MoveTrigger>().ID = movetrigger.ID;
                }
                EditorGUILayout.Space();
                break;

            case 1:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Trigger Zone</b>", style);
                movetrigger.DrawGizmo = EditorGUILayout.Toggle("Draw Trigger Zone", movetrigger.DrawGizmo);

                if (movetrigger.DrawGizmo)
                {
                    movetrigger.CustomGizmoColor = EditorGUILayout.ColorField("Color", movetrigger.CustomGizmoColor);
                    movetrigger.CustomGizmoColorAlpha = EditorGUILayout.Slider("Opacity", movetrigger.CustomGizmoColorAlpha, 0, 1);
                    movetrigger.CustomGizmoColor.a = movetrigger.CustomGizmoColorAlpha;
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Wireframe</b>", style);
                movetrigger.DrawWire = EditorGUILayout.Toggle("Draw Wireframe", movetrigger.DrawWire);

                if (movetrigger.DrawWire)
                {
                    movetrigger.CustomWireColor = EditorGUILayout.ColorField("Color", movetrigger.CustomWireColor);
                    movetrigger.CustomWireColorAlpha = EditorGUILayout.Slider("Opacity", movetrigger.CustomWireColorAlpha, 0, 1);
                    movetrigger.CustomWireColor.a = movetrigger.CustomWireColorAlpha;
                }
                EditorGUILayout.Space();
                break;
            default: break;
        }

        if (Event.current.type == EventType.Repaint)
        {
            if (movetrigger.gameObject.GetComponent<BoxCollider>() == null) movetrigger.gameObject.AddComponent<BoxCollider>();
            movetrigger.gameObject.GetComponent<BoxCollider>().isTrigger = true;
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
