using System;
using System.Collections.Generic;
using UnityEngine;

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

    public List<Vector3> GetFreeTiles()
    {
        List<Vector3> tiles = new List<Vector3>();

        for (int x = 0; x < Size.Width; x++)
        {
            for (int y = 0; y < Size.Height; y++)
            {
                if (Tiles[x, y] == 0)
                {
                    tiles.Add(new Vector3(x, 0, y));
                }
            }
        }

        return tiles;
    }

    public string Hello()
    {
        return DateTime.Now.ToString();
    }
}
