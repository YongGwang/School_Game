using UnityEngine;
using System;

/// <summary>
/// via. https://www.youtube.com/watch?v=6OT43pvUyfY
/// </summary>
/// /// =======================================================
/// Author : 2020/02/15(Sa)
/// History Log :
///		2020/02/15(Sa) Initial
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private Sound[] sounds = null;

    private void Awake()
    {
        #region Singleton
        if (Instance != null)   Destroy(Instance);
        Instance = this;
        #endregion

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.loop = s.loop;
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void Play(string name)
    {
        FindSound(name)?.source.Play();
    }

    public void Stop(string name)
    {
        FindSound(name)?.source.Stop();
    }

    public bool CheckIsPlaying(string name)
    {
        var s = FindSound(name);
        if (s == null)
        {
            return false;
        }
        else
        {
            return s.source.isPlaying;
        }
    }

    private Sound FindSound(String name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("No sound be found.");
            return null;
        }
        else
        {
            return s;
        }
    }
}