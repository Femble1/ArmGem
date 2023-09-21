using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
[CreateAssetMenu(fileName = "New Sound", menuName = "Audio")]
public class Sound : ScriptableObject
{
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    [HideInInspector]
    public AudioSource source;
}
