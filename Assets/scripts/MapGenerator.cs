using UnityEngine;
using System;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public int MapWidth = 150;
    public int MapHeight = 150;
    public bool UseRandomSeed = true;
    public string Seed;

    [Range(0, 100)]
    public int FillPercent = 45;

    [Range(0, 200)]
    public int WallsThreshold = 50;

    [Range(0, 200)]
    public int RoomsThreshold = 50;

    public GameObject[] Items;

    private Map map;
    private RegionFinder regionFinder;

    private Transform boardHolder;
    private List<List<TileCoordinate>> freeRegions;

    void Start()
    {
        map = new Map(MapWidth, MapHeight);
        regionFinder = new RegionFinder(map);
        boardHolder = new GameObject("Board").transform;
        GenerateMap();

        freeRegions = regionFinder.GetRegions(0);
        LayoutObjectAtRandomSpace(Items, 20, 50);
    }

    private void GenerateMap()
    {
        if (UseRandomSeed)
        {
            Seed = DateTime.Now.ToString();
        }

        DateTime start = DateTime.Now;

        map.RandomFill(Seed, FillPercent);
        map.Smooth();
        ProcessMap();

        MeshGenerator meshGenerator = GetComponent<MeshGenerator>();
        meshGenerator.GenerateMesh(map, 1f);

        Debug.Log(string.Format("Map generation total time: {0}ms",
                                (DateTime.Now - start).Milliseconds));

    }

    private void ProcessMap()
    {
        List<List<TileCoordinate>> wallRegions = regionFinder.GetRegions(1);

        foreach (List<TileCoordinate> wallRegion in wallRegions)
        {
            if (wallRegion.Count < WallsThreshold)
            {
                foreach (TileCoordinate tile in wallRegion)
                {
                    map.Tiles[tile.X, tile.Y] = 0;
                }
            }
        }

        List<List<TileCoordinate>> roomRegions = regionFinder.GetRegions(0);
        List<Room> rooms = new List<Room>();

        foreach (List<TileCoordinate> roomRegion in roomRegions)
        {
            if (roomRegion.Count < RoomsThreshold)
            {
                foreach (TileCoordinate tile in roomRegion)
                {
                    map.Tiles[tile.X, tile.Y] = 1;
                }
            }
            else
            {
                rooms.Add(new Room(roomRegion, map));
            }
        }

        rooms.Sort();
        rooms[0].IsMainRoom = true;
        rooms[0].IsAccessibleFromMainRoom = true;

        ConnectClosestRooms(rooms);
    }

    private void ConnectClosestRooms(List<Room> rooms, bool forceAccessibilityFromMainRoom = false)
    {
        List<Room> roomListA = new List<Room>();
        List<Room> roomListB = new List<Room>();

        if (forceAccessibilityFromMainRoom)
        {
            foreach (Room room in rooms)
            {
                if (room.IsAccessibleFromMainRoom)
                {
                    roomListB.Add(room);
                }
                else
                {
                    roomListA.Add(room);
                }
            }
        }
        else
        {
            roomListA = roomListB = rooms;
        }

        int bestDistance = 0;
        TileCoordinate bestTileA = new TileCoordinate();
        TileCoordinate bestTileB = new TileCoordinate();
        Room bestRoomA = new Room();
        Room bestRoomB = new Room();
        bool connectionFound = false;

        foreach (Room roomA in roomListA)
        {
            if (!forceAccessibilityFromMainRoom)
            {
                connectionFound = false;
                if (roomA.ConnectedRoomsCount > 0)
                {
                    continue;
                }
            }

            foreach (Room roomB in roomListB)
            {
                if (roomA == roomB && roomA.IsConnected(roomB))
                {
                    continue;
                }

                foreach (TileCoordinate tileA in roomA.EdgeTiles)
                {
                    foreach (TileCoordinate tileB in roomB.EdgeTiles)
                    {
                        int distanceBetweenRooms = (int)(Mathf.Pow(tileA.X - tileB.X, 2) + Mathf.Pow(tileA.Y - tileB.Y, 2));

                        if (distanceBetweenRooms < bestDistance || !connectionFound)
                        {
                            bestDistance = distanceBetweenRooms;
                            connectionFound = true;
                            bestTileA = tileA;
                            bestTileB = tileB;
                            bestRoomA = roomA;
                            bestRoomB = roomB;
                        }
                    }
                }
            }

            if (connectionFound && !forceAccessibilityFromMainRoom)
            {
                CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            }
        }

        if (connectionFound && forceAccessibilityFromMainRoom)
        {
            CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
            ConnectClosestRooms(rooms, true);
        }

        if (!forceAccessibilityFromMainRoom)
        {
            ConnectClosestRooms(rooms, true);
        }
    }

    private void CreatePassage(Room roomA, Room roomB, TileCoordinate tileA, TileCoordinate tileB)
    {
        Room.ConnectRooms(roomA, roomB);

        List<TileCoordinate> line = GetLine(tileA, tileB);
        foreach (TileCoordinate c in line)
        {
            DrawCircle(c, 2);
        }
    }

    private void DrawCircle(TileCoordinate c, int r)
    {
        for (int x = -r; x <= r; x++)
        {
            for (int y = -r; y <= r; y++)
            {
                if (x * x + y * y <= r * r)
                {
                    int drawX = c.X + x;
                    int drawY = c.Y + y;

                    if (map.IsInRange(drawX, drawY))
                    {
                        map.Tiles[drawX, drawY] = 0;
                    }
                }
            }
        }
    }

    private List<TileCoordinate> GetLine(TileCoordinate from, TileCoordinate to)
    {
        List<TileCoordinate> line = new List<TileCoordinate>();

        int x = from.X;
        int y = from.Y;

        int dx = to.X - from.X;
        int dy = to.Y - from.Y;

        bool inverted = false;
        int step = Math.Sign(dx);
        int gradientStep = Math.Sign(dy);

        int longest = Mathf.Abs(dx);
        int shortest = Mathf.Abs(dy);

        if (longest < shortest)
        {
            inverted = true;
            longest = Mathf.Abs(dy);
            shortest = Mathf.Abs(dx);

            step = Math.Sign(dy);
            gradientStep = Math.Sign(dx);
        }

        int gradientAccumulation = longest / 2;
        for (int i = 0; i < longest; i++)
        {
            line.Add(new TileCoordinate(x, y));

            if (inverted)
            {
                y += step;
            }
            else
            {
                x += step;
            }

            gradientAccumulation += shortest;

            if (gradientAccumulation >= longest)
            {
                if (inverted)
                {
                    x += gradientStep;
                }
                else
                {
                    y += gradientStep;
                }
                gradientAccumulation -= longest;
            }
        }

        return line;
    }

    private Vector3 GetWorldPosition(TileCoordinate tile)
    {
        return new Vector3(-map.Size.Width / 2 + .5f + tile.X, 0, -map.Size.Height / 2 + .5f + tile.Y);
    }

    private Vector3 GetRandomFreePosition()
    {
        int randomRegionIndex = UnityEngine.Random.Range(0, freeRegions.Count - 1);
        int randomTileIntex = UnityEngine.Random.Range(0, freeRegions[randomRegionIndex].Count - 1);

        TileCoordinate tile = freeRegions[randomRegionIndex][randomTileIntex];

        Vector3 randomPosition = GetWorldPosition(tile);
        freeRegions[randomRegionIndex].RemoveAt(randomTileIntex);
        return randomPosition;
    }

    private void LayoutObjectAtRandomSpace(GameObject[] tileCollection, int minimum, int maximum)
    {
        int objectCount = UnityEngine.Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = GetRandomFreePosition();
            GameObject tile = tileCollection[UnityEngine.Random.Range(0, tileCollection.Length)];
            CreateObject(randomPosition, tile);
        }
    }

    private void CreateObject(Vector3 position, GameObject prefab)
    {
        GameObject cube = Instantiate(prefab);
        cube.transform.SetParent(boardHolder);
        cube.transform.position = position;
    }
}