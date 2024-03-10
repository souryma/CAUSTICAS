using System.Collections.Generic;
using AStar;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGeneration : MonoBehaviour
{
    // [SerializeField] private List<RoomData> roomTypes;
    [SerializeField] private List<GameObject> roomTypes;

    [SerializeField] private int numberOfRooms;

    private RoomData[,] grid;
    public int2 gridSize;

    // Represent the map with blocking tiles (false = block)
    private bool[,] walkableMap;

    private int seed;

    private void Clear()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }


    // Start is called before the first frame update
    public void GenerateRandom()
    {
        Clear();

        seed = Random.Range(-9999999, 9999999);
        Random.InitState(seed);

        // ETAPE 1 : Créer un tableau en 2 dimensions
        grid = new RoomData[gridSize.x, gridSize.y];
        walkableMap = new bool[gridSize.y, gridSize.x];

        // Fill Grid with template block
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                grid[x, y] = ScriptableObject.CreateInstance<RoomData>();
                grid[x, y].coordinates = new int2(x, y);
            }
        }

        // ETAPE 2 : Choisir des cases interdites pour l'algorithmes de path finding
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                int random = Random.Range(0, 100);
                if (random > 70)
                {
                    grid[x, y].isAvailable = false;
                }
                else
                {
                    grid[x, y].isAvailable = true;
                }
            }
        }

        


        FindPath();


        /*
        Etape 5 : Dans le chemin trouvé, déterminer des cases qui seront un point de départ pour des chemins optionnels
        Etape 6 : Pour chaque case de départ choisir une case de fin et lancer l’algorithme de pathfinding entre les deux
         */

        SelectPrefab();
        Spawn();
    }

    private (int, int)[] path;
    private int2 startingRoom;
    private int2 endingRoom;

    private void FindPath()
    {
        path = new (int, int)[0];
        
        // Etape 3 : Choisir une case de début et de fin
        while (path.Length == 0)
        {
            startingRoom = new int2(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
            endingRoom = new int2(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
            
            // Generate the walkable map
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (grid[x, y].isAvailable)
                        walkableMap[y, x] = true;
                    else
                        walkableMap[y, x] = false;
                }
            }
            
            // Etape 4 : Lancer l’algorithme de pathfinding entre la case de début et de fin
            path = AStarPathfinding.GeneratePathSync(startingRoom.x, startingRoom.y, endingRoom.x, endingRoom.y, walkableMap, true, false);
        }
        
        grid[startingRoom.x, startingRoom.y].isStart = true;
        grid[startingRoom.x, startingRoom.y].isAvailable = true;
        grid[endingRoom.x, endingRoom.y].isEnd = true;
        grid[endingRoom.x, endingRoom.y].isAvailable = true;
    }

    private void SelectPrefab()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (grid[x, y].isAvailable == false)
                {
                    grid[x, y].prefab = roomTypes[0];
                }
                else
                {
                    grid[x, y].prefab = roomTypes[1];
                }
            }
        }
    }

    private void Spawn()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var tile = Instantiate(grid[x, y].prefab, transform);
                tile.transform.localPosition = new Vector3(x * 10, 0f, y * 10);

                if (grid[x, y].isAvailable == false)
                {
                    foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.material.color = Color.red;
                    }
                }

                // Color path cells
                foreach ((int, int) cordinate in path)
                {
                    if (x == cordinate.Item1 && y == cordinate.Item2)
                    {
                        foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                        {
                            mr.material.color = Color.yellow;
                        }
                    }
                }
                
                if (grid[x, y].isStart)
                {
                    foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.material.color = Color.green;
                    }
                }

                if (grid[x, y].isEnd)
                {
                    foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.material.color = Color.blue;
                    }
                }
            }
        }
    }
}