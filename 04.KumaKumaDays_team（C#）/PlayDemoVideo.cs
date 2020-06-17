using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
/// Author : Baku
public class PlayDemoVideo : MonoBehaviour
{
	[SerializeField] private RawImage rawImage = null;
	[SerializeField] private GameObject titleManager = null;
	[SerializeField] private float PlayTime = 0.0f;
	[SerializeField] private float WaitTime = 10.0f;
    [SerializeField] private CutSceneManager CutSceneManager = null;
	private VideoPlayer Demovideo;
	private bool ShouldPlay = false;
	private bool IsPlaying = false;
	private float RGB = 1.0f;
	private float Timer = 0.0f;
	private void Awake()
	{
        Debug.Assert(CutSceneManager != null);
		Demovideo = this.GetComponent<VideoPlayer>();
		Demovideo.Stop();
		rawImage.enabled = false;
	}

	void Update()
	{
		if (ShouldPlay && !CutSceneManager.IsPlaying)
		{
			Play();
			return;
		}

		Playing();
    }

	private void Play()
	{
		Demovideo.Play();
		rawImage.enabled = true;
		RGB -= Time.deltaTime;
		if (rawImage.color.a <= 1)
		{
			rawImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f - RGB);
		}
		else
		{
			RGB = 1.0f;
			rawImage.color = new Color(RGB, RGB, RGB, RGB);
			IsPlaying = true;
			ShouldPlay = false;
		}
		
	} 
	
	private void Playing()
	{
		if (IsPlaying)
		{
			titleManager.GetComponent<CutSceneManager>().SetInputState(false);
			Timer = 0.0f;
			if (JoyConInputManager.GetJoyConAnykeyDown() || Input.anyKeyDown)
			{
				IsPlaying = false;
				titleManager.GetComponent<CutSceneManager>().SetInputState(true);
			}
			if (PlayTime != 0.0f)
			{
				if (Demovideo.time > PlayTime)
				{
					IsPlaying = false;
					titleManager.GetComponent<CutSceneManager>().SetInputState(true);
				}
			}
			else
			{
				if (Demovideo.time >= Demovideo.length)
				{
					IsPlaying = false;
					titleManager.GetComponent<CutSceneManager>().SetInputState(true);
				}
			}
		}
		else
		{
			Timer += Time.deltaTime;
			if (Timer >= WaitTime) ShouldPlay = true;
			Stop();
		}
	}
	private void Stop()
	{
		RGB -= Time.deltaTime;
		if (rawImage.color.r > 0) rawImage.color = new Color(RGB, RGB, RGB);
		else
		{
			Demovideo.time = 0.0f;
			Demovideo.Stop();
			RGB = 1.0f;
			rawImage.color = new Color(RGB, RGB, RGB);
			rawImage.enabled = false;
		}
	}
}
