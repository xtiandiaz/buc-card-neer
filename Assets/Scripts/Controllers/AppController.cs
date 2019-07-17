using System;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public interface IAppController : IInitializable, IDisposable
{
    void Reload();
}

public class AppController : IAppController
{
    private readonly ZenjectSceneLoader sceneLoader;
    private IDisposable sceneLoading;

    private AppController(ZenjectSceneLoader sceneLoader)
    {
        this.sceneLoader = sceneLoader;
    }
    
    public void Initialize()
    {
        Application.targetFrameRate = 60;
    }

    public void Reload()
    {
        sceneLoading?.Dispose();
        sceneLoading = sceneLoader.LoadSceneAsync("Game", LoadSceneMode.Single)
            .AsObservable()
            .Subscribe();
    }

    public void Dispose()
    {
        sceneLoading?.Dispose();
    }
}