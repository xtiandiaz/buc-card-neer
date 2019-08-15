using System.Collections.Generic;
using UnityEngine;

public interface IAudioRepository
{
    void Index();
    bool DoesContain(AudioEventKey key);
    IAudioEvent GetEvent(AudioEventKey withKey);
    ICardAudioEventSwitch GetCardSwitch(AudioEventSwitchKey withKey);
}

[CreateAssetMenu(menuName = "Audio/Repository")]
public class AudioRepository : ScriptableObject, IAudioRepository
{
    private readonly Dictionary<AudioEventKey, IAudioEvent> eventIndex = new Dictionary<AudioEventKey, IAudioEvent>();

    private readonly Dictionary<AudioEventSwitchKey, CardAudioEventSwitch> cardSwitchIndex =
        new Dictionary<AudioEventSwitchKey, CardAudioEventSwitch>();

    [SerializeField] private AudioEvent[] events = default;
    [SerializeField] private CardAudioEventSwitch[] cardSwitches = default;

    public void Index()
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

    public IAudioEvent GetEvent(AudioEventKey withKey)
    {
        return eventIndex[withKey];
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