using UnityEngine;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour
{
    public MeshFilter WallMesh;
    public MeshFilter CaveMesh;

    private SquareGrid grid;
    private List<Vector3> allVertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private Dictionary<int, List<Triangle>> trianglesToVertex = new Dictionary<int, List<Triangle>>();
    private List<List<int>> outlines = new List<List<int>>();
    private HashSet<int> checkedVertices = new HashSet<int>();

    public void GenerateMesh(Map map, float squareSize)
    {
        allVertices.Clear();
        triangles.Clear();
        outlines.Clear();
        trianglesToVertex.Clear();
        checkedVertices.Clear();

        grid = new SquareGrid(map, squareSize);

        for (int x = 0; x < grid.Squares.GetLength(0); x++)
        {
            for (int y = 0; y < grid.Squares.GetLength(1); y++)
            {
                TriangualateSquare(grid.Squares[x, y]);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = allVertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.name = "CaveMesh";
        CaveMesh.mesh = mesh;

        CreateWallMesh();
    }

    private void CreateWallMesh()
    {
        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        float wallHeight = 4;

        foreach (List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count - 1; i++)
            {
                int startIndex = wallVertices.Count;

                wallVertices.Add(allVertices[outline[i]]);
                wallVertices.Add(allVertices[outline[i + 1]]);
                wallVertices.Add(allVertices[outline[i]] - Vector3.up * wallHeight);
                wallVertices.Add(allVertices[outline[i + 1]] - Vector3.up * wallHeight);

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = wallVertices.ToArray();
        mesh.triangles = wallTriangles.ToArray();

        WallMesh.mesh = mesh;

        MeshCollider meshCollider = WallMesh.gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    private void TriangualateSquare(Square square)
    {
        switch (square.Configuration)
        {
            case 0:
                break;

            case 1:
                MeshFromPoints(square.CenterLeft, square.CenterBottom, square.BottomLeft);
                break;
            case 2:
                MeshFromPoints(square.BottomRight, square.CenterBottom, square.CenterRight);
                break;
            case 4:
                MeshFromPoints(square.TopRight, square.CenterRight, square.CenterTop);
                break;
            case 8:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterLeft);
                break;
            case 3:
                MeshFromPoints(square.CenterRight, square.BottomRight, square.BottomLeft, square.CenterLeft);
                break;
            case 6:
                MeshFromPoints(square.CenterTop, square.TopRight, square.BottomRight, square.CenterBottom);
                break;
            case 9:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterBottom, square.BottomLeft);
                break;
            case 12:
                MeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterLeft);
                break;
            case 5:
                MeshFromPoints(square.CenterTop, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft, square.CenterLeft);
                break;
            case 10:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight, square.CenterBottom, square.CenterLeft);
                break;
            case 7:
                MeshFromPoints(square.CenterTop, square.TopRight, square.BottomRight, square.BottomLeft, square.CenterLeft);
                break;
            case 11:
                MeshFromPoints(square.TopLeft, square.CenterTop, square.CenterRight, square.BottomRight, square.BottomLeft);
                break;
            case 13:
                MeshFromPoints(square.TopLeft, square.TopRight, square.CenterRight, square.CenterBottom, square.BottomLeft);
                break;
            case 14:
                MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.CenterBottom, square.CenterLeft);
                break;
            case 15:
                MeshFromPoints(square.TopLeft, square.TopRight, square.BottomRight, square.BottomLeft);

                //In this case we are sure that the square can't have outline edge, 
                //because all walls are active with means all neighbors are walls.

                checkedVertices.Add(square.TopLeft.Index);
                checkedVertices.Add(square.TopRight.Index);
                checkedVertices.Add(square.BottomRight.Index);
                checkedVertices.Add(square.BottomLeft.Index);
                break;

        }
    }

    private void MeshFromPoints(params Vertex[] vertices)
    {
        AssignVertices(vertices);

        if (vertices.Length >= 3)
            CreateTriangle(vertices[0], vertices[1], vertices[2]);
        if (vertices.Length >= 4)
            CreateTriangle(vertices[0], vertices[2], vertices[3]);
        if (vertices.Length >= 5)
            CreateTriangle(vertices[0], vertices[3], vertices[4]);
        if (vertices.Length >= 6)
            CreateTriangle(vertices[0], vertices[4], vertices[5]);
    }

    private void AssignVertices(Vertex[] vertices)
    {
        foreach (Vertex vertex in vertices)
        {
            if (vertex.Index == -1)
            {
                vertex.Index = allVertices.Count;
                allVertices.Add(vertex.Position);
            }
        }
    }

    private void CreateTriangle(params Vertex[] vertices)
    {
        Triangle triangle = new Triangle(vertices[0].Index, vertices[1].Index, vertices[2].Index);

        foreach (Vertex vertex in vertices)
        {
            triangles.Add(vertex.Index);
            AddTriangleToDict(vertex.Index, triangle);
        }
    }

    private void AddTriangleToDict(int vertexIndex, Triangle triangle)
    {
        if (trianglesToVertex.ContainsKey(vertexIndex))
            trianglesToVertex[vertexIndex].Add(triangle);
        else
            trianglesToVertex.Add(vertexIndex, new List<Triangle>() { triangle });
    }

    private bool IsOutlineEdge(int vertexIndexA, int vertexIndexB)
    {
        List<Triangle> trianglesContainingVertexA = trianglesToVertex[vertexIndexA];
        int sharedTrianglesCount = 0;

        foreach (Triangle triangle in trianglesContainingVertexA)
        {
            if (triangle.ContainsVertex(vertexIndexB))
            {
                sharedTrianglesCount++;
                if (sharedTrianglesCount > 1) break;
            }
        }

        return sharedTrianglesCount == 1;
    }

    private int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = trianglesToVertex[vertexIndex];

        foreach (Triangle triangle in trianglesContainingVertex)
        {
            for (int i = 0; i < triangle.Vertices.Length; i++)
            {
                int vertex = triangle.Vertices[i];

                if (vertex != vertexIndex && !checkedVertices.Contains(vertex))
                {
                    if (IsOutlineEdge(vertexIndex, vertex))
                        return vertex;
                }
            }
        }

        return -1;
    }

    private void CalculateMeshOutlines()
    {
        for (int vertexIndex = 0; vertexIndex < allVertices.Count; vertexIndex++)
        {
            if (!checkedVertices.Contains(vertexIndex))
            {
                int connectedVertexIndex = GetConnectedOutlineVertex(vertexIndex);

                if (connectedVertexIndex != -1)
                {
                    checkedVertices.Add(vertexIndex);

                    List<int> outline = new List<int>() { vertexIndex };
                    outlines.Add(outline);

                    FollowOutline(connectedVertexIndex, outlines.Count - 1);

                    outlines[outlines.Count - 1].Add(vertexIndex);
                }
            }
        }
    }

    private void FollowOutline(int vertexIndex, int outlineIndex)
    {
        outlines[outlineIndex].Add(vertexIndex);
        checkedVertices.Add(vertexIndex);
        int nextVertexIndex = GetConnectedOutlineVertex(vertexIndex);

        if (nextVertexIndex != -1)
        {
            FollowOutline(nextVertexIndex, outlineIndex);
        }
    }
}
