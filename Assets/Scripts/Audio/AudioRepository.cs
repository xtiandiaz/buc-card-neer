using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName = "Audio/Repository")]
public class AudioRepository : ScriptableObject, IInitializable
{
    private readonly Dictionary<AudioEventKey, IAudioEvent> eventIndex = new Dictionary<AudioEventKey, IAudioEvent>();

    private readonly Dictionary<AudioEventSwitchKey, CardAudioEventSwitch> cardSwitchIndex =
        new Dictionary<AudioEventSwitchKey, CardAudioEventSwitch>();

    [SerializeField] private AudioEvent[] events = default;
    [SerializeField] private CardAudioEventSwitch[] cardSwitches = default;

    public IAudioEvent this[AudioEventKey key] => eventIndex[key];

    public void Initialize()
    {
        if (eventIndex.Count == 0)
        {
            foreach (var audioEvent in events)
                eventIndex.Add(audioEvent.Key, audioEvent);
        }

        if (cardSwitchIndex.Count == 0)
        {
            foreach (var cardSwitch in cardSwitches)
                cardSwitchIndex.Add(cardSwitch.Key, cardSwitch);
        }
    }

    public ICardAudioEventSwitch GetCardSwitch(AudioEventSwitchKey withKey)
    {
        return cardSwitchIndex[withKey];
    }

    public bool DoesContain(AudioEventKey key)
    {
        return eventIndex.ContainsKey(key);
    }
}