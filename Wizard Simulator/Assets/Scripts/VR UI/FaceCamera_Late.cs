using UnityEngine;
using System.Collections;

public class FaceCamera_Late : MonoBehaviour
{
    Camera referenceCamera;

    public bool reverseFace = true;

    void Awake() {
        // if no camera referenced, grab the main camera
        if (!referenceCamera) referenceCamera = Camera.main;
    }

    void LateUpdate() {
        // rotates the object relative to the camera
        if (referenceCamera != null) {
            // find the target position by multipling the rotation by either a forward or a back vector then adding it to the texts postion
            Vector3 targetPos = transform.position + referenceCamera.transform.rotation * (reverseFace ? Vector3.forward : Vector3.back);
            // simply look at the target postition
            transform.LookAt(targetPos);
        } else {
            // if no camera referenced, grab the main camera
            referenceCamera = Camera.main;
        }
    }
}