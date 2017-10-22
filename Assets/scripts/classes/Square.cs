public class Square
{
    public ControlVertex TopLeft, TopRight, BottomLeft, BottomRight;
    public Vertex CenterTop, CenterRight, CenterBottom, CenterLeft;
    public int Configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:Square"/> class.
    /// 1 -- 2
    /// |    |
    /// |    |
    /// 3 -- 4
    /// </summary>
    /// <param name="topLeft">Top left (1).</param>
    /// <param name="topRight">Top right (2).</param>
    /// <param name="bottomLeft">Bottom left (3).</param>
    /// <param name="bottomRight">Bottom right (4).</param>
    public Square(ControlVertex topLeft, ControlVertex topRight, ControlVertex bottomLeft, ControlVertex bottomRight)
    {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomLeft = bottomLeft;
        BottomRight = bottomRight;

        CenterTop = topLeft.Right;
        CenterRight = bottomRight.Above;
        CenterBottom = bottomLeft.Right;
        CenterLeft = bottomLeft.Above;

        if (TopLeft.IsActive)
            Configuration += 8;
        if (TopRight.IsActive)
            Configuration += 4;
        if (bottomRight.IsActive)
            Configuration += 2;
        if (BottomLeft.IsActive)
            Configuration += 1;
    }
}
