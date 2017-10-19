using System.Collections;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class DoorPro : MonoBehaviour
{
    public List<RotationTimelineData> RotationTimeline;

    [System.Serializable]
    public class RotationTimelineData
    {
        public enum TypeOfRotation { SingleRotation, LoopedRotation }
        public TypeOfRotation RotationType;
        public float InitialAngle = 0f;
        public float FinalAngle = 90f;
        public float Speed = 4f;
        [ConditionalHide("RotationType", true, true)]
        public int TimesMoveable = 0;
    }
    public int CurrentIndex = 0;
    //Hinge settings
    public enum PositionOfHinge { Left, Right }
    public PositionOfHinge HingePosition;

    //Rotation progress
    public int ProgressionOfLoop = 0;
    public int TimesRotated = 0;
    public bool RotationPending = false;

    //Declare two rotations
    public Quaternion InitialRot, FinalRot;
    public int State;

    //Create a hinge
    GameObject hinge;

    //3rd party compatability
    public enum ScaleOfDoor { Unity3DUnits, Other }
    public ScaleOfDoor DoorScale;
    public enum PositionOfPivot { Centered, CorrectlyPositioned }
    public PositionOfPivot PivotPosition;

    //Angle settings
    public bool ShortestWay = true;
    public enum WayOfRotation { Default, Shortest }
    public WayOfRotation RotationWay;
    public enum ConventionAngle { DoorsPro, Unity, }
    public ConventionAngle AngleConvention;

    void Start()
    {
        gameObject.isStatic = true;
        gameObject.tag = "Door";

        #region Hinge Algorithm
        if (DoorScale == ScaleOfDoor.Unity3DUnits && PivotPosition == PositionOfPivot.Centered)
        {
            hinge = new GameObject("Hinge");

            ShortestWay = (RotationWay == WayOfRotation.Shortest) ? true : false;

            float CosDeg = Mathf.Cos((transform.eulerAngles.y * Mathf.PI) / 180);
            float SinDeg = Mathf.Sin((transform.eulerAngles.y * Mathf.PI) / 180);

            //Read transform (position/rotation/scale) of the door.
            float PosDoorX = transform.position.x;
            float PosDoorY = transform.position.y;
            float PosDoorZ = transform.position.z;

            float RotDoorX = transform.localEulerAngles.x;
            float RotDoorZ = transform.localEulerAngles.z;

            float ScaleDoorX = transform.localScale.x;
            float ScaleDoorZ = transform.localScale.z;

            //Create a placeholder of the hinge's position/rotation.
            Vector3 HingePosCopy = hinge.transform.position;
            Vector3 HingeRotCopy = hinge.transform.localEulerAngles;

            if (HingePosition == PositionOfHinge.Left)
            {
                if (transform.localScale.x > transform.localScale.z)
                {
                    HingePosCopy.x = (PosDoorX - (ScaleDoorX / 2 * CosDeg));
                    HingePosCopy.z = (PosDoorZ + (ScaleDoorX / 2 * SinDeg));
                    HingePosCopy.y = PosDoorY;

                    HingeRotCopy.x = RotDoorX;
                    HingeRotCopy.y = -RotationTimeline[0].InitialAngle;
                    HingeRotCopy.z = RotDoorZ;
                }

                else
                {
                    HingePosCopy.x = (PosDoorX + (ScaleDoorZ / 2 * SinDeg));
                    HingePosCopy.z = (PosDoorZ + (ScaleDoorZ / 2 * CosDeg));
                    HingePosCopy.y = PosDoorY;

                    HingeRotCopy.x = RotDoorX;
                    HingeRotCopy.y = -RotationTimeline[0].InitialAngle;
                    HingeRotCopy.z = RotDoorZ;
                }
            }

            if (HingePosition == PositionOfHinge.Right)
            {
                if (transform.localScale.x > transform.localScale.z)
                {
                    HingePosCopy.x = (PosDoorX + (ScaleDoorX / 2 * CosDeg));
                    HingePosCopy.z = (PosDoorZ - (ScaleDoorX / 2 * SinDeg));
                    HingePosCopy.y = PosDoorY;

                    HingeRotCopy.x = RotDoorX;
                    HingeRotCopy.y = -RotationTimeline[0].InitialAngle;
                    HingeRotCopy.z = RotDoorZ;
                }

                else
                {
                    HingePosCopy.x = (PosDoorX - (ScaleDoorZ / 2 * SinDeg));
                    HingePosCopy.z = (PosDoorZ - (ScaleDoorZ / 2 * CosDeg));
                    HingePosCopy.y = PosDoorY;

                    HingeRotCopy.x = RotDoorX;
                    HingeRotCopy.y = -RotationTimeline[0].InitialAngle;
                    HingeRotCopy.z = RotDoorZ;
                }
            }

            hinge.transform.parent = transform.parent;
            hinge.transform.position = HingePosCopy;
            transform.SetParent(hinge.transform);
            hinge.transform.localEulerAngles = HingeRotCopy;
        }
        #endregion
    }

    public IEnumerator Move()
    {
        //Get references
        DetectionPro detection = GameObject.Find("Player").GetComponent<DetectionPro>();
        //int CurrentIndex = detection.CurrentIndex;
        RotationTimelineData currentRotationBlock = RotationTimeline[CurrentIndex];

        if (currentRotationBlock.RotationType == RotationTimelineData.TypeOfRotation.SingleRotation)
            currentRotationBlock.TimesMoveable = 1;

        if (AngleConvention == ConventionAngle.DoorsPro)
        {
            InitialRot = Quaternion.Euler(0, -currentRotationBlock.InitialAngle, 0);
            FinalRot = Quaternion.Euler(0, -currentRotationBlock.FinalAngle, 0);
        }

        else
        {
            InitialRot = Quaternion.Euler(0, currentRotationBlock.InitialAngle, 0);
            FinalRot = Quaternion.Euler(0, currentRotationBlock.FinalAngle, 0);
        }

        if (DoorScale == ScaleOfDoor.Unity3DUnits && PivotPosition == PositionOfPivot.Centered)
        {
            //Set the initial rotation
            if (TimesRotated == 0)
                hinge.transform.rotation = InitialRot;

            if (ProgressionOfLoop < currentRotationBlock.TimesMoveable || currentRotationBlock.TimesMoveable == 0)
            {
                //Change state from 1 to 0 and back (= alternate between FinalRot and InitialRot).
                if (hinge.transform.rotation == (State == 0 ? FinalRot : InitialRot)) State ^= 1;

                //Set 'FinalRotation' to 'FinalRot' when moving and to 'InitialRot' when moving back.
                Quaternion FinalRotation = ((State == 0) ? FinalRot : InitialRot);

                //Make the door/window rotate until it is fully opened/closed.
                while (Mathf.Abs(Quaternion.Angle(FinalRotation, hinge.transform.rotation)) > 0)
                {
                    RotationPending = true;
                    hinge.transform.rotation = QuaternionExtension.Slerp(hinge.transform.rotation, FinalRotation, Time.deltaTime * currentRotationBlock.Speed, ShortestWay);
                    yield return new WaitForEndOfFrame();
                }
                TimesRotated++;
                ProgressionOfLoop++;

                if (ProgressionOfLoop == currentRotationBlock.TimesMoveable && currentRotationBlock.TimesMoveable != 0)
                {
                    ProgressionOfLoop = 0;
                    TimesRotated = 0;
                    CurrentIndex++;
                }
                RotationPending = false;
            }
        }

        if (PivotPosition == PositionOfPivot.CorrectlyPositioned)
        {
            //Set the initial rotation
            if (TimesRotated == 0)
                transform.rotation = InitialRot;

            if (ProgressionOfLoop < currentRotationBlock.TimesMoveable || currentRotationBlock.TimesMoveable == 0)
            {
                //Change state from 1 to 0 and back (= alternate between FinalRot and InitialRot).
                if (transform.rotation == (State == 0 ? FinalRot : InitialRot)) State ^= 1;

                //Set 'FinalRotation' to 'FinalRot' when moving and to 'InitialRot' when moving back.
                Quaternion FinalRotation = ((State == 0) ? FinalRot : InitialRot);

                //Make the door/window rotate until it is fully opened/closed.
                while (Mathf.Abs(Quaternion.Angle(FinalRotation, transform.rotation)) > 0)
                {
                    RotationPending = true;
                    transform.rotation = QuaternionExtension.Slerp(transform.rotation, FinalRotation, Time.deltaTime * currentRotationBlock.Speed, ShortestWay);
                    yield return new WaitForEndOfFrame();
                }
                TimesRotated++;
                ProgressionOfLoop++;

                if (ProgressionOfLoop == currentRotationBlock.TimesMoveable && currentRotationBlock.TimesMoveable != 0)
                {
                    ProgressionOfLoop = 0;
                    TimesRotated = 0;
                    CurrentIndex++;
                }
                RotationPending = false;
            }
        }
    }
}

