using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName="Audio/Event")]
public class SimpleAudioEvent : AudioEvent
{
	public AudioClip[] clips;

	public RangedFloat volume;

	[MinMaxRange(0, 2)]
	public RangedFloat pitch;

	public override void Play(AudioSource bySource)
	{
		if (clips == null || clips.Length == 0) 
			return;

		bySource.clip = clips[Random.Range(0, clips.Length)];
		bySource.volume = Random.Range(volume.minValue, volume.maxValue);
		bySource.pitch = Random.Range(pitch.minValue, pitch.maxValue);
		bySource.Play();
	}
}