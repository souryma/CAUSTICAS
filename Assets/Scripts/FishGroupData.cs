using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FishGroupData : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private string name = "";
    [SerializeField] private int numberOfFishes = 0;
    private List<GameObject> _fishes;

    public void SpawnFishes(GameObject parent)
    {
        for (int i = 0; i < numberOfFishes; i++)
        {
            _fishes.Add(Instantiate(prefab, parent.transform));
        }
    }
}