using UnityEngine;

public class Vertex
{
    public Vector3 Position;
    public int Index = -1;

    public Vertex(Vector3 position)
    {
        this.Position = position;
    }
}

public class ControlVertex : Vertex
{
    public bool IsActive;
    public Vertex Above;
    public Vertex Right;


    public ControlVertex(Vector3 position, bool isActive, float squareSize) : base(position)
    {
        IsActive = isActive;
        Above = new Vertex(position + Vector3.forward * squareSize / 2f);
        Right = new Vertex(position + Vector3.right * squareSize / 2f);
    }
}