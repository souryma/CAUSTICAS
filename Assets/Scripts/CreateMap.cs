using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CreateMap : MonoBehaviour
{
    public GameObject Player;
    public GameObject Vacuum;

    public ProceduralGeneration proceduralGeneration;

    // Start is called before the first frame update
    void Start()
    {
        proceduralGeneration.GenerateRandom();

        Player.transform.position = new Vector3(proceduralGeneration.startingRoom.x * 10, 2,
            proceduralGeneration.startingRoom.y * 10);
        Vacuum.transform.position = new Vector3(proceduralGeneration.endingRoom.x * 10, 2,
            proceduralGeneration.endingRoom.y * 10);

        //proceduralGeneration.HideNonPath();
    }
}