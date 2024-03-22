using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CreateMap : MonoBehaviour
{
    public GameObject Player;

    public ProceduralGeneration proceduralGeneration;
    
    // Start is called before the first frame update
    void Start()
    {
        int2 start = proceduralGeneration.GenerateRandom();

        Player.transform.position = new Vector3(start.x, 2, start.y);
        
        //proceduralGeneration.HideNonPath();
    }
}
