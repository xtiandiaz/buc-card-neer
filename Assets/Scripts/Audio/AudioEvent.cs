using UnityEngine;

public abstract class AudioEvent : ScriptableObject
{
	[SerializeField] private AudioEventKey key;

	public AudioEventKey Key => key;
	
	public abstract void Play(AudioSource bySource);
}