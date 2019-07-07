using System;
using UniRx;
using UnityEngine;

public interface IAudioManager : IDisposable
{
    void Play(AudioEventKey withKey);
}

public class AudioManager : IAudioManager
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private readonly AudioRepository repository;
    private readonly AudioSourcePool sourcePool;

    private AudioManager(
        AudioRepository repository,
        AudioSourcePool sourcePool
        )
    {
        this.repository = repository;
        this.sourcePool = sourcePool;
    }

    public void Play(AudioEventKey withKey)
    {
        if (!repository.DoesContain(withKey))
        {
            Debug.LogWarning($"[AudioManager] The event {withKey} was not found in the repository.");
            return;
        }
        
        Play(repository[withKey]);
    }
    
    private void Play(IAudioEvent audioEvent)
    {
        var source = sourcePool.Spawn(audioEvent);
        
        disposables.Add(Observable.Timer(TimeSpan.FromSeconds(source.clip.length))
            .Subscribe(_ => sourcePool.Despawn(source)));
       
        source.Play();
    }

    public void Dispose()
    {
        disposables?.Dispose();
    }
}