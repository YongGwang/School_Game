using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// 
/// </summary>
/// =======================================================
/// Author : 2020/02/02 Sa
/// History Log :
///		2020/02/02(Sa) Initial
///		2020/02/13(Sa) Add input cool down
///		2020/02/21(Sa) 1.For generalization, change name from
///		                 TitleSceneManger -> CutSceneManager
///		               2.LoadSceneAsync
///		2020/02/22(Sa) Jump time setted by timeline duration
public class CutSceneManager : MonoBehaviour
{
    public bool IsPlaying { get; private set; }

    [Header("ManuallySituation")]
    [SerializeField] private bool assignNextSceneIndexManually = false;
    [SerializeField] private int index = 0;

    [Space]
    [SerializeField] private float inputInterval = 0.5f;
    [Tooltip("何回の入力でTimeline全部早送りきるか")]
    [SerializeField] private int jumpTime = 5;
    [SerializeField] private PlayableDirector director = null;
    [SerializeField] private AudioSource zrSoundEffect = null;

    private bool canInput = false;
    private float timer = 0.0f;
    private AsyncOperation async = null;

    private void Awake()
    {
        Debug.Assert(director != null && zrSoundEffect != null);
        IsPlaying = false;
        canInput = false;
        timer = 0.0f;
        zrSoundEffect = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if(!assignNextSceneIndexManually)
        {
            async = Utility.MoveNextSceneAsync();
        }
        else
        {
            async = Utility.MoveSceneAsync(index);
        }
    }

    private void Update()
    {
        if (!canInput)
        {
            if (timer >= inputInterval)
            {
                canInput = true;
                timer = 0.0f;
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        else
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        #region TimelineProcess
        if (JoyConInputManager.GetJoyConAnykeyDown() || Input.anyKeyDown)
        {
            zrSoundEffect.Play();
            canInput = false;
            if (!IsPlaying)
            {
                IsPlaying = true;
                director.Play();
            }
            else
            {
                if (director.time == director.playableAsset.duration)
                {
                    async.allowSceneActivation = true;
                    Resources.UnloadUnusedAssets();
                    System.GC.Collect();
                }
                else
                {
                    var nextTime = director.time + director.playableAsset.duration / jumpTime;
                    if (nextTime > director.playableAsset.duration)
                    {
                        director.time = director.playableAsset.duration;
                    }
                    else
                    {
                        director.time += director.playableAsset.duration / jumpTime;
                    }
                }
            }
        }
        #endregion
    }

	public void SetInputState(bool IsPlaying)
	{
		canInput = IsPlaying;
	}
}