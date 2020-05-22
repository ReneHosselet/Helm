using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour
{
    //camera for ui
    private Camera cameraToLookAt;

    void Start()
    {
        cameraToLookAt = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
    }

    void Update()
    {
        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(cameraToLookAt.transform.position - v);
        transform.rotation = (cameraToLookAt.transform.rotation);
    }
}
