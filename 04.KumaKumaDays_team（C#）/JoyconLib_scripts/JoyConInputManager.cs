using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// via: https://tech.mof-mof.co.jp/blog/unity-joycon-introduce.html
/// Manage joycon input for detect key press and joycon movment;
/// </summary>
/// History Log:
///     2019/12/14(Sa) Initial
///     2019/01/12(Sa) Add function get pressed button
///                    Add function that recenter joyCon
///     2020/01/21(Sa) Hide cursor
///     2020/02/03(Sa) Fixed getKeyDown function
///     2020/02/25(Sa) DonotDestoryAfterLoadScene
public class JoyConInputManager : MonoBehaviour
{
	private 
        readonly Joycon.Button[] m_buttons =
		Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

	private List<Joycon> m_joycons;
	private static Joycon m_joyconL;
	private static Joycon m_joyconR;
	private static Joycon.Button? m_pressedButtonL;
	private static Joycon.Button? m_pressedButtonR;
    static JoyConInputManager instance;
    public static JoyConInputManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
	{
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetControllers();
	}

    private void Update()
	{
		m_pressedButtonL = null;
		m_pressedButtonR = null;

		if (m_joycons == null || m_joycons.Count <= 0) return;

		SetControllers();

		foreach (var button in m_buttons)
		{
			if (m_joyconL != null && m_joyconL.GetButton(button))
			{
				m_pressedButtonL = button;
			}
			if (m_joyconR != null && m_joyconR.GetButton(button))
			{
				m_pressedButtonR = button;
			}
		}

		if (Input.GetKeyDown(KeyCode.Z))
		{
			m_joyconL.SetRumble(320, 160, 0.6f, 200);
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			m_joyconR.SetRumble(160, 320, 0.6f, 200);
		}
	}

#if UNITY_EDITOR
    private void OnGUI()
    {
        var style = GUI.skin.GetStyle("label");
        style.fontSize = 24;

        GUILayout.BeginHorizontal(GUILayout.Width(960));
        foreach (var joycon in m_joycons)
        {
            var isLeft = joycon.isLeft;
            var name = isLeft ? "Joy-Con (L)" : "Joy-Con (R)";
            var key = isLeft ? "Z キー" : "X キー";
            var button = isLeft ? m_pressedButtonL : m_pressedButtonR;
            var stick = joycon.GetStick();
            var gyro = joycon.GetGyro();
            var accel = joycon.GetAccel();
            var orientation = joycon.GetVector();

            GUILayout.BeginVertical(GUILayout.Width(480));
            GUILayout.Label(name);
            GUILayout.Label(key + "：振動");
            GUILayout.Label("押されているボタン：" + button);
            GUILayout.Label(string.Format("スティック：({0}, {1})", stick[0], stick[1]));
            GUILayout.Label("ジャイロ：" + gyro);
            GUILayout.Label("加速度：" + accel);
            GUILayout.Label("傾き：" + orientation.eulerAngles);
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
#endif

    private void SetControllers()
	{
		m_joycons = JoyconManager.Instance.j;
		if (m_joycons == null || m_joycons.Count <= 0) return;
		m_joyconL = m_joycons.Find(c => c.isLeft);
		m_joyconR = m_joycons.Find(c => !c.isLeft);
	}

	#region API
	public static Vector3 GetJoyConAccel(bool isLeftJoyCon)
	{
		if (isLeftJoyCon && m_joyconL != null) return m_joyconL.GetAccel();
		else if (!isLeftJoyCon && m_joyconR != null) return m_joyconR.GetAccel();
		else return Vector3.zero;
	}

	public static Vector3 GetJoyConGyro(bool isLeftJoyCon)
	{
		if (isLeftJoyCon && m_joyconL != null) return m_joyconL.GetGyro();
		else if (!isLeftJoyCon && m_joyconR != null) return m_joyconR.GetGyro();
		else return Vector3.zero;
	}

	public static Quaternion GetJoyConOrientation(bool isLeftJoyCon)
	{
		if (isLeftJoyCon && m_joyconL != null) return m_joyconL.GetVector();
		else if (!isLeftJoyCon && m_joyconR != null) return m_joyconR.GetVector();
		else return Quaternion.identity;
	}

	public static float[] Getstick(bool isLeftJoyCon)
	{

		if (isLeftJoyCon && m_joyconL != null) return m_joyconL.GetStick();
		else if (!isLeftJoyCon && m_joyconR != null) return m_joyconR.GetStick();
		else return new float[]{ 0, 0 };
	}

    public static bool GetJoyConButton(Joycon.Button button, bool isUsingRightJoyCon = true)
	{
        if (isUsingRightJoyCon)
        {
            return m_joyconR.GetButton(button);
        }
        else
        {
            return m_joyconL.GetButton(button);
        }
	}

    public static bool GetJoyConButtonDown(Joycon.Button button, bool isUsingRightJoyCon = true)
    {
        if (isUsingRightJoyCon)
        {
            if (m_joyconR == null)  return false;
            else                    return m_joyconR.GetButtonDown(button);
        }
        else
        {
            if (m_joyconL == null)  return false;
            else                    return m_joyconL.GetButtonDown(button);
        }
    }

	public static bool GetJoyConAnykeyDown()
	{
        if (m_pressedButtonL != null)
        {
            Joycon.Button b = (Joycon.Button)m_pressedButtonL;
            return m_joyconL.GetButtonDown(b);
        }
        else if (m_pressedButtonR != null)
        {
            Joycon.Button b = (Joycon.Button)m_pressedButtonR;
            return m_joyconR.GetButtonDown(b);
        }
        else
        {
            return false;
        }
    }

    public static bool GetJoyConAnyKey()
    {
        return m_pressedButtonL != null || m_pressedButtonR != null;
    }

	public static bool isUsingJoyCon()
	{
		return m_joyconL != null || m_joyconR != null;
	}

	public static bool isUsingBothJoyCons()
	{
		return m_joyconL != null && m_joyconR != null;
	}

	public static bool IsUsingLeftJoyCon()
	{
		if (m_joyconL != null) return true;
		else return false;
	}

	public static bool IsUsingRightJoyCon()
	{
		if (m_joyconR != null) return true;
		else return false;
	}

    public static Quaternion Recenter(bool isLeftJoyCon)
    {
		if (m_joyconL == null && m_joyconR == null)
		{
			Debug.LogWarning("Abort Recenter: No joycon found");
			return Quaternion.identity;
		}

		if (isLeftJoyCon) m_joyconL?.Recenter();
        else m_joyconR?.Recenter();
        return GetJoyConOrientation(isLeftJoyCon);
    }

    /// <summary>
    /// Set rumble of joyCon
    /// </summary>
    /// <param name="isLeftJoyCon">Is left hand joyCon?</param>
    /// <param name="fna">Low_freq, high_freq and amp</param>
    /// <param name="time">[Optinal]How long? in ms</param>
    public static void TriggerRumble(bool isLeftJoyCon, float low_freq,
		float high_freq, float amp, int time)
	{
		if (isLeftJoyCon && m_joyconL != null) m_joyconL.SetRumble(low_freq,
			high_freq, amp, time);
		else if (!isLeftJoyCon && m_joyconR != null) m_joyconR.SetRumble(low_freq,
			high_freq, amp, time);
	}

	#endregion
}