using System;
using UnityEngine;
using Zenject;

public class AppController : IInitializable, IDisposable
{
    public void Initialize()
    {
        Application.targetFrameRate = 50;
    }

    public void Dispose()
    {   
    }
}