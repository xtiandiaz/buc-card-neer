using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Audio/Manager")]
public class AudioManager : ScriptableObject, IInitializable
{
    private readonly Dictionary<AudioEventKey, AudioEvent> index = new Dictionary<AudioEventKey, AudioEvent>();

    [SerializeField] private AudioEvent[] events;
    
    public void Initialize()
    {
        foreach (var audioEvent in events)
        {
            index.Add(audioEvent.Key, audioEvent);
        }
    }
    
    public void PlayEvent(AudioEventKey withKey, AudioSource andSource)
    {
        index[withKey].Play(andSource);
    }
}