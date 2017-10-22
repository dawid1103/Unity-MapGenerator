using System;
using System.Collections.Generic;

public class Room : IComparable<Room>
{
    public List<TileCoordinate> EdgeTiles;
    public bool IsAccessibleFromMainRoom;
    public bool IsMainRoom;

    private List<TileCoordinate> tiles;
    private List<Room> connectedRooms;
    private int roomSize;

    public int ConnectedRoomsCount
    {
        get
        {
            return connectedRooms.Count;
        }
    }

    public Room()
    {

    }

    public Room(List<TileCoordinate> tiles, Map map)
    {
        this.tiles = tiles;
        roomSize = tiles.Count;
        connectedRooms = new List<Room>();
        EdgeTiles = new List<TileCoordinate>();

        foreach (TileCoordinate tile in tiles)
        {
            for (int x = tile.X - 1; x < tile.X + 1; x++)
            {
                for (int y = tile.Y - 1; y < tile.Y + 1; y++)
                {
                    if (x == tile.X || y == tile.Y)
                    {
                        if (map.Tiles[x, y] == 1)
                        {
                            EdgeTiles.Add(tile);
                        }
                    }
                }
            }
        }
    }

    public void SetAccessibleFromMainRoom()
    {
        if (!IsAccessibleFromMainRoom)
        {
            IsAccessibleFromMainRoom = true;
            foreach (Room connectedRoom in connectedRooms)
            {
                connectedRoom.SetAccessibleFromMainRoom();
            }
        }
    }

    public bool IsConnected(Room room)
    {
        return connectedRooms.Contains(room);
    }

    public static void ConnectRooms(Room roomA, Room roomB)
    {
        if (roomA.IsAccessibleFromMainRoom)
        {
            roomB.SetAccessibleFromMainRoom();
        }
        else if (roomB.IsAccessibleFromMainRoom)
        {
            roomA.SetAccessibleFromMainRoom();
        }

        roomA.connectedRooms.Add(roomB);
        roomB.connectedRooms.Add(roomA);
    }

    public int CompareTo(Room roomToCompare)
    {
        return roomToCompare.roomSize.CompareTo(roomSize);
    }
}
