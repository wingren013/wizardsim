using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceDemo : MonoBehaviour {

    void OnMouseDown()
    {
        GetComponent<Rigidbody>().AddForce(-transform.forward * 1000);
        GetComponent<Rigidbody>().useGravity = true;
    }
}
