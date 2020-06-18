using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
/// =============================================
/// Author : 2019/01/28 Sa
/// History Log :
///		2019/01/28(Sa) Initial
public class BalloonScoreText : MonoBehaviour
{
    [SerializeField] private float life = 2.0f;
    [SerializeField] private float moveSpeed = 4.0f;
    [SerializeField] private float spaceInterval = 3.0f;

    private Balloon.Player owner;
    private int holdScore = 0;

    public void InitializeScoreText(Balloon.Player newOwner, Balloon balloon)
    {
        owner = newOwner;
        holdScore = balloon.GetBalloonScore();

        string str;
        if (holdScore >= 0) str = '+' + holdScore.ToString();
        else str = holdScore.ToString();
        var images = UIResourceController.DisplayNumImage(transform,
                                                          str,
                                                          Balloon.GetColorByBalloonType(balloon.PresentBallonType),
                                                          spaceInterval,
                                                          true);
        Destroy(images[0].transform.parent.gameObject, life);
    }

    private void Update()
    {
        GetComponent<RectTransform>().Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.Self);
    }

    private void OnDestroy()
    {
        BalloonGameResultUIController.Instance?.UpdateTotalScore(owner, holdScore);
    }
}