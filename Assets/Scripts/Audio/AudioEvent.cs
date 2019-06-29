using UnityEngine;
using Random = UnityEngine.Random;

public interface IAudioEvent
{
	AudioEventKey Key { get; }
	AudioClip Clip { get; }
	float Volume { get; }
	float Pitch { get; }
	bool ShouldLoop { get; }
}

[CreateAssetMenu(menuName="Audio/Event")]
public class AudioEvent : ScriptableObject, IAudioEvent
{
	[SerializeField] private AudioEventKey key;
	[SerializeField] private AudioClip[] clips;
	[SerializeField] [Range(0, 1f)] private float volume = 1f;
	[SerializeField] [Range(0.5f, 2f)] private float pitch = 1f;
	[SerializeField] private bool shouldLoop;
	
	public AudioEventKey Key => key;
	public AudioClip Clip => clips[Random.Range(0, clips.Length)];
	public float Volume => volume;
	public float Pitch => pitch;
	public bool ShouldLoop => shouldLoop;
}