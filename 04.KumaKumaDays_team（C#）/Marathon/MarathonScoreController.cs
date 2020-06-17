using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Marathon game score calcutor
/// </summary>
/// =======================================================
/// Author : Sa
/// History Log :
///		2020/02/12(Sa) Initial.
///		2020/02/25(Sa) Change score calculation.
public class MarathonScoreController : MonoBehaviour
{
    public static MarathonScoreController Instance = null;
    public static int PlayerScore { get; private set; }
    public static int RivalScore { get; private set; }

    public bool IsFinishedResultTimeline { get; private set; }

    // In second
    [Header("ValueSetting")]
    [SerializeField] private float timeBenchmark = 120.0f;
    [SerializeField] private int scoreBenchmark = 100;
    [SerializeField] private float rivalCostTime = 160.0f;

    [Header("UI")]
    [SerializeField] private float numImageInterval = 80.0f;
    [SerializeField] private Transform minNumRoot = null;
    [SerializeField] private Color minColor = Color.white;
    [SerializeField] private Transform secNumRoot = null;
    [SerializeField] private Color secColor = Color.white;
    [SerializeField] private Transform scoreNumRoot = null;
    [SerializeField] private int scoreDigits = 4;
    [SerializeField] private Color scoreColor = Color.white;
    [SerializeField] private PlayableDirector resultTimeline = null;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;

        PlayerScore = 0;
        RivalScore = 0;
        IsFinishedResultTimeline = false;
    }

    private void Start()
    {
        MarathonGoalController.Instance.OnPlayerReachGoal += UpdateCharacterScore;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Update()
    {
        if (resultTimeline.time == resultTimeline.playableAsset.duration)
        {
            IsFinishedResultTimeline = true;
        }
    }

    private int CalcScore(float time)
    {
        var score = scoreBenchmark + (int)(timeBenchmark - time) * 1;
        return score < 0 ? 0 : score;
    }

    private void UpdateCharacterScore()
    {
        PlayerScore = CalcScore(MarathonGameTimerController.Instance.MarathonGameTimeInSec);
        string playerScoreStr = PlayerScore.ToString();
        if (playerScoreStr.Length < scoreDigits)
        {
            int lack = scoreDigits - playerScoreStr.Length;
            for (int i = 0; i < lack; i++)
            {
                playerScoreStr = 0.ToString() + playerScoreStr;
            }
        }
        string playerTimeSec = MarathonGameTimerController.Instance.Second.ToString();
        playerTimeSec = playerTimeSec.Length == 2 ? playerTimeSec : 0.ToString() + playerTimeSec;
        string playerTimeMin = MarathonGameTimerController.Instance.Minute.ToString();
        playerTimeMin = playerTimeMin.Length == 2 ? playerTimeMin : 0.ToString() + playerTimeMin;

        RivalScore = CalcScore(rivalCostTime);
        UIResourceController.DisplayNumImage(minNumRoot,
                                             playerTimeMin,
                                             minColor,
                                             numImageInterval);
        UIResourceController.DisplayNumImage(secNumRoot,
                                             playerTimeSec,
                                             secColor,
                                             numImageInterval);
        UIResourceController.DisplayNumImage(scoreNumRoot, playerScoreStr, scoreColor, numImageInterval);
        resultTimeline.Play();
    }
}