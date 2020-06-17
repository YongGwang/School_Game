using UnityEngine;
using TMPro;

/// <summary>
/// UI timer
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Baku)
/// History Log :
///		2020/02/13(Sa) Data safety ensure
///		2020/02/25(Sa) Can get game time
public class MarathonGameTimerController : MonoBehaviour
{
    public static MarathonGameTimerController Instance = null;

    public float MarathonGameTimeInSec
    {
        get
        {
            return Hour * 3600.0f + Minute * 60.0f + Second;
        }
    }

    private float timer = 0.0f;
    public int Hour { get; private set; }
    public int Minute { get; private set; }
    public int Second { get; private set; }
    private int millisecond = 0;
    private TextMeshProUGUI timerText = null;
    private bool shouldCoutTime = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        shouldCoutTime = false;
        timerText = GetComponent<TextMeshProUGUI>();
        timer = 0.0f;
        Hour = 0;
        Minute = 0;
        Second = 0;
        millisecond = 0;
    }

    private void Start()
    {
        UpdateTimerText(timer);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Update()
    {
        if (shouldCoutTime)
        {
            timer += Time.deltaTime;
            UpdateTimerText(timer);
        }
    }

    private void UpdateTimerText(float time)
    {
        Hour = (int)time / 3600;
        Minute = ((int)time - Hour * 3600) / 60;
        Second = (int)time - Hour * 3600 - Minute * 60;
        millisecond = (int)((time - (int)time) * 100);
        timerText.text = string.Format("{0:D2}:{1:D2}.{2:D2}", Minute, Second, millisecond);
    }

    public void ShutDownCount()
    {
        shouldCoutTime = false;
    }

    public void StartCount()
    {
        shouldCoutTime = true;
    }
}