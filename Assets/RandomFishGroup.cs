using System.Collections.Generic;
using UnityEngine;

public class RandomFishGroup : MonoBehaviour
{
    private List<GameObject> fishes;

    // Start is called before the first frame update
    void Start()
    {
        fishes = new List<GameObject>();

        foreach (Transform child in transform)
            fishes.Add(child.gameObject);

        foreach (var fish in fishes)
        {
            var mr = fish.GetComponent<MeshRenderer>();
            mr.material.SetFloat("_Distance", Random.Range(0.1f, 0.3f));
            mr.material.SetFloat("_Frequency", Random.Range(7f, 13f));
            mr.material.SetFloat("_Speed", Random.Range(10f, 30f));
            mr.material.SetFloat("_Power", Random.Range(11f, 15f));
        }

        Destroy(this);
    }
}