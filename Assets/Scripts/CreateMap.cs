using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CreateMap : MonoBehaviour
{
    public GameObject Player;
    public GameObject VacuumPrefab;

    public ProceduralGeneration proceduralGeneration;

    [SerializeField] private List<GameObject> _vacuums;

    // Start is called before the first frame update
    void Start()
    {
        _vacuums = new List<GameObject>();
        
        proceduralGeneration.GenerateRandom();

        // Spawn player
        Player.transform.position = new Vector3(proceduralGeneration.startingRoom.x * 10, 2,
            proceduralGeneration.startingRoom.y * 10);
        
        // spawn vaccum
        foreach (var endNode in proceduralGeneration.endingNodes)
        {
            GameObject vac = Instantiate(VacuumPrefab, transform);
            vac.transform.position = new Vector3(endNode.x * 10, 2, endNode.y * 10);
            _vacuums.Add(vac);
        }

        //proceduralGeneration.HideNonPath();
    }
}