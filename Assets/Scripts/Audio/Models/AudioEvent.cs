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
	[SerializeField] private AudioEventKey key = default;
	[SerializeField] private AudioClip[] clips = default;
	[SerializeField] [Range(0, 1f)] private float volume = 1f;
	[SerializeField] [Range(0.5f, 2f)] private float pitch = 1f;
	[SerializeField] private bool shouldLoop = default;
	
	public AudioEventKey Key => key;
	public AudioClip Clip => clips.Length > 0 ? clips[Random.Range(0, clips.Length)] : null;
	public float Volume => volume;
	public float Pitch => pitch;
	public bool ShouldLoop => shouldLoop;
}
