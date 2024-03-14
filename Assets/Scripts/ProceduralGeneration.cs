using System.Collections.Generic;
using AStar;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProceduralGeneration : MonoBehaviour
{
    [SerializeField] private List<GameObject> roomTypes;
    [SerializeField] private int2 gridSize;
    private bool _showGrid = true;

    private RoomData[,] _grid;

    private List<int2> _mainPath;
    private List<List<int2>> _sidePaths;
    private int2 _startingRoom;
    private int2 _endingRoom;

    private int _seed;

    private void Clear()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }


    // Returns the stating room coordinates
    public int2 GenerateRandom()
    {
        Clear();

        _seed = Random.Range(-9999999, 9999999);
        Random.InitState(_seed);

        // ETAPE 1 : Créer un tableau en 2 dimensions
        _grid = new RoomData[gridSize.x, gridSize.y];
        _mainPath = new List<int2>();
        _sidePaths = new List<List<int2>>();

        // Fill Grid with template block
        for (int x = 0; x < _grid.GetLength(0); x++)
        {
            for (int y = 0; y < _grid.GetLength(1); y++)
            {
                _grid[x, y] = new RoomData(new int2(x, y));
            }
        }

        // Creates the starting point and the ending point and finds the shortest path between the two
        FindMainPath();

        // Sets all blocs directions according to the path
        ComputePathDirection(_mainPath);

        // Etape 5 : Dans le chemin trouvé, déterminer des cases qui seront un point de départ pour des chemins optionnels
        foreach (var coordinate in _mainPath)
        {
            if (_grid[coordinate.x, coordinate.y].isStart || _grid[coordinate.x, coordinate.y].isEnd)
                continue;

            // 1 chances on 3 to create a node on path 
            if (Random.Range(0, 3) == 0)
            {
                _grid[coordinate.x, coordinate.y].isNodeStart = true;
            }
        }

        // Etape 6 : Pour chaque case de départ choisir une case de fin et lancer l’algorithme de pathfinding entre les deux
        CreateSideTunnels();

        OrientTshaped();

        SelectPrefab();
        Spawn();

        return _startingRoom * 10;
    }

    // 25% chance for a block to be a blocker
    private void SelectBlockers()
    {
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
    }

    private void OrientTshaped()
    {
        foreach (var sidePath in _sidePaths)
        {
            var currentTshape = sidePath[0];

            // Check edge case
            if (currentTshape.x == gridSize.x - 1)
            {
                _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.EAST;
                continue;
            }

            if (currentTshape.x == 0)
            {
                _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.WEST;
                continue;
            }

            if (currentTshape.y == gridSize.y - 1)
            {
                _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.NORTH;
                continue;
            }

            if (currentTshape.y == 0)
            {
                _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.SOUTH;
                continue;
            }

            // Check case where there are 4 path around
            if (_grid[currentTshape.x - 1, currentTshape.y].isPath &&
                _grid[currentTshape.x, currentTshape.y + 1].isPath &&
                _grid[currentTshape.x, currentTshape.y - 1].isPath &&
                _grid[currentTshape.x + 1, currentTshape.y].isPath)
            {
                // Check WEST
                if (isTileOnPath((currentTshape.x - 1, currentTshape.y), sidePath) == false &&
                    isTileOnPath((currentTshape.x - 1, currentTshape.y), _mainPath) == false)
                {
                    _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.WEST;
                    continue;
                }

                // Check NORTH
                if (isTileOnPath((currentTshape.x, currentTshape.y + 1), sidePath) == false &&
                    isTileOnPath((currentTshape.x, currentTshape.y + 1), _mainPath) == false)
                {
                    _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.NORTH;
                    continue;
                }

                // Check SOUTH
                if (isTileOnPath((currentTshape.x, currentTshape.y - 1), sidePath) == false &&
                    isTileOnPath((currentTshape.x, currentTshape.y - 1), _mainPath) == false)
                {
                    _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.SOUTH;
                    continue;
                }

                // If not one of the 3 above : EAST
                _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.EAST;
            }

            // if no block in the direction -> the Tshape must be in that direction 
            if (_grid[currentTshape.x + 1, currentTshape.y].isPath == false)
            {
                _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.EAST;
            }
            else if (_grid[currentTshape.x, currentTshape.y - 1].isPath == false)
            {
                _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.SOUTH;
            }
            else if (_grid[currentTshape.x - 1, currentTshape.y].isPath == false)
            {
                _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.WEST;
            }
            else _grid[currentTshape.x, currentTshape.y].blocDirection = RoomData.DIRECTION.NORTH;
        }
    }

    // Returns true if the tile is in the path
    private bool isTileOnPath((int, int) tile, List<int2> path)
    {
        bool ret = false;

        foreach (var pathTile in path)
        {
            if (tile.Item1 == pathTile.x && tile.Item2 == pathTile.y)
                ret = true;
        }

        return ret;
    }

    private void ComputePathDirection(List<int2> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            _grid[path[i].x, path[i].y].SetBlockDirection(path[i], path[i + 1]);

            if (i == 0) continue;
            // Detect corner using previous block direction
            if (_grid[path[i].x, path[i].y].blocDirection !=
                _grid[path[i - 1].x, path[i - 1].y].blocDirection)
            {
                _grid[path[i].x, path[i].y].isCorner = true;
                _grid[path[i].x, path[i].y].SetInverse(
                    _grid[path[i].x, path[i].y].blocDirection,
                    _grid[path[i - 1].x, path[i - 1].y].blocDirection);
            }
        }

        // Set direction for last bloc of the path
        _grid[path[^1].x, path[^1].y].InverseDirection(_grid[path[^2].x, path[^2].y].blocDirection);
    }

    private void FindMainPath()
    {
        (int, int)[] path = new (int, int)[0];

        // Etape 3 : Choisir une case de début et de fin
        while (path.Length == 0)
        {
            SelectBlockers();

            _startingRoom = new int2(Random.Range(0, gridSize.x / 5), Random.Range(0, gridSize.y));
            _endingRoom = new int2(Random.Range(gridSize.x / 2, gridSize.x), Random.Range(0, gridSize.y));

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
        _mainPath = FillCorners(path);

        // Tag all path elements
        foreach (var coordinates in _mainPath)
        {
            _grid[coordinates.x, coordinates.y].isPath = true;
        }
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
    private List<int2> FillCorners((int, int)[] path)
    {
        List<int2> newPath = new List<int2>();

        for (int i = 0; i < path.Length; i++)
        {
            int x = path[i].Item1;
            int y = path[i].Item2;

            // Stop loop if we are at the last cell
            if (x == _endingRoom.x && y == _endingRoom.y)
            {
                newPath.Add(new int2(_endingRoom.x, _endingRoom.y));
                break;
            }

            newPath.Add(new int2(path[i].Item1, path[i].Item2));

            if (x + 1 == path[i + 1].Item1 && y + 1 == path[i + 1].Item2)
            {
                // check if upper cell is available, add it, otherwise, add the right cell
                if (_grid[x, y + 1].isAvailable && _grid[x, y + 1].isPath == false)
                {
                    newPath.Add(new int2(x, y + 1));
                }
                else
                {
                    newPath.Add(new int2(x + 1, y));
                }
            }
            else
            {
                if (x - 1 == path[i + 1].Item1 && y + 1 == path[i + 1].Item2)
                {
                    // check if left cell is available, add it, otherwise, add the top cell
                    if (_grid[x - 1, y].isAvailable && _grid[x - 1, y].isPath == false)
                    {
                        newPath.Add(new int2(x - 1, y));
                    }
                    else
                    {
                        newPath.Add(new int2(x, y + 1));
                    }
                }
                else
                {
                    if (x - 1 == path[i + 1].Item1 && y - 1 == path[i + 1].Item2)
                    {
                        // check if bottom cell is available, add it, otherwise, add the left cell
                        if (_grid[x, y - 1].isAvailable && _grid[x, y - 1].isPath == false)
                        {
                            newPath.Add(new int2(x, y - 1));
                        }
                        else
                        {
                            newPath.Add(new int2(x - 1, y));
                        }
                    }
                    else
                    {
                        if (x + 1 == path[i + 1].Item1 && y - 1 == path[i + 1].Item2)
                        {
                            // check if right cell is available, add it, otherwise, add the bottom cell
                            if (_grid[x + 1, y].isAvailable && _grid[x + 1, y].isPath == false)
                            {
                                newPath.Add(new int2(x + 1, y));
                            }
                            else
                            {
                                newPath.Add(new int2(x, y - 1));
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

                // Do not try more than 20 times
                int numberOfTries = 0;
                bool searchFailed = false;
                // Keep searching until a valid end is found
                while (_grid[_endingRoom.x, _endingRoom.y].isAvailable == false ||
                       _grid[_endingRoom.x, _endingRoom.y].isPath == true)
                {
                    int endX = x + Random.Range(-5, 5);
                    while (endX < 0 || endX > gridSize.x - 1)
                    {
                        endX = x + Random.Range(-5, 5);
                    }

                    int endY = y + Random.Range(-5, 5);
                    while (endY < 0 || endY > gridSize.y - 1)
                    {
                        endY = y + Random.Range(-5, 5);
                    }

                    _endingRoom = new int2(endX, endY);

                    if (numberOfTries > 20)
                    {
                        searchFailed = true;
                        break;
                    }

                    numberOfTries++;
                }

                if (searchFailed) continue;

                path = AStarPathfinding.GeneratePathSync(x, y, _endingRoom.x, _endingRoom.y, GeneratesWalkableMap());

                // If no path is found, skip
                if (path.Length == 0)
                {
                    _grid[x, y].isNodeStart = false;
                    continue;
                }

                _grid[_endingRoom.x, _endingRoom.y].isNodeEnd = true;

                List<int2> newPath = new List<int2>();

                // Add corners to path
                newPath = FillCorners(path);

                foreach (var coordinates in newPath)
                {
                    _grid[coordinates.x, coordinates.y].isPath = true;
                }

                _sidePaths.Add(newPath);

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

    private void OrientCornerAccordingToDirection(ref GameObject go, RoomData.DIRECTION direction)
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

    private void OrientTshapeAccordingToDirection(ref GameObject go, RoomData.DIRECTION direction)
    {
        go.transform.rotation = Quaternion.Euler(0, 0, 0);
        switch (direction)
        {
            case RoomData.DIRECTION.WEST:
                go.transform.rotation = Quaternion.Euler(0, 270, 0);
                break;
            case RoomData.DIRECTION.NORTH:
                go.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case RoomData.DIRECTION.EAST:
                go.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case RoomData.DIRECTION.SOUTH:
                go.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
        }
    }

    public void ColorAllWhite()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (_grid[x, y].isPath)
                {
                    foreach (var mr in _grid[x, y].instantiatedGameObject.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.material.color = Color.white;
                    }
                }
            }
        }
    }

    public void ColorAllPath()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var tile = _grid[x, y].instantiatedGameObject;

                if (_grid[x, y].isAvailable == false)
                {
                    foreach (var mr in tile.GetComponentsInChildren<MeshRenderer>())
                    {
                        mr.material.color = Color.red;
                    }
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
                foreach (var coordinates in _mainPath)
                {
                    if (x == coordinates.x && y == coordinates.y)
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

    public void HideNonPath()
    {
        _showGrid = !_showGrid;

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (_grid[x, y].isPath == false)
                {
                    _grid[x, y].instantiatedGameObject.SetActive(_showGrid);
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
                _grid[x, y].instantiatedGameObject = tile;

                if (_grid[x, y].isPath == false)
                    tile.SetActive(_showGrid);

                OrientCornerAccordingToDirection(ref tile, _grid[x, y].blocDirection);
                if (_grid[x, y].isCornerInverted)
                {
                    Vector3 rotation = tile.transform.rotation.eulerAngles;
                    rotation.y += 90;
                    tile.transform.rotation = Quaternion.Euler(rotation);
                }

                if (_grid[x, y].isNodeStart)
                {
                    OrientTshapeAccordingToDirection(ref tile, _grid[x, y].blocDirection);
                }
            }
        }
    }
}