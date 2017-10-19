using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OpenTrigger : MonoBehaviour
{
    public int ID;

    #region Player Requirements
    public bool CorrectTag, CorrectName, CorrectView, CorrectButton, CorrectScript;
    public bool HasTag, HasName, IsLookingAt, HasPressed, HasScript;
    public string Tag, Name, Character, ScriptName;
    public GameObject Object = null;
    #endregion

    #region Gizmo Settings
    public bool DrawGizmo = true;
    public Color CustomGizmoColor = Color.green;
    public float CustomGizmoColorAlpha = 0.5f;
    public bool DrawWire = false;
    public Color CustomWireColor = Color.black;
    public float CustomWireColorAlpha = 1f;
    #endregion

    void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        CustomGizmoColor.a = CustomGizmoColorAlpha;
        Gizmos.color = CustomGizmoColor;
        if (GetComponent<BoxCollider>() && DrawGizmo) Gizmos.DrawCube(Vector3.zero, Vector3.one);
        Gizmos.color = CustomWireColor;
        if (GetComponent<BoxCollider>() && DrawWire) Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }

    void OnTriggerStay(Collider other)
    {
        DoorPro doorpro = GameObject.Find(transform.parent.transform.parent.name).GetComponent<DoorPro>();
        DetectionPro detection = GameObject.Find("Player").GetComponent<DetectionPro>();

        if (Object)
            detection.CheckUIPrefabs(Object);

        if (!doorpro.RotationPending && doorpro.CurrentIndex < doorpro.RotationTimeline.Count && doorpro.CurrentIndex == ID)
        {
            if (doorpro.ProgressionOfLoop % 2 == 0)
            {
                CorrectTag = (!HasTag || other.tag == Tag);
                CorrectName = (!HasName || other.name == Name);
                CorrectView = (!IsLookingAt || detection.CheckIfLookingAt(Object));
                CorrectButton = (!HasPressed || Input.GetKey(Character));
                CorrectScript = (!HasScript || other.gameObject.GetComponent(ScriptName) != null);

                if (CorrectTag && CorrectName && CorrectView && CorrectButton && CorrectScript) StartCoroutine(doorpro.Move());
            }
        }
    }
}