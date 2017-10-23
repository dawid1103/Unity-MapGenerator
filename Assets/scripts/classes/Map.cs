using System.Collections.Generic;

public class Map
{
    public int[,] Tiles;
    public Size Size;

    public Map(int width, int height)
    {
        Size = new Size(width, height);
        Tiles = new int[width, height];
    }

    public bool IsInRange(int x, int y)
    {
        return x >= 0 && x < Size.Width && y >= 0 && y < Size.Height;
    }

    public List<TileCoordinate> GetFreeTiles()
    {
        List<TileCoordinate> tiles = new List<TileCoordinate>();

        for (int x = 0; x < Size.Width; x++)
        {
            for (int y = 0; x < Size.Height; y++)
            {
                if (Tiles[x, y] == 1)
                {
                    tiles.Add(new TileCoordinate(x, y));
                }
            }
        }

        return tiles;
    }
}
