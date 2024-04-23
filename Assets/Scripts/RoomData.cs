using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class RoomData
{
    public enum DIRECTION
    {
        NORTH,
        WEST,
        SOUTH,
        EAST
    }

    public GameObject prefab;

    private int2 _coordinates;
    public bool isAvailable;
    public bool isStart;
    public bool isEnd;
    public bool isNodeStart;
    public bool isNodeEnd;
    public bool isPath;
    public bool isCorner;
    public bool isCornerInverted;
    public bool containsDivingSuit;
    public DIRECTION blocDirection;
    public GameObject instantiatedGameObject;

    public RoomData(int2 coordinates)
    {
        _coordinates = coordinates;

        isAvailable = true;
        isStart = false;
        isEnd = false;
        isNodeStart = false;
        isNodeEnd = false;
        isPath = false;
        isCorner = false;
        isCornerInverted = false;
        containsDivingSuit = false;
        blocDirection = DIRECTION.WEST;
    }

    // Returns a direction to the next bloc
    public void SetBlockDirection(int2 block, int2 nextBlock)
    {
        if (nextBlock.x > block.x && nextBlock.y == block.y)
            blocDirection = DIRECTION.EAST;
        else if (nextBlock.x == block.x && nextBlock.y > block.y)
            blocDirection = DIRECTION.NORTH;
        else if (nextBlock.x < block.x && nextBlock.y == block.y)
            blocDirection = DIRECTION.WEST;
        else blocDirection = DIRECTION.SOUTH;
    }

    // Return true if the corner must be inversed
    public void SetInverse(DIRECTION currentDir, DIRECTION previousDir)
    {
        if (previousDir == DIRECTION.WEST && currentDir == DIRECTION.NORTH ||
            previousDir == DIRECTION.NORTH && currentDir == DIRECTION.EAST ||
            previousDir == DIRECTION.EAST && currentDir == DIRECTION.SOUTH ||
            previousDir == DIRECTION.SOUTH && currentDir == DIRECTION.WEST)
            isCornerInverted = true;
        else
            isCornerInverted = false;
    }

    public void InverseDirection(DIRECTION direction)
    {
        blocDirection = direction switch
        {
            DIRECTION.NORTH => DIRECTION.SOUTH,
            DIRECTION.SOUTH => DIRECTION.NORTH,
            DIRECTION.WEST => DIRECTION.EAST,
            DIRECTION.EAST => DIRECTION.WEST,
            _ => blocDirection
        };
    }
}