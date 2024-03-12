using System.Collections.Generic;
using System.Linq;
using AStar;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGeneration : MonoBehaviour
{
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
                if (random > 75)
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

        // Sets all blocs directions according to the path
        ComputePathDirection(_mainPath);

        // Sets all RoomData path bool at true
        foreach (var (item1, item2) in _mainPath)
        {
            _grid[item1, item2].isPath = true;
        }

        // Etape 5 : Dans le chemin trouvé, déterminer des cases qui seront un point de départ pour des chemins optionnels
        foreach (var (item1, item2) in _mainPath)
        {
            if (_grid[item1, item2].isStart || _grid[item1, item2].isEnd)
                continue;

            // 1 chances on 3 to create a node on path 
            if (Random.Range(0, 3) == 0)
            {
                _grid[item1, item2].isNodeStart = true;
            }
        }


        // Etape 6 : Pour chaque case de départ choisir une case de fin et lancer l’algorithme de pathfinding entre les deux
        CreateSideTunnels();

        SelectPrefab();
        Spawn();
    }

    private void ComputePathDirection(List<(int, int)> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            _grid[path[i].Item1, path[i].Item2].blocDirection = RoomData.GetBlockDirection(path[i], path[i + 1]);

            if (i == 0) continue;
            // Detect corner using previous block direction
            if (_grid[path[i].Item1, path[i].Item2].blocDirection !=
                _grid[path[i - 1].Item1, path[i - 1].Item2].blocDirection)
            {
                _grid[path[i].Item1, path[i].Item2].isCorner = true;
                _grid[path[i].Item1, path[i].Item2].isInverted = RoomData.isInverse(
                    _grid[path[i].Item1, path[i].Item2].blocDirection,
                    _grid[path[i - 1].Item1, path[i - 1].Item2].blocDirection);
            }
        }

        // Set direction for last bloc of the path
        _grid[path[^1].Item1, path[^1].Item2].blocDirection =
            RoomData.InverseDirection(_grid[path[^2].Item1, path[^2].Item2].blocDirection);
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
        _grid[_startingRoom.x, _startingRoom.y].isPath = true;
        _grid[_endingRoom.x, _endingRoom.y].isEnd = true;
        _grid[_endingRoom.x, _endingRoom.y].isAvailable = true;
        _grid[_endingRoom.x, _endingRoom.y].isPath = true;

        // Add corners to path
        _mainPath = fillCorners ? FillCorners(path) : path.ToList();
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
                if (_grid[x, y].isAvailable && _grid[x, y].isPath == false)
                    walkableMap[y, x] = true;
                else
                    walkableMap[y, x] = false;
            }
        }

        return walkableMap;
    }

    // Adds corners to the path, because the pathfinding algorithm uses diagonals
    private List<(int, int)> FillCorners((int, int)[] path)
    {
        List<(int, int)> newPath = new List<(int, int)>();

        for (int i = 0; i < path.Length; i++)
        {
            int x = path[i].Item1;
            int y = path[i].Item2;

            // Stop loop if we are at the last cell
            if (x == _endingRoom.x && y == _endingRoom.y)
            {
                newPath.Add((_endingRoom.x, _endingRoom.y));
                break;
            }

            newPath.Add(path[i]);

            if (x + 1 == path[i + 1].Item1 && y + 1 == path[i + 1].Item2)
            {
                // check if upper cell is available, add it, otherwise, add the right cell
                if (_grid[x, y + 1].isAvailable && _grid[x, y + 1].isPath == false)
                {
                    newPath.Add((x, y + 1));
                }
                else
                {
                    newPath.Add((x + 1, y));
                }
            }
            else
            {
                if (x - 1 == path[i + 1].Item1 && y + 1 == path[i + 1].Item2)
                {
                    // check if left cell is available, add it, otherwise, add the top cell
                    if (_grid[x - 1, y].isAvailable && _grid[x - 1, y].isPath == false)
                    {
                        newPath.Add((x - 1, y));
                    }
                    else
                    {
                        newPath.Add((x, y + 1));
                    }
                }
                else
                {
                    if (x - 1 == path[i + 1].Item1 && y - 1 == path[i + 1].Item2)
                    {
                        // check if bottom cell is available, add it, otherwise, add the left cell
                        if (_grid[x, y - 1].isAvailable && _grid[x, y - 1].isPath == false)
                        {
                            newPath.Add((x, y - 1));
                        }
                        else
                        {
                            newPath.Add((x - 1, y));
                        }
                    }
                    else
                    {
                        if (x + 1 == path[i + 1].Item1 && y - 1 == path[i + 1].Item2)
                        {
                            // check if right cell is available, add it, otherwise, add the bottom cell
                            if (_grid[x + 1, y].isAvailable && _grid[x + 1, y].isPath == false)
                            {
                                newPath.Add((x + 1, y));
                            }
                            else
                            {
                                newPath.Add((x, y - 1));
                            }
                        }
                    }
                }
            }
        }

        return newPath;
    }

    private void CreateSideTunnels()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (!_grid[x, y].isNodeStart) continue;

                (int, int)[] path = new (int, int)[0];

                _endingRoom = new int2(x, y);
                // Keep searching until a valid end is found
                while (_grid[_endingRoom.x, _endingRoom.y].isAvailable == false ||
                       _grid[_endingRoom.x, _endingRoom.y].isPath == true)
                {
                    int endX = x + Random.Range(-5, 5);
                    while (endX < 0 || endX > gridSize.x - 1)
                        endX = x + Random.Range(-5, 5);

                    int endY = y + Random.Range(-5, 5);
                    while (endY < 0 || endY > gridSize.y - 1)
                        endY = y + Random.Range(-5, 5);

                    _endingRoom = new int2(endX, endY);
                }
                
                path = AStarPathfinding.GeneratePathSync(x, y, _endingRoom.x, _endingRoom.y, GeneratesWalkableMap());

                // If no path is found, skip
                if (path.Length == 0)
                {
                    _grid[x, y].isNodeStart = false;
                    continue;
                }
                
                _grid[_endingRoom.x, _endingRoom.y].isNodeEnd = true;

                List<(int, int)> newPath = new List<(int, int)>();

                // Add corners to path
                if (fillCorners)
                    newPath = FillCorners(path);
                else
                    newPath = path.ToList();

                foreach (var (item1, item2) in newPath)
                {
                    _grid[item1, item2].isPath = true;
                }

                // Set the right direction for each side tunnels
                ComputePathDirection(newPath);
            }
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
                    _grid[x, y].prefab = roomTypes[5];
                }

                if (_grid[x, y].isPath)
                    _grid[x, y].prefab = roomTypes[1];

                if (_grid[x, y].isStart || _grid[x, y].isEnd || _grid[x, y].isNodeEnd)
                    _grid[x, y].prefab = roomTypes[4];

                if (_grid[x, y].isCorner)
                    _grid[x, y].prefab = roomTypes[2];
                
                if (_grid[x, y].isNodeStart)
                    _grid[x, y].prefab = roomTypes[3];
            }
        }
    }

    private void OrientGameobjectAccordingToDirection(ref GameObject go, RoomData.DIRECTION direction)
    {
        switch (direction)
        {
            case RoomData.DIRECTION.WEST:
                go.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case RoomData.DIRECTION.NORTH:
                go.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case RoomData.DIRECTION.EAST:
                go.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case RoomData.DIRECTION.SOUTH:
                go.transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
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

                OrientGameobjectAccordingToDirection(ref tile, _grid[x, y].blocDirection);
                if (_grid[x, y].isInverted)
                {
                    Vector3 rotation = tile.transform.rotation.eulerAngles;
                    rotation.y += 90;
                    tile.transform.rotation = Quaternion.Euler(rotation);
                }

                // Color all path cells
                if (_grid[x, y].isPath)
                {
                    foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.material.color = Color.cyan;
                    }
                }

                // Color main path cells
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

                // color starting optionnal nodes
                if (_grid[x, y].isNodeStart)
                {
                    foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.material.color = new Color(1, 0.5f, 0);
                    }
                }
                
                if (_grid[x, y].isNodeEnd)
                {
                    foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.material.color = new Color(0.5f, 0f, 1f);
                    }
                }
            }
        }
    }
}