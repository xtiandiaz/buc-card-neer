using System;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static Vector3 GetVector(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector3.up;
            case Direction.Down:
                return Vector3.down;
            case Direction.Right:
                return Vector3.right;
            case Direction.Left:
                return Vector3.left;
            default:
                return Vector3.zero;       
        }
    }
    
    public static Direction GetOpposite(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Right:
                return Direction.Left;
            case Direction.Left:
                return Direction.Right;
            default:
                return Direction.None;       
        }
    }
    
    public static T PopRandomItem<T>(this IList<T> from)
    {
        if (from.Count == 0)
            return default;

        var randomIndex = UnityEngine.Random.Range(0, from.Count);
        var item = from[randomIndex];
        
        from.RemoveAt(randomIndex);

        return item;
    }
    
    public static T GetRandomItem<T>(this T[] from)
    {
        return from.Length == 0 ? default : from[UnityEngine.Random.Range(0, from.Length)];
    }
    
    /// <summary>
    /// Multiplies a timespan by an integer value
    /// </summary>
    public static TimeSpan Multiply(this TimeSpan multiplicand, int multiplier)
    {
        return TimeSpan.FromTicks(multiplicand.Ticks * multiplier);
    }

    /// <summary>
    /// Multiplies a timespan by a double value
    /// </summary>
    public static TimeSpan Multiply(this TimeSpan multiplicand, double multiplier)
    {
        return TimeSpan.FromTicks((long)(multiplicand.Ticks * multiplier));
    }
}
