using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Room", fileName = "Room")]
public class RoomData : ScriptableObject
{
    [SerializeField] public GameObject prefab;

    [SerializeField, Range(1, 4)] public int numberOfOpenings;

    public int2 coordinates;
    public bool isAvailable = true;
    public bool isStart = false;
    public bool isEnd = false;
}