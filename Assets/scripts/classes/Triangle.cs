public struct Triangle
{
    public int VertexIndexA;
    public int VertexIndexB;
    public int VertexIndexC;

    public int[] Vertices;

    public Triangle(int a, int b, int c)
    {
        VertexIndexA = a;
        VertexIndexB = b;
        VertexIndexC = c;

        Vertices = new int[] { a, b, c };
    }

    /// <summary>
    /// Check if triangle contains vertex
    /// </summary>
    /// <returns><c>true</c>, if vertex was containsed, <c>false</c> otherwise.</returns>
    /// <param name="vertexIndex">Vertex index to check.</param>
    public bool ContainsVertex(int vertexIndex)
    {
        return VertexIndexA == vertexIndex || VertexIndexB == vertexIndex || VertexIndexC == vertexIndex;
    }
}
