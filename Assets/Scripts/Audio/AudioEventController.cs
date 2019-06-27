using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioEventController : MonoBehaviour
{
	private AudioSource source;

    //Properties
    public String Name;

    public AudioClip Clip;

    public AudioMixerGroup Output;

    public bool Loop = false;

    //[Range(0, 1)] //0=2D; 1=3D
    private readonly float SpatialBlend = 0f;
    
    [Range(0,1)]
    public float Volume = 1f;

    [Range(0.5f, 2)]
    public float Pitch = 1f;



    //Create an audio source to be affected by the script
    void awake()
	{
		//Fetch the AudioSource from the GameObject
		source = GetComponent<AudioSource>();   
    }
		

	//Play an audio clip using the source
	void Start()
	{
        //Create an AudioSource component to the GameObject
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();

        //Call the Play function
        Play();
	}


    //Create AudioSource parameters
    public void SetSourceProperties(AudioClip clip, bool loop, float spatialBlend, float volume, float pitch)
    {
        source.clip = clip;
        source.loop = loop;
        source.spatialBlend = spatialBlend;
        source.volume = volume;
        source.pitch = pitch;
    }

    //Play function
    public void Play()
    {
        SetSourceProperties(Clip, Loop, SpatialBlend, Volume, Pitch);

        source.Play();

        Debug.Log(Name + " played");
    }
}
