using UnityEngine;
using Zenject;

public class AudioSourcePool : MonoMemoryPool<IAudioEvent, AudioSource>
{
    protected override void Reinitialize(IAudioEvent withEvent, AudioSource forSource)
    {
        forSource.clip = withEvent.Clip;
        forSource.volume = withEvent.Volume;
        forSource.pitch = withEvent.Pitch;
        forSource.loop = withEvent.ShouldLoop;
    }
}