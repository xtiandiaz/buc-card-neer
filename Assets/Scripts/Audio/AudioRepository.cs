using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Audio/Repository")]
public class AudioRepository : ScriptableObject, IInitializable
{
    private readonly Dictionary<AudioEventKey, IAudioEvent> index = new Dictionary<AudioEventKey, IAudioEvent>();

    [SerializeField] private AudioEvent[] events;
    
    public void Initialize()
    {
        foreach (var audioEvent in events)
        {
            index.Add(audioEvent.Key, audioEvent);
        }
    }

    public IAudioEvent this[AudioEventKey key] => index[key];
}