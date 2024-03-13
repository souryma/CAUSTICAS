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
    [HideInInspector] public bool isNodeStart = false;
    [HideInInspector] public bool isNodeEnd = false;
    [HideInInspector] public bool isPath = false;
    [HideInInspector] public bool isCorner = false;
    [HideInInspector] public bool isCornerInverted = false;
    [HideInInspector] public bool isTShapeInverted = false;
    [HideInInspector] public DIRECTION blocDirection = DIRECTION.WEST;
    public GameObject instantiatedGameObject;

    public void SetTBlockDirection((int, int) nextSideBlock, (int, int) nextMainblock)
    {
        // Default : direction NORTH = means no connexion to north
        
        // Check if there are tunnels on EAST and WEST, so direction is NORTH
        // if (nextSideBlock.Item1 < coordinates.x && nextSideBlock.Item2 == coordinates.y &&
        //     nextMainblock.Item1 > coordinates.x && nextMainblock.Item2 == coordinates.y && blocDirection == DIRECTION.NORTH)
        //     blocDirection = DIRECTION.NORTH;
        // if (nextSideBlock.Item1 < coordinates.x && nextSideBlock.Item2 == coordinates.y &&
        //     nextMainblock.Item1 > coordinates.x && nextMainblock.Item2 == coordinates.y && blocDirection == DIRECTION.SOUTH)
        //     blocDirection = DIRECTION.SOUTH;
        //
        // // Check if there are tunnels on EAST and NORTH, so direction is WEST
        // if (nextSideBlock.Item1 < coordinates.x && nextSideBlock.Item2 == coordinates.y &&
        //     nextMainblock.Item1 == coordinates.x && nextMainblock.Item2 > coordinates.y && blocDirection == DIRECTION.WEST)
        //     blocDirection = DIRECTION.SOUTH;
        // if (nextSideBlock.Item1 < coordinates.x && nextSideBlock.Item2 == coordinates.y &&
        //     nextMainblock.Item1 == coordinates.x && nextMainblock.Item2 > coordinates.y && blocDirection == DIRECTION.SOUTH)
        //     blocDirection = DIRECTION.WEST;

        // if (this.blocDirection == DIRECTION.NORTH && nextBlockSide.Item1 > this.coordinates.x &&
        //         nextBlockSide.Item2 == this.coordinates.y)
        //         this.isTShapeInverted = true;
        // if (this.blocDirection == DIRECTION.EAST && nextBlockSide.Item1 == this.coordinates.x &&
        //     nextBlockSide.Item2 < this.coordinates.y)
        //     this.isTShapeInverted = true;
        // if (this.blocDirection == DIRECTION.SOUTH && nextBlockSide.Item1 < this.coordinates.x &&
        //     nextBlockSide.Item2 == this.coordinates.y)
        //     this.isTShapeInverted = true;
        // if (this.blocDirection == DIRECTION.WEST && nextBlockSide.Item1 == this.coordinates.x &&
        //     nextBlockSide.Item2 > this.coordinates.y)
        //     this.isTShapeInverted = true;
    }

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

    // Return true if the corner must be inversed
    public static bool isInverse(DIRECTION currentDir, DIRECTION previousDir)
    {
        if (previousDir == DIRECTION.WEST && currentDir == DIRECTION.NORTH ||
            previousDir == DIRECTION.NORTH && currentDir == DIRECTION.EAST ||
            previousDir == DIRECTION.EAST && currentDir == DIRECTION.SOUTH ||
            previousDir == DIRECTION.SOUTH && currentDir == DIRECTION.WEST)
            return true;
        else
            return false;
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