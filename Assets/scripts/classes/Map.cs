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

    public void RandomFill(string seed, int fillPercent)
    {
        System.Random pseudoRandom = new System.Random(seed.GetHashCode());

        for (int x = 0; x < Size.Width; x++)
        {
            for (int y = 0; y < Size.Height; y++)
            {
                if (x == 0 || x == Size.Width - 1 || y == 0 || y == Size.Height - 1)
                {
                    Tiles[x, y] = 1;
                }
                else
                {
                    Tiles[x, y] = (pseudoRandom.Next(0, 100) < fillPercent) ? 1 : 0;
                }
            }
        }
    }

    public void Smooth()
    {
        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }
    }

    private void SmoothMap()
    {
        for (int x = 0; x < Size.Width; x++)
        {
            for (int y = 0; y < Size.Height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(new TileCoordinate(x, y));

                if (neighbourWallTiles > 4)
                {
                    Tiles[x, y] = 1;
                }
                else if (neighbourWallTiles < 4)
                {
                    Tiles[x, y] = 0;
                }
            }
        }
    }

    private int GetSurroundingWallCount(TileCoordinate coord)
    {
        int wallCount = 0;

        for (int neighbourX = coord.X - 1; neighbourX <= coord.X + 1; neighbourX++)
        {
            for (int neighbourY = coord.Y - 1; neighbourY <= coord.Y + 1; neighbourY++)
            {
                if (IsInRange(neighbourX, neighbourY))
                {
                    if (neighbourX != coord.X || neighbourY != coord.Y)
                    {
                        wallCount += Tiles[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }
}
