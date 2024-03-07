using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] private GameObject bigRoomPrefab;

    [SerializeField] private float2 gridSize; 

    private GameObject[,] grid;
    
    
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y] = bigRoomPrefab;
            }
        }
    }
}