public static class QuaternionExtension
{
    public static Quaternion Lerp(Quaternion p, Quaternion q, float t, bool shortWay)
    {
        if (shortWay)
        {
            float dot = Quaternion.Dot(p, q);
            if (dot < 0.0f)
                return Lerp(ScalarMultiply(p, -1.0f), q, t, true);
        }

        Quaternion r = Quaternion.identity;
        r.x = p.x * (1f - t) + q.x * (t);
        r.y = p.y * (1f - t) + q.y * (t);
        r.z = p.z * (1f - t) + q.z * (t);
        r.w = p.w * (1f - t) + q.w * (t);
        return r;
    }

    public static Quaternion Slerp(Quaternion p, Quaternion q, float t, bool shortWay)
    {
        float dot = Quaternion.Dot(p, q);
        if (shortWay)
        {
            if (dot < 0.0f)
                return Slerp(ScalarMultiply(p, -1.0f), q, t, true);
        }

        float angle = Mathf.Acos(dot);
        Quaternion first = ScalarMultiply(p, Mathf.Sin((1f - t) * angle));
        Quaternion second = ScalarMultiply(q, Mathf.Sin((t) * angle));
        float division = 1f / Mathf.Sin(angle);
        return ScalarMultiply(Add(first, second), division);
    }

    public static Quaternion ScalarMultiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    public static Quaternion Add(Quaternion p, Quaternion q)
    {
        return new Quaternion(p.x + q.x, p.y + q.y, p.z + q.z, p.w + q.w);
    }
}
