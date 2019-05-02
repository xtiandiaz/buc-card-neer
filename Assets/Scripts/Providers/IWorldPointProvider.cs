using UnityEngine;

public interface IWorldPointProvider
{
    Vector3 GetWorldPoint(Vector2 fromScreenPoint);
}