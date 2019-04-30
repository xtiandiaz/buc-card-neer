using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Extensions
{
    private static System.Random rng = new System.Random();  

    public static void Shuffle<T>(this IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;  
            int k = rng.Next(n + 1);  
            T value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
    
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

        var randomIndex = Random.Range(0, from.Count);
        var item = from[randomIndex];
        
        from.RemoveAt(randomIndex);

        return item;
    }
    
    public static T GetRandomItem<T>(this T[] from)
    {
        return from.Length == 0 ? default : from[Random.Range(0, from.Length)];
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
