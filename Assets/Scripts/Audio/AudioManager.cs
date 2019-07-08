using System;
using UniRx;
using UnityEngine;

public interface IAudioManager : IDisposable
{
    void Play(AudioEventSwitchKey fromSwitchWithKey, CardType forCardType);
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

    public void Play(AudioEventSwitchKey fromSwitchWithKey, CardType forCardType)
    {
        var audioEvent = repository.GetCardSwitch(fromSwitchWithKey).GetEvent(forCardType);

        if (audioEvent == null)
        {
            Debug.LogWarning(
                $"[AudioManager] The Event Switch {fromSwitchWithKey} was not found in the repository or didn't return the intended event.");
            return;
        }
        
        Play(audioEvent);
    }

    public void Play(AudioEventKey withKey)
    {
        if (!repository.DoesContain(withKey))
        {
            Debug.LogWarning($"[AudioManager] The Event {withKey} was not found in the repository.");
            return;
        }  
        
        Play(repository[withKey]);
    }

    private void Play(IAudioEvent audioEvent)
    {
        if (audioEvent == null)
        {
            Debug.LogError("[AudioManager] Attempted to play a null Audio Event.");
            return;
        }
        
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