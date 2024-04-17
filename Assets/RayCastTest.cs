using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray downRay = new Ray(transform.position, Vector3.left);

        if (Physics.Raycast(downRay, out hit))
        {
            Debug.Log(hit.distance);
            Debug.DrawLine (transform.position, hit.point,Color.red);
        }
    }
}
