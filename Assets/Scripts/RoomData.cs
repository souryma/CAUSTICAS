using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Room", fileName = "Room")]
public class RoomData : ScriptableObject
{
    public enum DIRECTION
    {
        NORTH,
        WEST,
        SOUTH,
        EAST
    }
    [SerializeField] public GameObject prefab;

    [SerializeField, Range(1, 4)] public int numberOfOpenings;

    [HideInInspector] public int2 coordinates;
    [HideInInspector] public bool isAvailable = true;
    [HideInInspector] public bool isStart = false;
    [HideInInspector] public bool isEnd = false;
    [HideInInspector] public bool isNode = false;
    [HideInInspector] public bool isPath = false;
    [HideInInspector] public bool isCorner = false;
    [HideInInspector] public DIRECTION blocDirection = DIRECTION.WEST;
    
    // Returns a direction to the next bloc
    public static DIRECTION GetBlockDirection((int, int) block, (int, int) nextBlock)
    {
        DIRECTION direction = DIRECTION.SOUTH;

        if (nextBlock.Item1 > block.Item1 && nextBlock.Item2 == block.Item2)
            direction = DIRECTION.EAST;
        else if (nextBlock.Item1 == block.Item1 && nextBlock.Item2 > block.Item2)
            direction = DIRECTION.NORTH;
        else if (nextBlock.Item1 < block.Item1 && nextBlock.Item2 == block.Item2)
            direction = DIRECTION.WEST;
        // other case must be south SOUTH

        return direction;
    }

    public static DIRECTION InverseDirection(DIRECTION direction)
    {
        DIRECTION inverse = DIRECTION.SOUTH;

        switch (direction)
        {
            case DIRECTION.NORTH:
                inverse = DIRECTION.SOUTH;
                break;
            case DIRECTION.SOUTH:
                inverse = DIRECTION.NORTH;
                break;
            case DIRECTION.WEST:
                inverse = DIRECTION.EAST;
                break;
            case DIRECTION.EAST:
                inverse = DIRECTION.WEST;
                break;
        }

        return inverse;
    }
}