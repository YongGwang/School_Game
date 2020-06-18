using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Balloon info and behaviour
/// </summary>
/// =======================================================
/// Author : 2019/12/12 Sa
/// History Log :
///		2019/12/12(Sa) Initial
///		2019/12/19(Sa) Add BalloonStatus enum and others
///		2019/12/21(Sa) Change scale mode
///		2019/12/22(Sa) Add details to scale mode
///	    2019/01/10(Sa) Change details
///	    2019/01/12(Sa) Add size wiggle after danger status
///	    2019/01/20(Sa) Add attribute about dead size
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class Balloon : MonoBehaviour
{
    public enum Player
    {
        GamePlayer,
        AI,
        None
    }

    public enum BalloonStatus
    {
        Normal,
        Danger,
        Released,
        MovingToDeath,
    }

    // Value means air limit
    public enum BalloonType
    {
        Red = 35,
        Yellow = 45,
        Lime = 55,
        Green = 65,
        Pink = 75,
        Orange = 85
    }

    public BalloonStatus State { get; private set; }
    public BalloonType PresentBallonType { get; private set; }
    public Player Owner { get; private set; }

    [Header("Ani")]
    [Tooltip("Wiggle angle range of the balloon")]
    [SerializeField] private float wiggleAngleRange = 10.0f;
    [Tooltip("Actual wiggle time will be calculated base " +
        "on this value")]
    [SerializeField] private float baseWiggleTime = 3.0f;
    [Tooltip("The bigger the balloon is, the faster the balloon wiggles." +
        "This value define the fastest limit")]
    [SerializeField] private float wiggleTimeMinLimit = 0.2f;
    [Tooltip("In percent(0.0~1.0). Define the variability of wiggle time.")]
    [SerializeField] private float wiggleTimeRandRange = 0.2f;
    [Tooltip("拡大縮小の速度")]
    [Range(0.0f, 10.0f)]
    [SerializeField] private float trembleSpeed = 2.0f;
    [Tooltip("拡大縮小の量")]
    [SerializeField] private float trembleAmp = 3.0f;
    [Tooltip("Step of be bigger")]
    [SerializeField] private int maxScaleStep = 5;
    [Tooltip("Length of time that every time balloon be bigger.")]
    [SerializeField] private const float enlargeTime = 0.4f;
    [Tooltip("In percent(0.0~1.0). Balloon will be shrinked" +
        "what percentage of scale.")]
    [SerializeField] private const float shrinkPercent = 0.14f;
    [Tooltip("Length of time that every time balloon be shrinked")]
    [SerializeField] private const float shrinkTime = 0.8f;

    [Header("BalloonAppearance")]
    [Tooltip("爆発エフェクト")]
    [SerializeField] private GameObject boomEffect = null;
    [Tooltip("風船のテクスチャ")]
    [SerializeField] private List<Texture> balloonTextures;
    [Tooltip("In percent(0.0~1.0). The initial scale that balloon appeared on stage.")]
    [SerializeField] private float initialScaleRatio = 0.2f;

    [Header("Input")]
    [Tooltip("入力の安全範囲")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float balloonSafeRange = 0.6f;

    [Header("Other")]
    [Tooltip("Define the power when release the balloon and push it to backward")]
    [SerializeField] private float moveAwayForce = 400.0f;
    [Tooltip("First enum as base, delta between base*this value" +
    "= really size")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float scalePerAirDelta = 0.1f;
    [Tooltip("The size when balloon go to screen and boom")]
    [Range(0.0f, 1.0f)]
    [SerializeField] private float deathSizeRatio = 0.4f;

    // 0% - 100%
    private Vector3 initialScale = Vector3.zero;
    private float beInflated = 0;
    private int presentScaleStep = 0;
    private Vector3 scalePerStep = Vector3.zero;
    private IEnumerator presentScaleCorcoroutine = null;
    private AudioSource inflateSFX = null;
    private bool isInvokingWiggle = false;
    private Quaternion initialQuat = Quaternion.identity;
    private Quaternion wiggleTargetQuat = Quaternion.identity;
    private float nextWiggleTime = 0.0f;
    private float wiggleAngularVel = 0.0f;
    private Color originCol = Color.white;
    private float airLimit = 0;
    private Vector3 destination = Vector3.zero;
    private float journeyTime = 0.0f;
    private float fracComplete = 0.0f;
    private float startTime = 0.0f;
    private Vector3 initialPostionForDevote = Vector3.zero;
    private Vector3 initialScaleForDevote = Vector3.zero;

    private void Awake()
    {
        initialPostionForDevote = Vector3.zero;
        initialScaleForDevote = Vector3.zero;
        Owner = Player.None;
        startTime = 0.0f;
        journeyTime = 0.0f;
        fracComplete = 0.0f;
        destination = Vector3.zero;
        airLimit = 0;
        nextWiggleTime = 0.0f;
        wiggleAngularVel = 0.0f;
        presentScaleStep = 0;
        presentScaleCorcoroutine = null;
        State = BalloonStatus.Normal;
        isInvokingWiggle = false;
        wiggleTargetQuat = Quaternion.identity;
    }

    #region PUBLIC
    public static Color GetColorByBalloonType(BalloonType type)
    {
        Color tempCol = new Color();
        switch (type)
        {
            case BalloonType.Red:
                tempCol = Color.red;
                break;
            case BalloonType.Yellow:
                tempCol = Color.yellow;
                break;
            case BalloonType.Lime:
                tempCol = Color.HSVToRGB(0.12f, 1, 1);
                break;
            case BalloonType.Green:
                tempCol = Color.green;
                break;
            case BalloonType.Pink:
                tempCol = Color.HSVToRGB(0.9f, 0.45f, 1);
                break;
            case BalloonType.Orange:
                tempCol = Color.HSVToRGB(0.38f, 1, 1);
                break;
        }
        return tempCol;
    }

    /// <summary>
    /// Set balloon type/color/airLimit/size.
    /// Use this only when spawn this.
    /// </summary>
    /// <param name="type"></param>
    public void InitializeBalloon(BalloonType type, Player newOwner)
    {
        Owner = newOwner;
        PresentBallonType = type;
        airLimit = (float)type;
        originCol = GetColorByBalloonType(type);
        GetComponentInChildren<MeshRenderer>().
            material.SetColor("_Color", originCol);
        float sizeDelta = ((float)type -
            (float)(BalloonType)System.Enum.GetValues(typeof(BalloonType)).GetValue(0));
        transform.localScale = transform.localScale * initialScaleRatio
            + sizeDelta * scalePerAirDelta * Vector3.one;
        initialScale = transform.localScale;
        initialQuat = transform.rotation;
        scalePerStep = (initialScale / initialScaleRatio -
            initialScale) / maxScaleStep;
        GetComponent<Rigidbody>().detectCollisions = false;
        inflateSFX = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Infalte this ballone and return if it will boom
    /// </summary>
    /// <param name="inflateSpeed">For audioSource pitch(playback speed)</param>
    /// <returns>If this ballone be boomed</returns>
    public bool Inflate(float volume, float inflateSpeed)
    {
        beInflated += volume;
        if (beInflated > airLimit)
        {
            Boom();
            return true;
        }
        else
        {
            UpdateScale();
            UpdateStatus();
            if (!inflateSFX.isPlaying)
            {
                inflateSFX.pitch =
                    inflateSpeed == 0.0f ? 2.0f : inflateSpeed;
                inflateSFX.Play();
            }

            return false;
        }
    }

    /// <summary>
    /// Handle explosion of balloon
    /// </summary>
    public void Boom()
    {
        if (Owner == Player.GamePlayer) AudioManager.Instance.Stop("BalloonReady");
        AudioManager.Instance.Play("BalloonExplosion");
        Instantiate(boomEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    /// <summary>
    /// Release this balloon and push it to 
    /// backward by.a random direction.
    /// </summary>
    public void MoveAway()
    {
        if (Owner == Player.GamePlayer) AudioManager.Instance.Stop("BalloonReady");
        State = BalloonStatus.Released;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Vector3 forceDir = Vector3.Scale(Random.insideUnitSphere,
            new Vector3(1, 1, 0)) + transform.right;
        rigidbody.AddForce(transform.right * moveAwayForce);
        rigidbody.detectCollisions = true;
    }

    public void Devote(Vector3 to, float journeyTime)
    {
        State = BalloonStatus.MovingToDeath;
        this.destination = to;
        this.journeyTime = journeyTime;
        startTime = Time.time;
        initialPostionForDevote = transform.position;
        initialScaleForDevote = transform.localScale;
    }

    public int GetBalloonScore()
    {
        return (int)PresentBallonType * BalloonGameManager.Instance.BalloonScoreMultiplier;
    }
    #endregion

    #region NewAdded
    private void Wiggle()
    {
        // Process about quaternion
        if (!isInvokingWiggle)
        {
            isInvokingWiggle = true;
            nextWiggleTime = baseWiggleTime - (baseWiggleTime - wiggleTimeMinLimit) /
                maxScaleStep * presentScaleStep;
            // Add rand
            nextWiggleTime *= UnityEngine.Random.Range(1.0f - wiggleTimeRandRange,
                1.0f + wiggleTimeRandRange);
            Invoke("RefreshWiggleTargetQuat", nextWiggleTime);
        }
        var delta = Quaternion.Angle(transform.rotation, wiggleTargetQuat);
        if (delta > 0.0f)
        {
            var t = Mathf.SmoothDampAngle(delta, 0.0f, ref wiggleAngularVel, nextWiggleTime);
            t = 1.0f - t / delta;
            transform.rotation = Quaternion.Slerp(transform.rotation, wiggleTargetQuat, t);
        }

        // Process about scale
        if (State == BalloonStatus.Danger)
        {
            transform.localScale += Vector3.one * (2.0f * Mathf.PingPong(
                Time.time * trembleSpeed, trembleAmp) - trembleAmp);
        }
    }

    /// <summary>
    /// Calc next time wiggle rotation(Quaternion)
    /// </summary>
    private void RefreshWiggleTargetQuat()
    {
        Vector3 randVector = UnityEngine.Random.insideUnitSphere * 2.0f -
            new Vector3(-1.0f, -1.0f, -1.0f);
        wiggleTargetQuat = Quaternion.Euler(randVector.x * wiggleAngleRange,
                        randVector.y * wiggleAngleRange,
                        randVector.z * wiggleAngleRange) * initialQuat;
        isInvokingWiggle = false;
    }

    private void UpdateStatus()
    {
        float processPercent = beInflated / airLimit;
        if (processPercent < balloonSafeRange)
        {
            State = BalloonStatus.Normal;
        }
        else
        {
            State = BalloonStatus.Danger;
            if (Owner == Player.GamePlayer && !AudioManager.Instance.CheckIsPlaying("BalloonReady"))
            {
                AudioManager.Instance.Play("BalloonReady");
            }
        }
    }

    private void UpdateScale()
    {
        if (beInflated == 0.0f) return;

        int newStep = Mathf.RoundToInt(beInflated / (airLimit / maxScaleStep));
        if (presentScaleStep >= newStep) return;

        if (presentScaleCorcoroutine != null)
        {
            StopCoroutine(presentScaleCorcoroutine);
        }
        presentScaleCorcoroutine = ScaleBalloon(newStep - presentScaleStep);
        StartCoroutine(presentScaleCorcoroutine);
        presentScaleStep = newStep;
    }

    private IEnumerator ScaleBalloon(int stepDelta)
    {
        Vector3 startScale = transform.localScale;
        Vector3 vel = Vector3.zero;
        Vector3 targetScale = startScale + scalePerStep * stepDelta;
        // Enlarge 
        while (Mathf.Abs(transform.localScale.x - targetScale.x) > 0.02f)
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale,
                ref vel, enlargeTime);
            yield return null;
        }
        // Shrink
        targetScale = transform.localScale * (1.0f - shrinkPercent);
        while (Mathf.Abs(transform.localScale.x - targetScale.x) > 0.02f)
        {
            transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale,
                ref vel, shrinkTime);
            yield return null;
        }
        presentScaleCorcoroutine = null;
    }
    #endregion

    private void Update()
    {
        if (State != BalloonStatus.Released)
        {
            Wiggle();
        }

        if (State == BalloonStatus.MovingToDeath)
        {
            fracComplete = (Time.time - startTime) / journeyTime;
            transform.position = Vector3.Slerp(initialPostionForDevote,
                                               this.destination, fracComplete);
            transform.localScale = Vector3.Slerp(initialScaleForDevote,
                                                 initialScaleForDevote * deathSizeRatio, fracComplete);
            // Reached destination
            if (Vector3.Distance(transform.position, destination) <= 2.0f)
            {
                BalloonGameResultUIController.Instance.SpawnScoreText(Owner, this);
                Boom();
            }
        }
    }
}