using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour {

    Camera refCam;

    public float maxDst = 1.0f;
    public float minYRot = 0.0f;
    public float maxYRot = 360.0f;

    // allows me to clamp angles within the eulerangles which converts it from -180 to 180... to 0 to 360
    // be carefull with negative numbers and min and max
    float ClampAngle(float angle, float min, float max) {
        if (angle < 90 || angle > 270) {
            if (angle > 180) angle -= 360.0f;
            if (max > 180) max -= 360.0f;
            if (min > 180) min -= 360.0f;
        }
        angle = Mathf.Clamp(angle, min, max);
        if (angle < 0) angle += 360;
        return angle;
    }

    void Start() {
        // if no camera referenced, grab the main camera
        if (!refCam) refCam = Camera.main;
    }

    void Awake() {
        // if no camera referenced, grab the main camera
        if (!refCam) refCam = Camera.main;
    }

    void OnValidate() {
        // must clamp the dst so there is no negative values.
        //Be careful a 0 value will not allow the text to apear
        if (maxDst < 0.0f)
            maxDst = 0.0f;
        // Must clamp the values to not allow for a rang outside of 0 to 360
        if (minYRot < 0.0f)
            minYRot = 0.0f;
        if (maxYRot > 360.0f)
            maxYRot = 360.0f;
    }

    void Update() {
        // find the distance between the text and the main camera
        float distance = Vector3.Distance(transform.position, refCam.transform.position);
        // if the distance is withing the max distance set enable the mesh otherwise it should be invisible
        if (distance <= maxDst)
            GetComponent<MeshRenderer>().enabled = true;
        else
            GetComponent<MeshRenderer>().enabled = false;
    }

    void LateUpdate() {
        if (refCam != null) {
            Quaternion clampedRot = new Quaternion();
            // create a new rotator to eliminate the X rotation and to clamp the y rotation to the defined permeters
            clampedRot.eulerAngles = new Vector3(0.0f, ClampAngle(refCam.transform.eulerAngles.y, minYRot, maxYRot), refCam.transform.eulerAngles.z);
            // find the target position by multipling the rotation by a forward vector then adding it to the texts postion
            Vector3 targetPos = transform.position + clampedRot * (Vector3.forward);
            // simply look at the target postition
            transform.LookAt(targetPos);
        } else {
            // if no camera referenced, grab the main camera
            refCam = Camera.main;
        }
    }
}
