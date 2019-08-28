using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkyView
{
    Vector3 LocalPosition { get; set; }
    Vector3 LocalScale { get; set; }
}

public class SkyView : MonoBehaviour, ISkyView
{
    [SerializeField] private Transform blueTransform = default;
    
    public Vector3 LocalPosition
    {
        get => transform.localPosition;
        set => transform.localPosition = value;
    }
    
    public Vector3 LocalScale
    {
        get => blueTransform.localScale;
        set => blueTransform.localScale = value;
    }
}
