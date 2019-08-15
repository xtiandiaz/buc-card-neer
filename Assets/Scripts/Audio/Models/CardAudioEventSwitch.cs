using System;
using System.Linq;
using UnityEngine;

public enum AudioEventSwitchKey
{
    None,
    
    CardReveal = 1,
    CardStash = 2,
    CardBoard = 3,
    CardConfront = 4,
    CardClash = 5,
}

public interface IAudioEventSwitch<in T> where T : struct
{ 
    AudioEventSwitchKey Key { get; }
    
    IAudioEvent GetEvent(T forType);
}

public interface ICardAudioEventSwitch : IAudioEventSwitch<CardType>
{
}

[CreateAssetMenu(menuName = "Audio/Card Switch")]
public class CardAudioEventSwitch : ScriptableObject, ICardAudioEventSwitch
{
    [SerializeField] private AudioEventSwitchKey key = default;
    [SerializeField] private Alternative[] alternatives = default;

    public AudioEventSwitchKey Key => key;

    public IAudioEvent GetEvent(CardType forType)
    {
        return alternatives.FirstOrDefault(alt => (alt.cardType & forType) != 0).audioEvent;
    }
    
    [Serializable]
    public struct Alternative
    {
        public CardType cardType;
        public AudioEvent audioEvent;
    }
}