using UnityEngine;
using UnityEditor;
using Malee.Editor;

[CustomEditor(typeof(DoorPro)), CanEditMultipleObjects]
public class DoorProEditor : Editor
{
    private ReorderableList RotationTimeline;

    int NumberOfTriggers, NumberOfBlocks, toolBar;

    public GameObject MoveTrigger, OpenTrigger, CloseTrigger, DoorParent, RotationParent;

    void OnEnable()
    {
        DoorPro DoorPro = target as DoorPro;

        RotationTimeline = new ReorderableList(serializedObject.FindProperty("RotationTimeline"), true, true, true, ReorderableList.ElementDisplayType.Expandable, "RotationType", null);

        if (DoorPro.transform.parent == null)
        {
            //Create a parent with the same name as the door itself and reset it
            DoorParent = new GameObject(DoorPro.gameObject.name);
            DoorParent.transform.localRotation = Quaternion.identity;
            DoorParent.transform.localPosition = Vector3.zero;
            DoorParent.transform.localScale = Vector3.one;
            DoorPro.transform.parent = DoorParent.transform;
        }

        //Loop through all the children of the parent object and check for triggers
        NumberOfTriggers = 0;
        for (int x = 0; x < DoorPro.transform.parent.childCount; x++)
        {
            if (DoorPro.transform.parent.transform.Find("Rotation " + x + " (Single)") != null || DoorPro.transform.parent.transform.Find("Rotation " + x + " (Looped)") != null) NumberOfTriggers += 1;
        }
    }

