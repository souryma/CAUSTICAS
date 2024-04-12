using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class VacuumComponent : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField, Range(0, 0.1f)] private float speed = 0.03f;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed);
        
        transform.LookAt(target.transform);
    }
}