using UnityEngine;

public static class GameStatics
{
    public const float CardWidth = 2.5f;
    public const float CardHeight = 3.5f;
    
    public static readonly float CardExtent = Mathf.Sqrt(Vector2.SqrMagnitude(new Vector2(CardWidth, CardHeight)));
    public static readonly float HalfCardExtent = CardExtent * 0.5f;
}