    public override void OnInspectorGUI()
    {
        DoorPro DoorPro = target as DoorPro;

        string[] menuOptions = new string[2];
        menuOptions[0] = "Door";
        menuOptions[1] = "Rotations";

        EditorGUILayout.Space();
        toolBar = GUILayout.Toolbar(toolBar, menuOptions);

        GUIStyle style = new GUIStyle()
        {
            richText = true
        };

        switch (toolBar)
        {
            //Door and hinge settings
            case 0:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Door Settings</b>", style);
                DoorPro.DoorScale = (DoorPro.ScaleOfDoor)EditorGUILayout.EnumPopup("Scale", DoorPro.DoorScale);
                DoorPro.PivotPosition = (DoorPro.PositionOfPivot)EditorGUILayout.EnumPopup("Pivot Position", DoorPro.PivotPosition);
                EditorGUILayout.Space();
                if (DoorPro.DoorScale == DoorPro.ScaleOfDoor.Unity3DUnits && DoorPro.PivotPosition == DoorPro.PositionOfPivot.Centered)
                {
                    EditorGUILayout.LabelField("<b>Hinge Settings</b>", style);
                    DoorPro.HingePosition = (DoorPro.PositionOfHinge)EditorGUILayout.EnumPopup("Position", DoorPro.HingePosition);
                    EditorGUILayout.Space();
                }

                if (DoorPro.DoorScale == DoorPro.ScaleOfDoor.Other && DoorPro.PivotPosition == DoorPro.PositionOfPivot.Centered)
                    EditorGUILayout.HelpBox("If your door is not scaled in Unity3D units and the pivot position is not already positioned correctly, the hinge algorithm will not work as expected.", MessageType.Error);

                else if (Tools.pivotMode == PivotMode.Center)
                    EditorGUILayout.HelpBox("Make sure the tool handle is placed at the active object's pivot point.", MessageType.Warning);

                EditorGUILayout.Space();
                break;

            //Rotation settings
            case 1:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("<b>Angle Settings</b>", style);
                DoorPro.RotationWay = (DoorPro.WayOfRotation)EditorGUILayout.EnumPopup("Rotation", DoorPro.RotationWay);
                DoorPro.AngleConvention = (DoorPro.ConventionAngle)EditorGUILayout.EnumPopup("Convention", DoorPro.AngleConvention);
                EditorGUILayout.Space();

                RotationTimeline.DoLayoutList();
                serializedObject.ApplyModifiedProperties();

                //Loop through the rotation timeline to check for errors
                if (DoorPro.RotationTimeline != null)
                {
                    for (int x = 0; x < DoorPro.RotationTimeline.Count; x++)
                    {
                        if ((DoorPro.RotationTimeline[x].FinalAngle - DoorPro.RotationTimeline[x].InitialAngle) >= 360)
                            EditorGUILayout.HelpBox("The difference between FinalAngle and InitialAngle should not exceed 360°. (See rotation " + (x + 1) + ")", MessageType.Warning);

                        if (DoorPro.RotationTimeline[x].Speed <= 0)
                            EditorGUILayout.HelpBox("Speed is zero/negative at rotation block " + (x + 1) + ".", MessageType.Warning);
                    }
                }
                break;
            default: break;
        }

        NumberOfBlocks = 0;
        if (DoorPro.RotationTimeline != null)
            NumberOfBlocks = DoorPro.RotationTimeline.Count;

        serializedObject.Update();

        if (!Application.isPlaying)
        {
            if (NumberOfTriggers < NumberOfBlocks)
            {
                for (int index = 0; index < NumberOfBlocks; index++)
                {
                    if (DoorPro.transform.parent.transform.Find("Rotation " + (index + 1) + " (Single)") == null && DoorPro.RotationTimeline[index].RotationType == DoorPro.RotationTimelineData.TypeOfRotation.SingleRotation)
                    {
                        MoveTrigger = new GameObject("Move Trigger");
                        RotationParent = new GameObject("Rotation " + (index + 1) + " (Single)");

                        ResetTransform(MoveTrigger, DoorPro);
                        SetParentChild(RotationParent, MoveTrigger);
                        AddMoveTriggerScript(MoveTrigger, index);
                        NumberOfTriggers += 1;
                    }

                    if (DoorPro.transform.parent.transform.Find("Rotation " + (index + 1) + " (Looped)") == null && DoorPro.RotationTimeline[index].RotationType == DoorPro.RotationTimelineData.TypeOfRotation.LoopedRotation)
                    {
                        OpenTrigger = new GameObject("Open Trigger");
                        CloseTrigger = new GameObject("Close Trigger");
                        RotationParent = new GameObject("Rotation " + (index + 1) + " (Looped)");

                        ResetTransform(OpenTrigger, DoorPro);
                        SetParentChild(RotationParent, OpenTrigger);
                        AddOpenTriggerScript(OpenTrigger, index);

                        ResetTransform(CloseTrigger, DoorPro);
                        SetParentChild(RotationParent, CloseTrigger);
                        AddCloseTriggerScript(CloseTrigger, index);

                        NumberOfTriggers += 1;
                    }
                }
            }

            for (int index = 0; index < NumberOfBlocks; index++)
            {
                if (DoorPro.RotationTimeline[index].RotationType == DoorPro.RotationTimelineData.TypeOfRotation.LoopedRotation && DoorPro.transform.parent.transform.Find("Rotation " + (index + 1) + " (Single)"))
                {
                    if (!Application.isPlaying) DestroyImmediate(DoorPro.transform.parent.transform.Find("Rotation " + (index + 1) + " (Single)").gameObject);

                    OpenTrigger = new GameObject("Open Trigger");
                    CloseTrigger = new GameObject("Close Trigger");
                    RotationParent = new GameObject("Rotation " + (index + 1) + " (Looped)");

                    ResetTransform(OpenTrigger, DoorPro);
                    SetParentChild(RotationParent, OpenTrigger);
                    AddOpenTriggerScript(OpenTrigger, index);
                    OpenTrigger.GetComponent<OpenTrigger>().CustomGizmoColor = Color.green;

                    ResetTransform(CloseTrigger, DoorPro);
                    SetParentChild(RotationParent, CloseTrigger);
                    AddCloseTriggerScript(CloseTrigger, index);
                    CloseTrigger.GetComponent<CloseTrigger>().CustomGizmoColor = Color.red;
                }

                if (DoorPro.RotationTimeline[index].RotationType == DoorPro.RotationTimelineData.TypeOfRotation.SingleRotation && DoorPro.transform.parent.transform.Find("Rotation " + (index + 1) + " (Looped)"))
                {
                    if (!Application.isPlaying) DestroyImmediate(DoorPro.transform.parent.transform.Find("Rotation " + (index + 1) + " (Looped)").gameObject);

                    MoveTrigger = new GameObject("Move Trigger");
                    RotationParent = new GameObject("Rotation " + (index + 1) + " (Single)");

                    ResetTransform(MoveTrigger, DoorPro);
                    SetParentChild(RotationParent, MoveTrigger);
                    AddMoveTriggerScript(MoveTrigger, index);
                }
            }

            if (NumberOfTriggers > NumberOfBlocks)
            {
                while (NumberOfTriggers > NumberOfBlocks)
                {
                    if (DoorPro.transform.parent.transform.Find("Rotation " + NumberOfTriggers + " (Single)") != null)
                    {
                        DestroyImmediate(DoorPro.transform.parent.transform.Find("Rotation " + NumberOfTriggers + " (Single)").gameObject);
                        NumberOfTriggers--;
                    }

                    if (DoorPro.transform.parent.transform.Find("Rotation " + NumberOfTriggers + " (Looped)") != null)
                    {
                        DestroyImmediate(DoorPro.transform.parent.transform.Find("Rotation " + NumberOfTriggers + " (Looped)").gameObject);
                        NumberOfTriggers--;
                    }
                }
            }
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

    public static void AddOpenTriggerScript(GameObject Trigger, int index)
    {
        Trigger.AddComponent<OpenTrigger>();
        Trigger.GetComponent<OpenTrigger>().ID = index;
    }

    public static void AddCloseTriggerScript(GameObject Trigger, int index)
    {
        Trigger.AddComponent<CloseTrigger>();
        Trigger.GetComponent<CloseTrigger>().ID = index;
    }

    public static void AddMoveTriggerScript(GameObject Trigger, int index)
    {
        Trigger.AddComponent<MoveTrigger>();
        Trigger.GetComponent<MoveTrigger>().ID = index;
    }

    static class Styles
    {
        internal static GUIStyle helpbox;
        internal static GUIContent AxisError;

        static Styles()
        {
            AxisError = IconContent("CollabConflict", " The y-axis should be defined as 'up'.");
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
