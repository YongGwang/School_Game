using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;  //TODO: Del this after changed SceneManager

public class GameManager : MonoBehaviour
{
	public static bool bIsGameStarted = false;
	public Text inGameTimeCounterText;
	public Text gameClearInputText;
	public Text rankReturnHintText;
	public Animator countdownAnimator;
	public MovePlayer playerInfo;

    // Rank UI
    [SerializeField] private GameObject rankUI = null;
    private List<Animator> rankUIanimators = new List<Animator>();
    private const float RankBarWaitTime = 1.0f;
    private IEnumerator PresentShowRankBarCoroutine = null;
    private bool bIsQuickPlay = false;

    private float gameTime;
	private InputField gameClearInputField;
	private bool bIsGameClear = false;
	private List<string[]> csvTextData;
	private string filePath;
	private bool bIsRankInTop10;

    // Const
    private const string GameRankFileName = "GameRankData";
	private const KeyCode Restart = KeyCode.F1; //TODO: Del this after changed SceneManager
	private const float PrintLineWaitTime = 1.1f;

	// Start is called before the first frame update
	void Start()
	{
		csvTextData = new List<string[]>();
		filePath = Application.dataPath + "/Resources/" + GameRankFileName + ".csv";

        // Rank UI
        foreach (var i in rankUI.GetComponentsInChildren<Animator>())
        {
            rankUIanimators.Add(i);
        }
        PresentShowRankBarCoroutine = null;

    gameClearInputText.gameObject.SetActive(false);
        bIsQuickPlay = rankReturnHintText.enabled = bIsGameStarted = bIsRankInTop10 = bIsGameClear = false;
		gameTime = 0.0f;
		UpdateTimeCounterText();
		MovePlayer.OnPlayerReachedGoal += GameClear;    // Call GameClear func when playe reach goal(check MovePlayer file)
		gameClearInputField = gameClearInputText.GetComponentInChildren<InputField>();
		gameClearInputField.characterLimit = 10;

		// Create CSV file that holding game rank data
		if (!Resources.Load(GameRankFileName))
		{
			System.IO.File.CreateText(filePath);
		}
		UpdateCsvTextData();
	}

	// Update is called once per frame
	void Update()
	{
		if (!bIsGameStarted)
		{
			if (countdownAnimator.GetCurrentAnimatorStateInfo(0).IsName("Countdown"))
			{
				return;
			}
			else
			{
				bIsGameStarted = true;
				StartCoroutine(playerInfo.MoveParticle());
			}
			Destroy(countdownAnimator.gameObject);
		}

		if (!bIsGameClear)
		{
			UpdateTimeCounterText();
		}
        else
        {
            if (
                PresentShowRankBarCoroutine != null && 
                ( Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Joystick1Button0) )
                )
            {
				StopCoroutine(PresentShowRankBarCoroutine);
                PresentShowRankBarCoroutine = null;
                bIsQuickPlay = true;
            }
        }

		//level load
		if (Input.GetKeyDown(Restart))
		{
			SceneManager.LoadScene("TestRun", LoadSceneMode.Single);
		}
	}

	private void UpdateTimeCounterText()
	{
		gameTime = Time.timeSinceLevelLoad;
		inGameTimeCounterText.text = gameTime.ToString("f2") + " Mins";
	}

	private void GameClear()
	{
		Debug.Log("Game Clear");
		gameClearInputText.gameObject.SetActive(true);
		bIsGameClear = true;
	}

	// Call after player input name and push enter key
	public void FinishInputAndSaveGameTimeData()
	{
		// If player rank in Top10, save data in file
		if (csvTextData.Count < 9 ||
			(csvTextData.Count >= 9 && CalcScore(gameTime) > float.Parse(csvTextData[csvTextData.Count - 1][1])))
		{
			bIsRankInTop10 = true;
			string[] newData = { gameClearInputField.text, CalcScore(gameTime).ToString() };
			csvTextData.Add(newData);
			csvTextData.Sort((b, a) => float.Parse(a[1]).CompareTo(float.Parse(b[1])));
			if (csvTextData.Count > 10) csvTextData.RemoveRange(10, csvTextData.Count - 10);

			// Refesh CSV file
			string all = System.String.Empty;
			foreach (var i in csvTextData)
			{
				for (int j = 0; j < i.Length; j++)
				{
					all += i[j];
					if (j < i.Length - 1) all += ',';
				}
				all += "\n";
			}
			File.WriteAllText(filePath, all);
		}

		Invoke("ShowRank", 0.2f);
		gameClearInputText.gameObject.SetActive(false);
	}

	private void ShowRank()
	{
        if (PresentShowRankBarCoroutine != null)
        {
            StopCoroutine(PresentShowRankBarCoroutine);
        }
        PresentShowRankBarCoroutine = PrintLine(csvTextData);
        StartCoroutine(PrintLine(csvTextData));
	}

	private IEnumerator PrintLine(List<string[]> text)
	{
        for (int i = 0; i < rankUIanimators.Count; i++)
        {
            Text rankInfo = rankUIanimators[i].gameObject.
                transform.GetChild(0).GetChild(0).GetComponent<Text>();
            if (i < csvTextData.Count)
            {
                string[] line = csvTextData[i];
                rankInfo.text = (i + 1).ToString().PadRight(9) + line[0].PadRight(13) + line[1];
            }
            else
            {
                if (!bIsRankInTop10)
                {
                    rankInfo.text = "NoRank".PadRight(9) + gameClearInputField.text.PadRight(13) + CalcScore(gameTime).ToString();
                }
                else
                {
                    rankInfo.text = "NONE";
                }
            }
            rankUIanimators[i].SetBool("CanShow", true);
            if (!bIsQuickPlay)
            {
                yield return new WaitForSeconds(RankBarWaitTime);
            }
            else
            {
                yield return null;
            }
        }
        PresentShowRankBarCoroutine = null;

        // Wait push any key
        rankReturnHintText.enabled = true;
		yield return new WaitWhile(() => !Input.anyKeyDown);
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}

	private void UpdateCsvTextData()
	{
		var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		var sr = new StreamReader(fs);
		csvTextData.Clear();
		while (sr.Peek() != -1)
		{
			string line = sr.ReadLine();
			csvTextData.Add(line.Split(','));
		}
	}

    int CalcScore(float gameTime)
	{
		int newSocre = 0;
		if (gameTime <= 65.0f)
		{
			newSocre = 8000 * playerInfo.PresentItemGettedCount;
		}
		else
		{
			int costScore = Mathf.RoundToInt((gameTime - 65.0f) / 0.01f * 2.0f);
			newSocre = (8000 - costScore) * playerInfo.PresentItemGettedCount;
		}
		return Mathf.Clamp(newSocre, 0, 999999);
	}

	private void OnDestroy()
	{
		MovePlayer.OnPlayerReachedGoal -= GameClear;    // Call GameClear func when playe reach goal(check MovePlayer file)
	}
}