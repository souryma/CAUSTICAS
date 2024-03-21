using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingSuitController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Set the rotation of the diving suit to the rotation of the camera
        transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.x, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z);
        //transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);

        
    }
}
