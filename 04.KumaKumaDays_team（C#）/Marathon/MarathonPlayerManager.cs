using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Marathon player behaviour and state controller.
/// Warning: Use RecenterPlayerJoyCon instant of JoyConInputManger.Instance.RecenterJoyCon().
/// Scene manager plz check MarathonGameManager class.
/// </summary>
/// =======================================================
/// Author : 2020/02/11(Sa)
/// History Log :
///		2020/02/11(Sa) Initial.
///		2020/02/12(Sa) Fixed bug about cannot fell out bridge
///		               Add HoldUp state.
///		2020/02/13(Sa) Add keyboard input.
///		2020/02/20(Sa) Change OnTrigger to OnCollision.
///		2020/02/24(Sa) Move all process happen on change state
///		               into SetState function.
///		               Bridge reset joyCon after fixed second.
///		2020/02/25(Sa) Add sounds
public class MarathonPlayerManager : MonoBehaviour
{
    public enum MarathonPlayerStatus
    {
        WarmingUp = -1,
        Running = 0,
        Dashing = 1,
        BeKnockedBack = 2,
        Dizzying = 3,
        Dancing = 4,
        Falling = 5,
        PassingBridge = 6,
        HoldUp = 7,
        ReachGoal = 8
    }

    public MarathonPlayerStatus State { get; private set; }
    public static MarathonPlayerManager Instance = null;

    [Header("Input")]
    [SerializeField] private bool isUsingLeftJoyCon = false;
    [Range(0.0f, 180.0f)]
    [SerializeField] private float angleLimit = 70.0f;
    [Range(0.0f, 200.0f)]
    [SerializeField] private float joyConRotSpeed = 100.0f;
    [Range(0.0f, 1.0f)]
    [Tooltip("何もせずでもズレ修正")]
    [SerializeField] private float JoyConRotFixAngle = 0.62f;
    [Range(1.0f, 10.0f)]
    [SerializeField] private float JoyConRotFixPower = 1.144f;
    [SerializeField] private float keyboardRotSpeed = 100.0f;

    [Header("Running")]
    [SerializeField] private float runSpeed = 10.0f;

    [Header("BeKnockedBack")]
    [SerializeField] private float BeKnockedBackTimeLength = 0.5f;
    [SerializeField] private float rollingBackwardSpeed = 20.0f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 20.0f;
    [SerializeField] private float dashTime = 4.0f;
    [Tooltip("エネルギーが満ちる時、ダッシュできる回数")]
    [SerializeField] private int dashPointAmount = 2;
    [Tooltip("1 Pointにどのぐらいのエネルギーを持つ")]
    [SerializeField] private float dashPointCapacity = 1.0f;
    [SerializeField] private HoneyMarkHolder honeyMarkHolder = null;

    [Header("Bridge")]
    [Range(0.0f, 1.0f)]
    [Tooltip("Lower, stricter")]
    [SerializeField] private float accelTolerance = 0.4f;
    [Range(0.8f, 1.0f)]
    [Tooltip("Higher, stricter")]
    [SerializeField] private float angleTolerance = 0.98f;
    [SerializeField] private float checkUnstableAfterSec = 1.2f;
    [SerializeField] private float unstableHoldTime = 0.5f;
    [Tooltip("When unstableHoldTime, in firset unstableRemedyTime you can" +
        "fix the input to stable")]
    [SerializeField] private float unstableRemedyTime = 0.5f;
    [Tooltip("橋から外へ落ちる力")]
    [SerializeField] private float fellOutForce = 20.0f;

    [Header("Falling")]
    [SerializeField] private float fallingTimeLength = 3.0f;
    [SerializeField] private float landBaseline = -1.0f;
    [SerializeField] private Vector3 resetPositionOffset = new Vector3(0f, 1.0f, 0f);
    [SerializeField] private float afterResetHoldUpTimeLength = 3.0f;

    [Header("Dizzying")]
    [SerializeField] private float dizzyingTimeLength = 3.0f;
    [SerializeField] private GameObject dizzyEffectObj = null;

    [Header("Dancing")]
    [SerializeField] private float dancingTimeLength = 3.0f;

