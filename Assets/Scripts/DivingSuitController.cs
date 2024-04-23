using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivingSuitController : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            GameManager.instance.GetDivingSuit();
            gameObject.SetActive(false);
        }
    }
}
