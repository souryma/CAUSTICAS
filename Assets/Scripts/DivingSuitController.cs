using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingSuitController : MonoBehaviour
{
    [SerializeField] private GameObject divingSuit;
    
    // Start is called before the first frame update
    void Start()
    {
        divingSuit.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.hasDivingSuit)
        {
            // Set the rotation of the diving suit to the rotation of the camera
            transform.rotation = Quaternion.Euler(Camera.main.transform.rotation.x, Camera.main.transform.rotation.y, Camera.main.transform.rotation.z);
            //transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
        }
    }
}
