using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// </summary>
/// =======================================================
/// Author : 2020/02/04 Sa
/// History Log :
///		2020/02/04(Sa) Initial
///		2020/02/04(Sa) Score start with 0 to max
///		2020/02/21(Sa) Async load next scene
///		2020/02/22(Sa) Use sprite for score
public class ResultSceneManger : MonoBehaviour
{
    [SerializeField] private float calcTimeInterval = 0.1f;
    [SerializeField] private int calcStep = 12;
    [SerializeField] private int calcStepVariety = 6;
    [SerializeField] private float textSpaceInterval = 12.0f;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private RectTransform playerScoreTextHolder = null;
    [SerializeField] private RectTransform rivalScoreTextHolder = null;
    [SerializeField] private TMP_Text resultText = null;
    [SerializeField] private Transform middleMarkTrans = null;

    private float playerScore = 0;
    private float playerScoreMax = 0;
    private float rivalScore = 0;
    private float rivalScoreMax = 0;
    private float allScoreMax = 0;
    private float scoreGap = 0;
    private float timer;
    private Vector3 initiLeftBarPos = Vector3.zero;
    private Vector3 initiRightBarPos = Vector3.zero;
    private Vector3 initiMiddleMarkPos = Vector3.zero;
    private AsyncOperation async = null;

    private void Awake()
    {
        playerScoreMax = BalloonGameManager.PlayerScore + MarathonScoreController.PlayerScore;
        playerScoreMax = 1333;
        playerScore = 0.0f;
        rivalScoreMax = BalloonGameManager.RivalScore + MarathonScoreController.RivalScore;
        rivalScoreMax = 1222;
        rivalScore = 0.0f;
        allScoreMax = playerScore + rivalScore;
        scoreGap = Mathf.Abs(playerScoreMax - rivalScoreMax);

        // Setting
        initiMiddleMarkPos = middleMarkTrans.position;
        initiLeftBarPos = middleMarkTrans.GetChild(0).position;
        initiRightBarPos = middleMarkTrans.GetChild(1).position;
        resultText.gameObject.SetActive(false);
    }

    private void Start()
    {
        ProcessScoreText();
        if (playerScoreMax > rivalScoreMax)
        {
            resultText.text = "Kuma Win";
            async = Utility.MoveSceneAsync(Utility.GetCurretSceneIndex() + 1);
        }
        else
        {
            resultText.text = "Deer Win";
            async = Utility.MoveSceneAsync(Utility.GetCurretSceneIndex() + 2);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Utility.ReloadCurrentScene();
        }

        if (playerScore == playerScoreMax && rivalScore == rivalScoreMax)
        {
            resultText.gameObject.SetActive(true);

            // Go next scene
            if (JoyConInputManager.GetJoyConAnykeyDown() || Input.anyKeyDown)
            {
                async.allowSceneActivation = true;
                Resources.UnloadUnusedAssets();
                System.GC.Collect();
            }
            return;
        }

        if (timer >= calcTimeInterval)
        {
            timer = 0.0f;

            // Main process
            float playerScoreAdd = calcStep + UnityEngine.Random.Range(0, calcStepVariety);
            float rivalScoreAdd = calcStep + UnityEngine.Random.Range(0, calcStepVariety);
            UpdateScore(ref playerScore, ref playerScoreAdd, playerScoreMax);
            UpdateScore(ref rivalScore, ref rivalScoreAdd, rivalScoreMax);
            ProcessScoreBar(playerScoreAdd, rivalScoreAdd);
            ProcessScoreText();
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private void ProcessScoreBar(float playerScoreCut, float rivalScoreCut)
    {
        // Bar process
        if (scoreGap == 0.0f) return;
        float delta = (playerScoreCut - rivalScoreCut) / scoreGap / 2.0f;
        middleMarkTrans.position += new Vector3(
                                    Vector3.Distance(initiLeftBarPos, initiRightBarPos) * delta,
                                    0.0f, 0.0f);
    }

    private void ProcessScoreText()
    {
        UIResourceController.DisplayNumImage(playerScoreTextHolder,
                                             playerScore.ToString(),
                                             textColor,
                                             textSpaceInterval,
                                             false);

        UIResourceController.DisplayNumImage(rivalScoreTextHolder,
                                             rivalScore.ToString(),
                                             textColor,
                                             textSpaceInterval,
                                             false);
    }

	/// <summary>
	/// Consider scoreAdd will be used in UI process for add/decrease bar, the num must 
	/// be right, so set ref for scoreAdd
	/// </summary>
	/// <param name="totalScore">Present score</param>
	/// <param name="scoreAdd">Add score</param>
	/// <param name="scoreMax">Max limit of score</param>
    private void UpdateScore(ref float totalScore,ref float scoreAdd, float scoreMax)
    {
        if (totalScore + scoreAdd >= scoreMax)
        {
			scoreAdd = scoreMax - totalScore;
			totalScore = scoreMax;
        }
        else
        {
            totalScore += scoreAdd;
        }
    }
}