    [Header("PassingBridge")]
    [SerializeField] private float passBridgeSpeed = 7.0f;

    [Header("Finish")]
    [SerializeField] private float rotToCameraSpeed = 20.0f;

    private float presentMoveSpeed = 0.0f;
    private Rigidbody rd = null;
    private Vector3 cameraDistance = Vector3.zero;
    private Animator ani = null;
    private Bridge passingBridge = null;
    private Quaternion targetJoyConRot = Quaternion.identity;
    private Quaternion deferredJoyConRot = Quaternion.identity;
    private float timer = 0.0f;
    private Queue<GameObject> passedFloorQueue = null;
    private List<float> dashEnergyList = null;
    private bool shouldCheckUnstable = false;
    private bool shouldCountUnstableTimer = false;
    private Quaternion passBridgeBenchmarkQuat = Quaternion.identity;
    private int prevFullHoneyCount = 0;
    private int frameCount = 0;
    private List<MarathonPlayerStatus> negativeStatus = new List<MarathonPlayerStatus>
    { MarathonPlayerStatus.Dancing, MarathonPlayerStatus.BeKnockedBack, MarathonPlayerStatus.Dizzying };

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

        dashEnergyList = new List<float>(dashPointAmount);
        for (int i = 0; i < dashPointAmount; i++)
        {
            dashEnergyList.Add(0);
        }
        passedFloorQueue = new Queue<GameObject>();
        timer = 0.0f;
        passingBridge = null;
        deferredJoyConRot = targetJoyConRot;
        State = MarathonPlayerStatus.WarmingUp;
        presentMoveSpeed = runSpeed;
        rd = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();
        dizzyEffectObj.SetActive(false);
        shouldCheckUnstable = false;
        shouldCountUnstableTimer = false;
        passBridgeBenchmarkQuat = Quaternion.identity;
        prevFullHoneyCount = 0;
        frameCount = 0;
        Debug.Assert(unstableRemedyTime < unstableHoldTime);
    }

    private void Start()
    {
        targetJoyConRot = JoyConInputManager.GetJoyConOrientation(isUsingLeftJoyCon);
        RecenterPlayerJoyCon();
    }

    private void OnDestroy()
    {
        Instance = null;
        Resources.UnloadUnusedAssets();
    }

    private void Update()
    {
        #region Development
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Utility.ReloadCurrentScene();
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            Utility.MoveNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            for (int i = 0; i < dashEnergyList.Count; i++)
            {
                dashEnergyList[i] = dashPointCapacity;
            }
        }
        #endregion
        
        // If fell out main ground
        if (transform.position.y < landBaseline)
        {
            if (!AudioManager.Instance.CheckIsPlaying("Fall"))
            {
                AudioManager.Instance.Play("Fall");
            }
            SetPlayerState(MarathonPlayerStatus.Falling);
        }

        switch (State)
        {
            case MarathonPlayerStatus.WarmingUp:
                break;
            case MarathonPlayerStatus.Running:
                bool isTriggered = JoyConInputManager.GetJoyConButtonDown(Joycon.Button.SHOULDER_2) ||
                                   Input.GetButtonDown("Fire1");
                if (dashEnergyList[0] == 1.0f && isTriggered)
                {
                    dashEnergyList.RemoveAt(0);
                    dashEnergyList.Add(0.0f);
                    honeyMarkHolder.RefreshHoneyMarks(dashEnergyList);
                    SetPlayerState(MarathonPlayerStatus.Dashing);
                }
                MovePlayer(runSpeed);
                RotatePlayer();
                break;
            case MarathonPlayerStatus.Dashing:
                Dash();
                break;
            case MarathonPlayerStatus.Falling:
                Fall();
                RotatePlayer();
                break;
            case MarathonPlayerStatus.BeKnockedBack:
                BeKnockedBack();
                break;
            case MarathonPlayerStatus.Dizzying:
                if (timer >= dizzyingTimeLength)
                {
                    SetPlayerState(MarathonPlayerStatus.Running);
                }
                else
                {
                    timer += Time.deltaTime;
                }
                break;
            case MarathonPlayerStatus.Dancing:
                if (timer >= dancingTimeLength)
                {
                    SetPlayerState(MarathonPlayerStatus.Running);
                }
                else
                {
                    timer += Time.deltaTime;
                }
                break;
            case MarathonPlayerStatus.PassingBridge:
                PassBridge();
                break;
            case MarathonPlayerStatus.HoldUp:
                if (timer >= afterResetHoldUpTimeLength)
                {
                    SetPlayerState(MarathonPlayerStatus.Running);
                }
                else
                {
                    timer += Time.deltaTime;
                }
                RotatePlayer();
                break;
            case MarathonPlayerStatus.ReachGoal:
                Vector3 relativePos = Camera.main.transform.position - transform.position;
                Quaternion rot = Quaternion.LookRotation(relativePos);
                rot = Quaternion.Euler(0.0f, rot.eulerAngles.y, 0.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      rot,
                                                      rotToCameraSpeed * Time.deltaTime);
                break;
        }

        // Set animation
        GetComponent<Animator>().SetInteger("playerState", (int)State);
    }

    #region OnCollisionProcess
    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<IReactionable>()?.OnEnter(this);

        // Save floor info
        if (collision.gameObject.tag == "Floor")
        {
            if (passedFloorQueue.Count == 3) passedFloorQueue.Dequeue();
            passedFloorQueue.Enqueue(collision.gameObject);
        }
        else
        {
            JoyConInputManager.TriggerRumble(true, 320, 160, 0.6f, 200);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        collision.gameObject.GetComponent<IReactionable>()?.OnStay(this);
    }

    private void OnCollisionExit(Collision collision)
    {
        collision.gameObject.GetComponent<IReactionable>()?.OnExit(this);
    }
    #endregion

    #region OnTriggerProcess
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IReactionable>()?.OnEnter(this);
    }
    #endregion

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<IReactionable>()?.OnExit(this);
    }

    //=====================================================================================================
    // Private added functions
    //=====================================================================================================
    private void MovePlayer(float speed)
    {
        rd.velocity = transform.forward * speed;
    }

    private void RotatePlayer()
    {
        #region KeyboardInput
        var horizontalInput = Input.GetAxis("Keyboard_Horizontal");
        if (horizontalInput != 0.0f)
        {
            var newQuat = Quaternion.Euler(0.0f,
                                           horizontalInput * keyboardRotSpeed * Time.deltaTime,
                                           0.0f);
            rd.MoveRotation(newQuat * rd.rotation);
            return;
        }
        #endregion

        #region JoyConInput
        // Recenter
        if (JoyConInputManager.GetJoyConButtonDown(Joycon.Button.SHOULDER_1))
        {
            RecenterPlayerJoyCon();
        }

        // Clamp
        ++frameCount;
        Quaternion joyConFixQuat = Quaternion.Euler(0.0f, JoyConRotFixAngle * Mathf.Pow(frameCount, JoyConRotFixPower) * Time.deltaTime, 0.0f);
        targetJoyConRot = joyConFixQuat * JoyConInputManager.GetJoyConOrientation(isUsingLeftJoyCon);
        if (targetJoyConRot != Quaternion.identity)
        {
            deferredJoyConRot = Quaternion.RotateTowards(deferredJoyConRot,
                                                     targetJoyConRot, joyConRotSpeed * Time.deltaTime);

            var joyConDir = deferredJoyConRot * Vector3.up;
            if (Vector3.Angle(joyConDir, Vector3.forward) > angleLimit)
            {
                return;
            }

            // Add gyro data to player model rotation
            var initiRot = transform.rotation.eulerAngles;
            var tempQuat = deferredJoyConRot * Quaternion.Euler(90.0f, 0.0f, 0.0f);
            transform.rotation = Quaternion.Euler(initiRot.x, tempQuat.eulerAngles.y, initiRot.z);
            transform.rotation *= Quaternion.AngleAxis(180.0f, Vector3.up);
        }
        #endregion
    }

    private void PassBridge()
    {
        if (!shouldCountUnstableTimer)
        {
            Debug.Assert(passingBridge != null);
            MovePlayer(passBridgeSpeed);
            transform.position = new Vector3(passingBridge.transform.position.x,
                                             transform.position.y, transform.position.z);
            transform.localRotation = Quaternion.identity;
        }

        #region UnstableProcess
        if (!shouldCheckUnstable) return;
        var joyConAccel = JoyConInputManager.GetJoyConAccel(isUsingLeftJoyCon);
        bool isAccelUnstable = joyConAccel.magnitude - 1.0f > accelTolerance ||
                               Input.GetAxis("Keyboard_Horizontal") != 0.0f;
        var joyConYRot = JoyConInputManager.GetJoyConOrientation(isUsingLeftJoyCon).eulerAngles.y;
        bool isAngleUnstable = Quaternion.Dot(passBridgeBenchmarkQuat,
                                              JoyConInputManager.GetJoyConOrientation(isUsingLeftJoyCon))
                                              < angleTolerance;

        if (isAccelUnstable || isAngleUnstable)
        {
            shouldCountUnstableTimer = true;
            ani.SetBool("isBridgeUnstable", true);
        }

        if (shouldCountUnstableTimer)
        {
            timer += Time.deltaTime;
        }

        if (timer >= unstableHoldTime)
        {
            timer = 0.0f;
            shouldCountUnstableTimer = false;
            ani.SetBool("isBridgeUnstable", false);
        }
        else
        {
            if (timer < unstableRemedyTime) return;
            if (isAccelUnstable || isAngleUnstable)
            {
                timer = 0.0f;
                shouldCountUnstableTimer = false;
                ani.SetBool("isBridgeUnstable", false);
                SetPlayerState(MarathonPlayerStatus.Falling);
                passingBridge.BeforePlayerFelledOut(this);
                Vector3 dropDir = UnityEngine.Random.value > 0.5f ?
                                  transform.right : -transform.right;
                rd.AddForce(dropDir * fellOutForce, ForceMode.Impulse);
            }
        }
        #endregion
    }

    /// <summary>
    /// Behaviour when falling. No collider detect when falling
    /// (Consider felling out bridge. Bridge collider will drag player back to bridge)
    /// </summary>
    private void Fall()
    {
        if (timer >= fallingTimeLength)
        {
            ResetPlayerOnGround();
            timer = 0.0f;
        }
        else
        {
            timer += Time.deltaTime;
            var colliders = GetComponents<Collider>();
            foreach (var i in colliders) i.enabled = false;
        }
    }

    private void ResetPlayerOnGround()
    {
        rd.velocity = Vector3.zero;

        // If drop from bridge, land to bridge. Otherwise land to floor
        Vector3 resetPosition;
        if (passingBridge != null)
        {
            resetPosition = passingBridge.playerFellOutPosition + resetPositionOffset;
            passingBridge.OnPlayerReturn();
        }
        else
        {
            GameObject farestFloor = passedFloorQueue.Peek();
            foreach (var i in passedFloorQueue)
            {
                if (i.transform.position.z < farestFloor.transform.position.z)
                {
                    farestFloor = i;
                }
            }
            resetPosition = farestFloor.transform.position;
        }

        var colliders = GetComponents<Collider>();
        foreach (var i in colliders) i.enabled = true;
        transform.localPosition = resetPosition + resetPositionOffset;
        transform.localRotation = Quaternion.identity;
        SetPlayerState(MarathonPlayerStatus.HoldUp);
    }

    private void Dash()
    {
        MovePlayer(dashSpeed);
        RotatePlayer();

        if (timer >= dashTime)
        {
            SetPlayerState(MarathonPlayerStatus.Running);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private void BeKnockedBack()
    {
        if (timer < BeKnockedBackTimeLength)
        {
            timer += Time.deltaTime;
            rd.velocity = -transform.forward * rollingBackwardSpeed * Time.deltaTime;
        }
        else
        {
            SetPlayerState(MarathonPlayerStatus.Running);
        }
    }

    private void StartCheckUnstable()
    {
        shouldCheckUnstable = true;
        passBridgeBenchmarkQuat = RecenterPlayerJoyCon();
    }

    //=====================================================================================================
    // Public functions
    //=====================================================================================================
    public void SetPlayerState(MarathonPlayerManager.MarathonPlayerStatus newState)
    {
        if (State == MarathonPlayerStatus.ReachGoal) return;

        if (negativeStatus.Contains(newState) && negativeStatus.Contains(State))
        {
            return;
        }
        else
        {
            // Old state
            switch (State)
            {
                case MarathonPlayerStatus.Dashing:
                    AudioManager.Instance.Stop("Dashing");
                    break;
                case MarathonPlayerStatus.Dizzying:
                    dizzyEffectObj.SetActive(false);
                    break;
                case MarathonPlayerStatus.PassingBridge:
                    shouldCheckUnstable = false;
                    ani.SetBool("isBridgeUnstable", false);
                    break;
                case MarathonPlayerStatus.ReachGoal:
                    break;
            }

            // New state
            switch (newState)
            {
                case MarathonPlayerStatus.Dashing:
                    AudioManager.Instance.Play("Dashing");
                    break;
                case MarathonPlayerStatus.Dizzying:
                    {
                        AudioManager.Instance.Play("Dizzy");
                        rd.velocity = Vector3.zero;
                        dizzyEffectObj.SetActive(true);
                        var parSystem = dizzyEffectObj.GetComponent<ParticleSystem>();
                        if (!parSystem.isPlaying) parSystem.Play();
                    }
                    break;
                case MarathonPlayerStatus.Falling:
                    if (State == MarathonPlayerStatus.Falling) return;
                    AudioManager.Instance.Play("Falling");
                    break;
                case MarathonPlayerStatus.PassingBridge:
                    Invoke("StartCheckUnstable", checkUnstableAfterSec);
                    break;
            }

            State = newState;
            timer = 0.0f;
        }
    }

    public int GetDashPointCount()
    {
        return dashPointAmount;
    }

    public void AddDashEnergy(float added)
    {
        // Index of first element that not full 
        int baseIndex = 0;
        for (; baseIndex < dashEnergyList.Count; baseIndex++)
        {
            if (dashEnergyList[baseIndex] != 1.0f) break;
        }

        if (baseIndex < dashEnergyList.Count)
        {
            float total = dashEnergyList[baseIndex] + added;
            if (total > dashPointCapacity)
            {
                added -= (dashPointCapacity - dashEnergyList[baseIndex]);
                dashEnergyList[baseIndex] = 1.0f;
                float time = added / dashPointCapacity;
                for (int i = 0; i < Mathf.CeilToInt(time); i++)
                {
                    if (baseIndex + i > dashEnergyList.Count) break;
                    if (i < Mathf.CeilToInt(time))
                    {
                        dashEnergyList[baseIndex + i] = dashPointCapacity;
                    }
                    else
                    {
                        dashEnergyList[baseIndex + i] = time - Mathf.Floor(time);
                    }
                }
            }
            else
            {
                dashEnergyList[baseIndex] += added;
            }
        }
        
        honeyMarkHolder.RefreshHoneyMarks(dashEnergyList);

        // Sound hint
        int presentFullHoneyCount = 0;
        foreach (var i in dashEnergyList)
        {
            if (i == dashPointCapacity) ++presentFullHoneyCount;
        }
        if (presentFullHoneyCount > prevFullHoneyCount)
        {
            AudioManager.Instance.Play("HoneyReady");
        }
        prevFullHoneyCount = presentFullHoneyCount;
    }

    public void UpdateBridgeInfo(Bridge b = null)
    {
        passingBridge = b;
    }

    public Quaternion RecenterPlayerJoyCon()
    {
        frameCount = 0;
        return JoyConInputManager.Recenter(isUsingLeftJoyCon);
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public void OnGameStart()
    {
        SetPlayerState(MarathonPlayerStatus.Running);
        RecenterPlayerJoyCon();
    }

    public void OnPlayerReachGoal()
    {
        rd.velocity = Vector3.zero;
        AudioManager.Instance.Play("SceneEnd");
        if (State == MarathonPlayerStatus.Falling)
        {
            ResetPlayerOnGround();
        }
        SetPlayerState(MarathonPlayerStatus.ReachGoal);
    }
}