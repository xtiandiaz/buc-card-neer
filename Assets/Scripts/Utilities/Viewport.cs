using UnityEngine;

public class Viewport
{
    public Viewport(float width, float height)
    {
        Size = new Vector2(width, height);
    }
    
    public Vector2 Size { get; }
}