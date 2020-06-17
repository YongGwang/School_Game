using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class Menu_SelectIcon : MonoBehaviour
{
    int index = 0;
    public int totalLevels = 3;
    public float yOffset = 1f;

    #region ParamtersForEditor
    [Header("移動Object")]
    public GameObject[] menuObjs;

    [Header("メイン入力設定")]
    public KeyCode UpArrowKey_pad = KeyCode.JoystickButton3;
    public KeyCode DownArrowKey_pad = KeyCode.JoystickButton3;

    [Tooltip("アクションキー")]
    public KeyCode actionKey = KeyCode.DownArrow;
    public KeyCode actionKey_pad = KeyCode.JoystickButton2;
    #endregion
    public GameObject BlackPanelObject;
    private bool bIsShowingRank = false;
    [SerializeField] private GameObject rankUI = null;
    private AudioSource MoveIconSound;
    private AudioSource SelectIconSound;
    private List<Animator> rankUIanimators = new List<Animator>();

    private const float RankBarWaitTime = 1.0f;
    private IEnumerator PresentShowRankBarCoroutine = null;
    private string filePath;
    List<string[]> csvData = new List<string[]>();

	private float timerForMoveCursors = 0.0f;
	private const float moveCursorsCoolDown = 0.2f;
	private bool bIsMovedCursors = false;

    // Start is called before the first frame update
    void Start()
    {
        //fadeIn
        BlackPanelObject.SetActive(false);

        //sounds
        MoveIconSound = GetComponents<AudioSource>()[0]; ;
        SelectIconSound = GetComponents<AudioSource>()[1]; ;

        filePath = Application.dataPath + "/Resources/" + "GameRankData.csv";
		bIsMovedCursors = bIsShowingRank = false;
        foreach (var i in rankUI.GetComponentsInChildren<Animator>())
        {
            rankUIanimators.Add(i);
        }

        var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var sr = new StreamReader(fs);
        while (sr.Peek() != -1)
        {
            csvData.Add(sr.ReadLine().Split(','));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            Debug.Log("DashStart!!!!");
        }

        // Handle Timer
        if (bIsMovedCursors)
		{
			timerForMoveCursors += Time.deltaTime;
			if (timerForMoveCursors > moveCursorsCoolDown)
			{
				bIsMovedCursors = false;
				timerForMoveCursors = 0.0f;
			}
		}

        if (!bIsShowingRank)
        {
            MoveCursors();
            InputSelection();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(actionKey_pad) || Input.GetKeyDown(KeyCode.JoystickButton1))
            {
                bIsShowingRank = false;
                foreach (var i in rankUIanimators)
                {
                    i.SetBool("CanShow", false);
                }

                if (PresentShowRankBarCoroutine != null)
                {
                    StopCoroutine(PresentShowRankBarCoroutine);
                    PresentShowRankBarCoroutine = null;
                }
            }
        }
    }

    private void InputSelection()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(actionKey_pad) || Input.GetButtonDown("Stick_Dash"))
        {
            SelectIconSound.Play();

            if (index == 0)
            {
                StartCoroutine(DelayFunction());
            }
            else if (index == 1)
            {
                bIsShowingRank = true;
                if (PresentShowRankBarCoroutine != null)
                {
                    StopCoroutine(PresentShowRankBarCoroutine);
                }
                PresentShowRankBarCoroutine = ShowRankBar();
                StartCoroutine(PresentShowRankBarCoroutine);
            }
            else if (index == 2)
            {
                Application.Quit();
            }
        }
    }

    IEnumerator ShowRankBar()
    {
        for (int i = 0; i < rankUIanimators.Count; i++)
        {
            Text rankInfo = rankUIanimators[i].gameObject.
                transform.GetChild(0).GetChild(0).GetComponent<Text>();
            if (i < csvData.Count)
            {
                string[] line = csvData[i];
                rankInfo.text = (i + 1).ToString().PadRight(9) + line[0].PadRight(13) + line[1];
            }
            else
            {
                rankInfo.text = "NONE";
            }

            rankUIanimators[i].SetBool("CanShow", true);
            Debug.Log(i.ToString());
            yield return new WaitForSeconds(RankBarWaitTime);
        }
        PresentShowRankBarCoroutine = null;
    }

    IEnumerator DelayFunction()
    {
        BlackPanelObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    private void MoveCursors()
    {
        int GetVerticalAxis = (int)Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.DownArrow) ||
			( GetVerticalAxis < 0 && !bIsMovedCursors))
        {
            //sound
            MoveIconSound.Play();
			bIsMovedCursors = true;
			if (index < totalLevels - 1)
            {
                index++;
                Vector3 position = transform.position;
                position.y -= yOffset;
                foreach (var i in menuObjs)
                {
                    i.transform.position = position;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) ||
			(GetVerticalAxis > 0 && !bIsMovedCursors))
		{
            //sound
            MoveIconSound.Play();
			bIsMovedCursors = true;
			if (index > 0)
            {
                index--;
                Vector3 position = transform.position;
                position.y += yOffset;
                foreach (var i in menuObjs)
                {
                    i.transform.position = position;
                }
            }
        }
    }
}