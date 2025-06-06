﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Flock : MonoBehaviour
{
    [Serializable]
    enum FISH_TYPE
    {
        Starry,
        Spiky,
        Moony,
        Longy
    }
    
    [Header("Spawn Setup")]
    [SerializeField] private FlockUnit flockUnitPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 spawnBounds;
    [SerializeField] private FISH_TYPE _fishType;

    [Header("Speed Setup")]
    [Range(0, 10)]
    [SerializeField] private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0, 10)]
    [SerializeField] private float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }


    [Header("Detection Distances")]

    [Range(0, 10)]
    [SerializeField] private float _cohesionDistance;
    public float cohesionDistance { get { return _cohesionDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceDistance;
    public float avoidanceDistance { get { return _avoidanceDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _aligementDistance;
    public float aligementDistance { get { return _aligementDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _obstacleDistance;
    public float obstacleDistance { get { return _obstacleDistance; } }

    [Range(0, 100)]
    [SerializeField] private float _boundsDistance;
    public float boundsDistance { get { return _boundsDistance; } }


    [Header("Behaviour Weights")]

    [Range(0, 10)]
    [SerializeField] private float _cohesionWeight;
    public float cohesionWeight { get { return _cohesionWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceWeight;
    public float avoidanceWeight { get { return _avoidanceWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _aligementWeight;
    public float aligementWeight { get { return _aligementWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _boundsWeight;
    public float boundsWeight { get { return _boundsWeight; } }

    [Range(0, 100)]
    [SerializeField] private float _obstacleWeight;
    public float obstacleWeight { get { return _obstacleWeight; } }

    public FlockUnit[] allUnits { get; set; }

    private void Start()
    {
        GenerateUnits();
    }

    private void Update()
    {
        for (int i = 0; i < allUnits.Length; i++)
        {
            allUnits[i].MoveUnit();
        }
    }

    private void GenerateUnits()
    {
        allUnits = new FlockUnit[flockSize];
        for (int i = 0; i < flockSize; i++)
        {
            var randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            allUnits[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation);
            allUnits[i].myTransform.parent = transform;
            // Apply a random scale to the fish
            var randomScale = Random.Range(0.5f, 1.5f);
            allUnits[i].myTransform.localScale = new Vector3(randomScale, randomScale, randomScale);
            
            // Set random data in the shader
            var mr = allUnits[i].GetComponentInChildren<MeshRenderer>();

            switch (_fishType)
            {
                case FISH_TYPE.Starry :
                    mr.material.SetFloat("_Distance", Random.Range(0.1f, 0.3f));
                    mr.material.SetFloat("_Frequency", Random.Range(7f, 13f));
                    mr.material.SetFloat("_Speed", Random.Range(10f, 30f));
                    mr.material.SetFloat("_Power", Random.Range(11f, 15f));
                    break;
                case FISH_TYPE.Longy :
                    mr.material.SetFloat("_Distance", Random.Range(0.1f, 0.3f));
                    mr.material.SetFloat("_Frequency", Random.Range(7f, 13f));
                    mr.material.SetFloat("_Speed", Random.Range(10f, 30f));
                    mr.material.SetFloat("_Power", Random.Range(11f, 15f));
                    break;
                case FISH_TYPE.Moony :
                    mr.material.SetFloat("_Distance", Random.Range(0.1f, 0.3f));
                    mr.material.SetFloat("_Frequency", Random.Range(7f, 13f));
                    mr.material.SetFloat("_Speed", Random.Range(10f, 30f));
                    mr.material.SetFloat("_Power", Random.Range(11f, 15f));
                    break;
                case FISH_TYPE.Spiky :
                    mr.material.SetFloat("_Distance", Random.Range(0.5f, 1.5f));
                    mr.material.SetFloat("_Frequency", Random.Range(1f, 5f));
                    mr.material.SetFloat("_Speed", Random.Range(1f, 5f));
                    mr.material.SetFloat("_Power", Random.Range(0.75f, 2f));
                    break;
            }
            
            
            allUnits[i].AssignFlock(this);
            allUnits[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }
    }
}
