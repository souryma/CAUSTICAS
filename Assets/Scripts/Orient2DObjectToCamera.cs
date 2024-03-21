using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orient2DObjectToCamera : MonoBehaviour
{
    // Update is called once per frame
    [SerializeField] private bool freezeXZAxis = true;
    void LateUpdate()
    {
        if (freezeXZAxis)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.x+90f, Camera.main.transform.rotation.eulerAngles.y+180f, transform.rotation.z);
        }
        else
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
