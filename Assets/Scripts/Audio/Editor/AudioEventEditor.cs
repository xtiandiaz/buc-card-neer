using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioEvent), true)]
public class AudioEventEditor : Editor
{
	[SerializeField] private AudioSource _previewer;

	public void OnEnable()
	{
		_previewer = EditorUtility.CreateGameObjectWithHideFlags(
			"Audio preview", 
			HideFlags.HideAndDontSave, 
			typeof(AudioSource)).GetComponent<AudioSource>();
	}

	public void OnDisable()
	{
		DestroyImmediate(_previewer.gameObject);
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
		if (GUILayout.Button("Preview"))
		{
			var audioEvent = (AudioEvent) target;
			
			_previewer.clip = audioEvent.Clip;
			_previewer.volume = audioEvent.Volume;
			_previewer.pitch = audioEvent.Pitch;	
			_previewer.loop = false;	
			
			_previewer.Play();
		}
		EditorGUI.EndDisabledGroup();
	}
}
