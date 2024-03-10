using System.Collections.Generic;
using System.Linq;
using AStar;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGeneration : MonoBehaviour
{
    // [SerializeField] private List<RoomData> roomTypes;
    [SerializeField] private List<GameObject> roomTypes;
    [SerializeField] private int2 gridSize;
    [SerializeField] private bool fillCorners = true;

    private RoomData[,] _grid;

    private List<(int, int)> _mainPath;
    private int2 _startingRoom;
    private int2 _endingRoom;

    // Represent the map with blocking tiles (false = block)

    private int _seed;

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

        _seed = Random.Range(-9999999, 9999999);
        Random.InitState(_seed);

        // ETAPE 1 : Créer un tableau en 2 dimensions
        _grid = new RoomData[gridSize.x, gridSize.y];
        _mainPath = new List<(int, int)>();

        // Fill Grid with template block
        for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                _grid[x, y] = ScriptableObject.CreateInstance<RoomData>();
                _grid[x, y].coordinates = new int2(x, y);
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
                    _grid[x, y].isAvailable = false;
                }
                else
                {
                    _grid[x, y].isAvailable = true;
                }
            }
        }

        // Creates the starting point and the ending point and finds the shortest path between the two
        FindPath();

        
        // Etape 5 : Dans le chemin trouvé, déterminer des cases qui seront un point de départ pour des chemins optionnels
        // Etape 6 : Pour chaque case de départ choisir une case de fin et lancer l’algorithme de pathfinding entre les deux

        SelectPrefab();
        Spawn();
    }

    private void FindPath()
    {
        (int, int)[] path = new (int, int)[0];

        // Etape 3 : Choisir une case de début et de fin
        while (path.Length == 0)
        {
            _startingRoom = new int2(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
            _endingRoom = new int2(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));

            // Etape 4 : Lancer l’algorithme de pathfinding entre la case de début et de fin
            path = AStarPathfinding.GeneratePathSync(_startingRoom.x, _startingRoom.y, _endingRoom.x,
                _endingRoom.y, GeneratesWalkableMap());
        }

        // Update start and end tile on grid
        _grid[_startingRoom.x, _startingRoom.y].isStart = true;
        _grid[_startingRoom.x, _startingRoom.y].isAvailable = true;
        _grid[_endingRoom.x, _endingRoom.y].isEnd = true;
        _grid[_endingRoom.x, _endingRoom.y].isAvailable = true;

        // Add corners to path
        if (fillCorners)
            FillCorners(path);
        else 
            _mainPath = path.ToList();
            
    }

    // Generates a 2D array containing bool representing blockers on the map (false = tile blocked)
    private bool[,] GeneratesWalkableMap()
    {
        bool[,] walkableMap = new bool[gridSize.y, gridSize.x];

        // Generate the walkable map
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (_grid[x, y].isAvailable)
                    walkableMap[y, x] = true;
                else
                    walkableMap[y, x] = false;
            }
        }

        return walkableMap;
    }

    // Adds corners to the path, because the pathfinding algorithm uses diagonals
    private void FillCorners((int, int)[] path)
    {
        for (int i = 0; i < path.Length; i++)
        {
            int x = path[i].Item1;
            int y = path[i].Item2;

            // Stop loop if we are at the last cell
            if (x == _endingRoom.x && y == _endingRoom.y)
                break;

            if (x + 1 == path[i + 1].Item1 && y + 1 == path[i + 1].Item2)
            {
                // check if upper cell is available, add it, otherwise, add the right cell
                _mainPath.Add(_grid[x, y + 1].isAvailable ? (x, y + 1) : (x + 1, y));
            }
            else
            {
                if (x - 1 == path[i + 1].Item1 && y + 1 == path[i + 1].Item2)
                {
                    // check if left cell is available, add it, otherwise, add the top cell
                    _mainPath.Add(_grid[x - 1, y].isAvailable ? (x - 1, y) : (x, y + 1));
                }
                else
                {
                    if (x - 1 == path[i + 1].Item1 && y - 1 == path[i + 1].Item2)
                    {
                        // check if bottom cell is available, add it, otherwise, add the left cell
                        _mainPath.Add(_grid[x, y - 1].isAvailable ? (x, y - 1) : (x - 1, y));
                    }
                    else
                    {
                        if (x + 1 == path[i + 1].Item1 && y - 1 == path[i + 1].Item2)
                        {
                            // check if right cell is available, add it, otherwise, add the bottom cell
                            _mainPath.Add(_grid[x + 1, y].isAvailable ? (x + 1, y) : (x, y - 1));
                        }
                    }
                }
            }

            _mainPath.Add(path[i]);
        }
    }

    private void SelectPrefab()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (_grid[x, y].isAvailable == false)
                {
                    _grid[x, y].prefab = roomTypes[0];
                }
                else
                {
                    _grid[x, y].prefab = roomTypes[1];
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
                var tile = Instantiate(_grid[x, y].prefab, transform);
                tile.transform.localPosition = new Vector3(x * 10, 0f, y * 10);

                if (_grid[x, y].isAvailable == false)
                {
                    foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.material.color = Color.red;
                    }
                }

                // Color path cells
                foreach (var (item1, item2) in _mainPath)
                {
                    if (x == item1 && y == item2)
                    {
                        foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                        {
                            mr.material.color = Color.yellow;
                        }
                    }
                }

                if (_grid[x, y].isStart)
                {
                    foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.material.color = Color.green;
                    }
                }

                if (_grid[x, y].isEnd)
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