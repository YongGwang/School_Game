using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI. Notice honey amount
/// </summary>
/// =======================================================
/// Author : 2020/02/14 Sa
/// History Log :
///		2020/02/14(Sa) Initial
public class HoneyMarkHolder : MonoBehaviour
{
    [SerializeField] private GameObject honeyMarkPrefab = null;
    private List<GameObject> honeyMarks = null;

    private void Awake()
    {
        honeyMarks = new List<GameObject>();
    }

    private void Start()
    {
        for (int i = 0; i < MarathonPlayerManager.Instance.GetDashPointCount(); i++)
        {
            var temp = Instantiate(honeyMarkPrefab, transform);
            honeyMarks.Add(temp);
            temp.transform.GetChild(0).GetComponent<Image>().fillAmount = 0.0f;
        }
    }

    public void RefreshHoneyMarks(List<float> energyList)
    {
        Debug.Assert(energyList.Count == honeyMarks.Count);
        for (int i = 0; i < honeyMarks.Count; i++)
        {
            var targetImage = honeyMarks[i].transform.GetChild(0).GetComponent<Image>();
            targetImage.fillAmount = energyList[i];
            if (targetImage.fillAmount == 1.0f)
            {
                targetImage.GetComponent<Animator>().SetBool("isFull", true);
            }
            else
            {
                targetImage.GetComponent<Animator>().SetBool("isFull", false);
            }
        }
    }
}