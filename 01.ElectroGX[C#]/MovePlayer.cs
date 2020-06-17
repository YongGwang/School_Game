using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;	// TODO: Del this after updated Player's NS sign

public class MovePlayer : MonoBehaviour
{
    #region ParamtersForEditor
    [Header("キャラクターの移動設定")]
    [Range(1, 20)]
    [Tooltip("最大何段階があるか")]
    public int maxSpeedStep = 12;
    [Range(1, 20)]
    [Tooltip("デフォルトのキャラクター移動速度")]
    public float baseSpeed = 3.5f;
    [Range(0.0f, 10.0f)]
    [Tooltip("実際のキャラ速度 ＝ Base Speed(デフォルトの速度) + PresentSpeedStep(今の速度段階) * valuePreSpeedStep(一段階上がると加える速さ)")]
    public float valuePreSpeedStep = 2.5f;
    [Range(0.0f, 100.0f)]
    [Tooltip("横移動の速さ")]
    public float horizontalSpeed = 20.0f;
    [Range(0.0f, 100.0f)]
    [Tooltip("ダッシュ状態の時間長さ")]
    public float dashTime = 1.1f;

    [Header("入力設定")]
    [Tooltip("左磁石切り替え入力")]
    public KeyCode leftMagnetSwitchKey = KeyCode.LeftArrow;
    public KeyCode leftMagnetSwitchKey_pad = KeyCode.JoystickButton4;
    [Tooltip("右磁石切り替え入力")]
    public KeyCode rightMagnetSwitchKey = KeyCode.RightArrow;
    public KeyCode rightMagnetSwitchKey_pad = KeyCode.JoystickButton5;
    [Tooltip("アクションキー")]
    public KeyCode actionKey = KeyCode.DownArrow;
    public KeyCode actionKey_pad = KeyCode.JoystickButton3;
    #endregion

    #region Public
    [Header("以下は触らなくてもいいもの")]
    public GameObject Effect;
    public GameObject ActionHintObj;    // When move in quick action area, this image will appear, notice player push action key

    // Lighting LineRenderer
    public GameObject LeftLightningObj;
    public GameObject RightLightningObj;

    public Gradient redGradient;
    public Gradient blueGradient;

    [Range(0.0f, 2.0f)] public float FreezeInputTimeAfterDamaged = 0.4f;
    [Range(0.0f, 2.0f)] public float GodModeTimeAfterDamaged = 1.2f;
    public Text NStext;     // TODO:Del this after updated Player's NS sign
    public bool bFlash = false;
    public bool bCameraStop = false;
    public float interval = 0.2f;

    // Events
    public static event System.Action OnPlayerReachedGoal;
    public static event System.Action OnGetSpeedUpItem;
    public static event System.Action OnHitFence;

    public float PresentPlayerForwardSpeedStep { get; private set; }
    private int presentItemGettedCount;
    public int PresentItemGettedCount
    {
        get { return presentItemGettedCount == 0 ? 1 : presentItemGettedCount; }
    }

    #endregion

    #region Private
    private AudioSource MoveAudioSource;
    private AudioSource DashAudioSource;
    [SerializeField] private GameObject Abutton = null;
    [SerializeField] private AudioClip[] audioClip;
    [SerializeField] private GameObject leftMagnet = null;
    private Material leftMagnetMat;
    [SerializeField] private GameObject rightMagnet = null;
    private Material rightMagnetMat;
    private LineRenderer leftLightingRenderer;
    private LineRenderer rightLightingRenderer;
    private float timerForFreezeInput = 0.0f;
    private bool bIsGodModeByDamaged = false;
    private float timerForGodModByDamaged = 0.0f;
    private ParticleSystem.MainModule leftMagnetMainModule;
    private ParticleSystem.MainModule rightMagnetMainModule;


    [SerializeField] private GameObject CharacterModelOne = null;
    [SerializeField] private GameObject CharacterModelTwo = null;

    private bool bIsLeftN = true;   // Is left side magnet N ?
    private bool bIsRightN = true;
    private bool bIsPlayerN = true;
    private bool bIsPlayerStartRun = false;
    private float PresentPlayerHorizontalSpeed = 0.0f;

    // Dash
    private bool bIsPlayerDashing = false;
    private float TimerForDash = 0.0f;
    [SerializeField] private GameObject dashEffect = null;

    private float alphaSin;
    private float Timer;
    private IEnumerator ProcessingSpeedDownCoroutine = null;

    // Set movement const
    private const float ChangePathInterval = 0.1f;

    // Set position const
    private const float LeftPathXPosition = -1.5f;
    private const float MiddlePathXPosition = 0.0f;
    private const float RightPathXPosition = 1.5f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        ActionHintObj.SetActive(false);
        dashEffect.SetActive(false);
        Effect.SetActive(false);
        CharacterModelOne.SetActive(true);
        CharacterModelTwo.SetActive(true);
        presentItemGettedCount = 0;
        timerForGodModByDamaged = timerForFreezeInput = PresentPlayerForwardSpeedStep = TimerForDash = PresentPlayerHorizontalSpeed = 0.0f;

        PresentPlayerForwardSpeedStep = 0.0f;
        TimerForDash = PresentPlayerHorizontalSpeed = 0.0f;
        bIsPlayerN = bIsLeftN = bIsRightN = true;
        bIsPlayerDashing = bIsPlayerStartRun = bIsGodModeByDamaged = bIsPlayerDashing = false;
        leftMagnetMat = leftMagnet.GetComponent<ParticleSystemRenderer>().material;
        rightMagnetMat = rightMagnet.GetComponent<ParticleSystemRenderer>().material;

        // magnet particle Sys
        leftMagnetMainModule = leftMagnet.GetComponent<ParticleSystem>().main;
        rightMagnetMainModule = rightMagnet.GetComponent<ParticleSystem>().main;
        leftMagnetMainModule.simulationSpeed = rightMagnetMainModule.simulationSpeed = 0.1f;

        // Lighting linerenderer
        leftLightingRenderer = LeftLightningObj.GetComponent<LineRenderer>();
        rightLightingRenderer = RightLightningObj.GetComponent<LineRenderer>();
        leftLightingRenderer.colorGradient = rightLightingRenderer.colorGradient = redGradient;

        leftMagnetMat.color = rightMagnetMat.color = Color.red;
        NStext.text = "N";		// TODO:Del this after updated Player's NS sign

        //Sound
        MoveAudioSource = GetComponents<AudioSource>()[0];
        DashAudioSource = GetComponents<AudioSource>()[1];

        //A_Button
        Abutton.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            Debug.Log("DashStart!!!!");
        }

        //LightningEffect
        LeftLightningObj.transform.position = new Vector3(0, 0, this.gameObject.transform.position.z);
        RightLightningObj.transform.position = new Vector3(0, 0, this.gameObject.transform.position.z);

        if (!GameManager.bIsGameStarted)
        {
            PresentPlayerForwardSpeedStep = -1;
        }
        else
        {
            if (!bIsPlayerStartRun)
            {
                PresentPlayerForwardSpeedStep = 0;
                bIsPlayerStartRun = true;
            }
        }

        // Assign Velocity for movement
        Vector3 vel;
        if (bIsPlayerDashing)
        {
            TimerForDash += Time.deltaTime;
            vel = new Vector3(PresentPlayerHorizontalSpeed, 0, baseSpeed + maxSpeedStep * valuePreSpeedStep);
            if (TimerForDash >= dashTime)
            {
                bIsPlayerDashing = false;
                dashEffect.SetActive(false);
                Effect.SetActive(false);
                CharacterModelOne.SetActive(false);
                CharacterModelTwo.SetActive(false);
                TimerForDash = 0.0f;
            }
        }
        else
        {
            vel = new Vector3(PresentPlayerHorizontalSpeed, 0, baseSpeed + PresentPlayerForwardSpeedStep * valuePreSpeedStep);
            CharacterModelOne.SetActive(true);
            CharacterModelTwo.SetActive(true);
        }
        transform.position += vel * Time.deltaTime;

        // Synchronize magnet position
        leftMagnet.transform.position = new Vector3(leftMagnet.transform.position.x, leftMagnet.transform.position.y, transform.position.z + 200.0f);
        rightMagnet.transform.position = new Vector3(rightMagnet.transform.position.x, rightMagnet.transform.position.y, transform.position.z + 200.0f);
        ActionHintObj.GetComponent<RectTransform>().position = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);

        //IsDamaged?
        if (bIsGodModeByDamaged)
        {
            timerForGodModByDamaged += Time.deltaTime;
            if (timerForGodModByDamaged > GodModeTimeAfterDamaged)
            {
                bIsGodModeByDamaged = false;
                timerForGodModByDamaged = 0.0f;
            }
        }

        HandleInput();
        ChangePath();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(leftMagnetSwitchKey) || Input.GetKeyDown(leftMagnetSwitchKey_pad))
        {
            bIsLeftN = false;
            leftMagnetMat.color = Color.blue;
            leftLightingRenderer.colorGradient = blueGradient;
            MoveAudioSource.Play();
        }
        else if (Input.GetKeyUp(leftMagnetSwitchKey) || Input.GetKeyUp(leftMagnetSwitchKey_pad))
        {
            bIsLeftN = true;
            leftMagnetMat.color = Color.red;
            leftLightingRenderer.colorGradient = redGradient;
        }

        if (Input.GetKeyDown(rightMagnetSwitchKey) || Input.GetKeyDown(rightMagnetSwitchKey_pad))
        {
            bIsRightN = false;
            rightMagnetMat.color = Color.blue;
            rightLightingRenderer.colorGradient = blueGradient;
            MoveAudioSource.Play();
        }
        else if (Input.GetKeyUp(rightMagnetSwitchKey) || Input.GetKeyUp(rightMagnetSwitchKey_pad))
        {
            bIsRightN = true;
            rightMagnetMat.color = Color.red;
            rightLightingRenderer.colorGradient = redGradient;
        }
    }
    private void ChangePath()
    {
        if (bIsLeftN == bIsRightN)  // Player stay in middle
        {
            if (Mathf.Abs(transform.position.x - MiddlePathXPosition) >= 0.1f)
            {
                PresentPlayerHorizontalSpeed = (transform.position.x < MiddlePathXPosition ? 1 : -1) * horizontalSpeed;
            }
            else
            {
                PresentPlayerHorizontalSpeed = 0.0f;
                transform.position = new Vector3(MiddlePathXPosition, transform.position.y, transform.position.z);
            }
        }
        else if (bIsLeftN && !bIsRightN)  // Player move to right
        {
            if (transform.position.x < RightPathXPosition)
            {
                PresentPlayerHorizontalSpeed = horizontalSpeed;
            }
            else
            {
                PresentPlayerHorizontalSpeed = 0.0f;
                transform.position = new Vector3(RightPathXPosition, transform.position.y, transform.position.z);
            }
        }
        else if (!bIsLeftN && bIsRightN)  // Player move to left
        {
            if (transform.position.x > LeftPathXPosition)
            {
                PresentPlayerHorizontalSpeed = -horizontalSpeed;
            }
            else
            {
                PresentPlayerHorizontalSpeed = 0.0f;
                transform.position = new Vector3(LeftPathXPosition, transform.position.y, transform.position.z);
            }
        }
    }

    #region AboutCollision
    private void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.gameObject.tag == "Fence")
        {
            if (bIsPlayerDashing || bIsGodModeByDamaged) return;   // God mode
            OnHitFence?.Invoke();
            bIsGodModeByDamaged = true;
            Explosion exp = otherObj.GetComponent<Explosion>();
            exp.Explode();
            StartCoroutine(FlashOff(4f));
            //ActionHintObj.SetActive(false);
            Abutton.SetActive(false);
            // Prevention call coroutine repeatedly
            if (ProcessingSpeedDownCoroutine != null)
            {
                StopCoroutine(ProcessingSpeedDownCoroutine);
            }
            else
            {
                ProcessingSpeedDownCoroutine = SpeedDown();
                StartCoroutine(ProcessingSpeedDownCoroutine);
            }
        }
        else if (otherObj.gameObject.tag == "Goal")
        {
            //Destroy(gameObject);
            OnPlayerReachedGoal?.Invoke();
            bCameraStop = true;
            NStext.text = "";
        }
        else if (otherObj.gameObject.tag == "Electronic")
        {
            if (bIsGodModeByDamaged) return;
            PresentPlayerForwardSpeedStep = Mathf.Clamp(PresentPlayerForwardSpeedStep + 1.0f, 1.0f, maxSpeedStep);
            ++presentItemGettedCount;
            OnGetSpeedUpItem?.Invoke();

            Destroy(otherObj.gameObject);
        }

        if (otherObj.gameObject.tag == "QuickActionArea")
        {
            if (bIsGodModeByDamaged) return;

            //ActionHintObj.SetActive(true);
            Abutton.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "QuickActionArea")
        {
            //ActionHintObj.SetActive(false);
            Abutton.SetActive(false);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "QuickActionArea")
        {
            if (Input.GetKeyDown(actionKey) || Input.GetKeyDown(actionKey_pad) || Input.GetButtonDown("Stick_Dash"))
            {
                Debug.Log("Action Success");
                Explosion exp = other.gameObject.transform.parent.GetChild(0).gameObject.GetComponent<Explosion>();
                CharacterModelOne.SetActive(false);
                CharacterModelTwo.SetActive(false);
                if (exp)
                {
                    exp.Explode();
                }
                //ActionHintObj.SetActive(false);
                Abutton.SetActive(false);
                if (bIsPlayerDashing)
                {
                    TimerForDash = dashTime;
                    Debug.Log("reset timer");
                }
                bIsPlayerDashing = true;
                dashEffect.SetActive(true);
                Effect.SetActive(true);     //Effect Active
                other.enabled = false;
                DashAudioSource.Play();
            }
        }
    }
    #endregion

    IEnumerator ColorCoroutine()
    {
        while (bFlash)
        {
            // var renderComponent = GetComponent<Renderer>();
            // renderComponent.enabled = !renderComponent.enabled;
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator FlashOff(float time)
    {
        while (time > 0)
        {
            bFlash = true;
            yield return new WaitForSeconds(0.2f);
            bFlash = false;
            time--;
            yield return null;  // ここ
        }
    }

    IEnumerator SpeedDown()
    {
        float tempVelStep = Mathf.Clamp(PresentPlayerForwardSpeedStep - 2, 3, maxSpeedStep);

        if (PresentPlayerForwardSpeedStep < 5)
        {
            PresentPlayerForwardSpeedStep = 0;
        }
        else
        {
            PresentPlayerForwardSpeedStep = PresentPlayerForwardSpeedStep - 5;
        }
        yield return new WaitForSeconds(1.5f);
        PresentPlayerForwardSpeedStep = tempVelStep;
        ProcessingSpeedDownCoroutine = null;
    }

    public IEnumerator MoveParticle()
    {
        float newSpeed = 0.0f;
        while (leftMagnetMainModule.simulationSpeed <= 1.0f)
        {
            newSpeed += 0.09f * Time.deltaTime;
            leftMagnetMainModule.simulationSpeed = rightMagnetMainModule.simulationSpeed += newSpeed;
            yield return null;
        }
        leftMagnetMainModule.simulationSpeed = rightMagnetMainModule.simulationSpeed = 1.0f;
    }
}