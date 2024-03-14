using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class VacuumComponent : MonoBehaviour
{
    public ProceduralGeneration proceduralGeneration;

    private void SpawnVacuum()
    {
    }

    private Vector3 startPoint;
    private Vector3 endPoint;

    private void Start()
    {
        startPoint = Vector3.zero;
        endPoint = Vector3.zero;
    }

    [Range(0, 5)] public float Speed;

    private bool isAtPoint = true;
    private int _pathNumber = 0;

    private float _floorHeight = 0.114f;

    private void Update()
    {
        if (isAtPoint)
        {
            startPoint = new Vector3(proceduralGeneration.MainPath[_pathNumber].x, _floorHeight,
                proceduralGeneration.MainPath[_pathNumber].y);
            startPoint = new Vector3(proceduralGeneration.MainPath[_pathNumber + 1].x, _floorHeight,
                proceduralGeneration.MainPath[_pathNumber + 1].y);
            
            startPoint *= 10;
            endPoint *= 10;
            
            transform.position = new Vector3(startPoint.x, _floorHeight, startPoint.y);

            isAtPoint = false;
        }

        var direction = new Vector3(endPoint.x, 0, endPoint.y);
        //direction = direction.normalized;

        Debug.Log(direction.x);
        transform.position += Time.deltaTime * Speed * direction;

        if (transform.position == endPoint)
            isAtPoint = true;
    }
}