using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public interface IAppNavigator
{
    void GoToMainMenu();
    void GoToGame();
}

public interface IAppInfo
{
    int BuildNumber { get; }
}

public interface IAppController : IInitializable, IDisposable
{
    void Reload(); // TODO Refactor
}

public class AppController : IAppController, IAppNavigator, IAppInfo
{
    private readonly IMenuFactory menuFactory;
    private IDisposable sceneLoading;

    public int BuildNumber => 8;

    public void Initialize()
    {
        Application.targetFrameRate = 60;
    }

    public void Reload()
    {
        GoToScene("Game");
    }

    public void Dispose()
    {
        sceneLoading?.Dispose();
    }

    public void GoToMainMenu()
    {
        GoToScene("MainMenu");
    }

    public void GoToGame()
    {
        GoToScene("Game");
    }

    private void GoToScene(string name)
    {
        sceneLoading?.Dispose();
        sceneLoading = Observable.FromCoroutine(() => LoadScene("Loading", LoadSceneMode.Additive))
            .ContinueWith(Observable.FromCoroutine(
                () => LoadScene(name, TimeSpan.FromSeconds(0.5), LoadSceneMode.Single)))
            .Subscribe();
    }

    private IEnumerator LoadScene(string name, TimeSpan withDelay, LoadSceneMode andMode)
    {
        yield return new WaitForSeconds((float) withDelay.TotalSeconds);

        yield return LoadScene(name, andMode);
    }
    
    private IEnumerator LoadScene(string name, LoadSceneMode withMode)
    {
        var asyncLoad = SceneManager.LoadSceneAsync(name, withMode);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}