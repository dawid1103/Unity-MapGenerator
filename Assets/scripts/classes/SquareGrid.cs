using UnityEngine;

public class SquareGrid
{
    public Square[,] Squares;

    public SquareGrid(Map map, float squareSize)
    {
        int vertexCountX = map.Size.Width;
        int vertexCountY = map.Size.Height;

        float mapWidth = vertexCountX * squareSize;
        float mapHeight = vertexCountY * squareSize;

        ControlVertex[,] controlVertex = new ControlVertex[vertexCountX, vertexCountY];

        for (int x = 0; x < vertexCountX; x++)
        {
            for (int y = 0; y < vertexCountY; y++)
            {
                Vector3 position = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0, -mapHeight / 2 + y + squareSize / 2);
                controlVertex[x, y] = new ControlVertex(position, map.Tiles[x, y] == 1 , squareSize);
            }
        }

        Squares = new Square[vertexCountX - 1, vertexCountY - 1];

        for (int x = 0; x < vertexCountX - 1; x++)
        {
            for (int y = 0; y < vertexCountY - 1; y++)
            {
                Squares[x, y] = new Square(controlVertex[x, y + 1], controlVertex[x + 1, y + 1], controlVertex[x, y], controlVertex[x + 1, y]);
            }
        }
    }
}