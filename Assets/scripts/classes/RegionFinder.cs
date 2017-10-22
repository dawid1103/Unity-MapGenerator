using System.Collections.Generic;

public class RegionFinder
{
    private Map map;

    public RegionFinder(Map map)
    {
        this.map = map;
    }

    public List<List<TileCoordinate>> GetRegions(int tileType)
    {
        List<List<TileCoordinate>> regions = new List<List<TileCoordinate>>();
        bool[,] checkedTiles = new bool[map.Size.Width, map.Size.Height];

        for (int x = 0; x < map.Size.Width; x++)
        {
            for (int y = 0; y < map.Size.Height; y++)
            {
                if (checkedTiles[x, y] == false && map.Tiles[x, y] == tileType)
                {
                    List<TileCoordinate> region = GetRegionTiles(x, y);
                    regions.Add(region);

                    foreach (TileCoordinate tile in region)
                    {
                        checkedTiles[tile.X, tile.Y] = true;
                    }
                }
            }
        }

        return regions;
    }

    private List<TileCoordinate> GetRegionTiles(int startX, int startY)
    {
        List<TileCoordinate> tiles = new List<TileCoordinate>();
        bool[,] checkedTiles = new bool[map.Size.Width, map.Size.Height];
        int tileType = map.Tiles[startX, startY];

        Queue<TileCoordinate> queue = new Queue<TileCoordinate>();
        queue.Enqueue(new TileCoordinate(startX, startY));
        checkedTiles[startX, startY] = true;

        while (queue.Count > 0)
        {
            TileCoordinate tile = queue.Dequeue();
            tiles.Add(tile);

            for (int x = tile.X - 1; x <= tile.X + 1; x++)
            {
                for (int y = tile.Y - 1; y <= tile.Y + 1; y++)
                {
                    if (map.IsInRange(x, y) && (y == tile.Y || x == tile.X))
                    {
                        if (checkedTiles[x, y] == false && map.Tiles[x, y] == tileType)
                        {
                            checkedTiles[x, y] = true;
                            queue.Enqueue(new TileCoordinate(x, y));
                        }
                    }
                }
            }
        }

        return tiles;
    }
}
