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
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private IDisposable sceneLoading;
    
    public void Initialize()
    {
        Application.targetFrameRate = 50;
    }

    public void Reload()
    {
        sceneLoading?.Dispose();
        sceneLoading = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single)
            .AsObservable()
            .Subscribe();
    }

    public void Dispose()
    {
        disposables.Dispose();
        sceneLoading?.Dispose();
    }
}