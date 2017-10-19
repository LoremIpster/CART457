using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OpenTrigger))]
public class OpenTriggerEditor : Editor
{
    int toolBar;

    public override void OnInspectorGUI()
    {
        OpenTrigger opentrigger = target as OpenTrigger;

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
                opentrigger.HasTag = EditorGUILayout.Toggle("Tag", opentrigger.HasTag);
                if (opentrigger.HasTag)
                    opentrigger.Tag = EditorGUILayout.TagField("", opentrigger.Tag);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                opentrigger.HasName = EditorGUILayout.Toggle("Name", opentrigger.HasName);
                if (opentrigger.HasName)
                    opentrigger.Name = EditorGUILayout.TextField("", opentrigger.Name);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                opentrigger.IsLookingAt = EditorGUILayout.Toggle("Looking At", opentrigger.IsLookingAt);
                if (opentrigger.IsLookingAt)
                    opentrigger.Object = (GameObject)EditorGUILayout.ObjectField("", opentrigger.Object, typeof(GameObject), true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                opentrigger.HasPressed = EditorGUILayout.Toggle("Pressed", opentrigger.HasPressed);
                if (opentrigger.HasPressed)
                    opentrigger.Character = EditorGUILayout.TextField("", opentrigger.Character);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                opentrigger.HasScript = EditorGUILayout.Toggle("Script", opentrigger.HasScript);
                if (opentrigger.HasScript)
                    opentrigger.ScriptName = EditorGUILayout.TextField("", opentrigger.ScriptName);
                EditorGUILayout.EndHorizontal();

                if (opentrigger.HasPressed && opentrigger.Character == null || opentrigger.HasPressed && opentrigger.Character == "")
                    EditorGUILayout.HelpBox("The character field has been left empty.", MessageType.Warning);
                if (opentrigger.IsLookingAt && opentrigger.Object == null)
                    EditorGUILayout.HelpBox("The object field has been left empty.", MessageType.Warning);

                EditorGUILayout.Space();
                GUI.color = Color.green;
                if (GUILayout.Button("Add Open Trigger"))
                {
                    DoorPro doorpro = GameObject.Find(opentrigger.transform.parent.transform.parent.name).GetComponent<DoorPro>();

                    GameObject OpenTrigger = new GameObject("Open Trigger");
                    GameObject RotationParent = opentrigger.transform.parent.gameObject;

                    ResetTransform(OpenTrigger, doorpro);
                    SetParentChild(RotationParent, OpenTrigger);
                    OpenTrigger.AddComponent<OpenTrigger>();
                    OpenTrigger.GetComponent<OpenTrigger>().ID = opentrigger.ID;
                }
                EditorGUILayout.Space();
                break;

            case 1:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Trigger Zone</b>", style);
                opentrigger.DrawGizmo = EditorGUILayout.Toggle("Draw Trigger Zone", opentrigger.DrawGizmo);

                if (opentrigger.DrawGizmo)
                {
                    opentrigger.CustomGizmoColor = EditorGUILayout.ColorField("Color", opentrigger.CustomGizmoColor);
                    opentrigger.CustomGizmoColorAlpha = EditorGUILayout.Slider("Opacity", opentrigger.CustomGizmoColorAlpha, 0, 1);
                    opentrigger.CustomGizmoColor.a = opentrigger.CustomGizmoColorAlpha;
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Wireframe</b>", style);
                opentrigger.DrawWire = EditorGUILayout.Toggle("Draw Wireframe", opentrigger.DrawWire);

                if (opentrigger.DrawWire)
                {
                    opentrigger.CustomWireColor = EditorGUILayout.ColorField("Color", opentrigger.CustomWireColor);
                    opentrigger.CustomWireColorAlpha = EditorGUILayout.Slider("Opacity", opentrigger.CustomWireColorAlpha, 0, 1);
                    opentrigger.CustomWireColor.a = opentrigger.CustomWireColorAlpha;
                }
                EditorGUILayout.Space();
                break;
            default: break;
        }

        if (Event.current.type == EventType.Repaint)
        {
            if (opentrigger.gameObject.GetComponent<BoxCollider>() == null) opentrigger.gameObject.AddComponent<BoxCollider>();
            opentrigger.gameObject.GetComponent<BoxCollider>().isTrigger = true;
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