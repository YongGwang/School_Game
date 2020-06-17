using UnityEngine;

/// <summary>
/// via. https://www.youtube.com/watch?v=6OT43pvUyfY
/// </summary>
/// /// =======================================================
/// Author : 2020/02/15(Sa)
/// History Log :
///		2020/02/15(Sa) Initial
[System.Serializable]
public class Sound
{
    public bool loop = false;
    public string name;
    public AudioClip clip;
    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;
    public float pitch = 1.0f;
    [HideInInspector]
    public AudioSource source;
}