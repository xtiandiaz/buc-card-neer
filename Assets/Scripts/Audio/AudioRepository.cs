using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Audio/Repository")]
public class AudioRepository : ScriptableObject, IInitializable
{
    private Dictionary<AudioEventKey, IAudioEvent> index;

    [SerializeField] private AudioEvent[] events;
    
    public IAudioEvent this[AudioEventKey key] => index[key];
    
    public void Initialize()
    {
        index = new Dictionary<AudioEventKey, IAudioEvent>();
        
        foreach (var audioEvent in events)
        {
            index.Add(audioEvent.Key, audioEvent);
        }
    }

    public bool DoesContain(AudioEventKey key)
    {
        return index.ContainsKey(key);
    }
}