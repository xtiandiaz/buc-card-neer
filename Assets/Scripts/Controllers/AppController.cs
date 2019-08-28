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

public interface IAppStatus
{
}

public interface IAppInfo
{
    int BuildNumber { get; }
}

public interface IAppController : IInitializable, IDisposable, IAppStatus
{
}

public class AppController : IAppController, IAppNavigator, IAppInfo
{
    private readonly IMenuFactory menuFactory;
    private readonly ZenjectSceneLoader sceneLoader;
    private readonly IStageModel defaultStage;
    private IDisposable sceneLoading;

    private AppController(
        ZenjectSceneLoader sceneLoader,
        Stage.Factory stageFactory,
        IStageModel defaultStage
        )
    {
        this.sceneLoader = sceneLoader;
        this.defaultStage = defaultStage;
    }
    
    public int BuildNumber => 9;

    public void Initialize()
    {
        Application.targetFrameRate = 60;
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
        GoToScene("Game", container => 
            {
                container.BindInstance(defaultStage).WhenInjectedInto<GameInstaller>();
            });
    }

    private void GoToScene(string withName, Action<DiContainer> andExtraBindings = null)
    {
        sceneLoading?.Dispose();
        sceneLoading = Observable.FromCoroutine(() => LoadScene("Loading", LoadSceneMode.Single))
            .ContinueWith(Observable.FromCoroutine(
                () => LoadScene(withName, TimeSpan.FromSeconds(0.5), LoadSceneMode.Single, andExtraBindings)))
            .Subscribe();
    }

    private IEnumerator LoadScene(
        string withName, 
        TimeSpan delay, 
        LoadSceneMode andMode, 
        Action<DiContainer> andExtraBindings = null)
    {
        yield return new WaitForSeconds((float) delay.TotalSeconds);

        yield return LoadScene(withName, andMode, andExtraBindings);
    }
    
    private IEnumerator LoadScene(string withName, LoadSceneMode mode, Action<DiContainer> andExtraBindings = null)
    {
        var asyncLoad = andExtraBindings != null 
            ? sceneLoader.LoadSceneAsync(withName, mode, andExtraBindings) 
            : sceneLoader.LoadSceneAsync(withName, mode);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}