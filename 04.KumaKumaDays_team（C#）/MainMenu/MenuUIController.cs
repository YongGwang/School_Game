using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Process about main menu UI and input
/// </summary>
/// =======================================================
/// Author : 2019/01/12 Sa
/// History Log :
///		2019/01/12(Sa) Initial
public class MenuUIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> iconObjs = new List<GameObject>();
    [SerializeField] private bool isUsingLeftJoyCon = false;
    [SerializeField] private GameObject pointerMark = null;
    [SerializeField] private float pointerMoveSpeed = 20.0f;
    [SerializeField] bool shouldReverseJoyConY = false;
    [SerializeField] private Vector2 pointerAdjuster = Vector2.zero;

    private Vector3 joyConPointDir = Vector3.zero;
    private PointerEventData eventDataCurrentPosition;
    private List<RaycastResult> results;
    private bool isDirtyInitilized = false;

    #region InheritMethod
    private void Awake()
    {
        results = new List<RaycastResult>();
        joyConPointDir = Vector3.zero;
        eventDataCurrentPosition = new PointerEventData(EventSystem.current);
    }

    private void Update()
    {
        if (!isDirtyInitilized)
        {
            JoyConInputManager.Recenter(isUsingLeftJoyCon);
            isDirtyInitilized = true;
        }
        
        // Calc the point of intersection between joyConDirection and UIScreen
        var joyConQuat = JoyConInputManager.GetJoyConOrientation(isUsingLeftJoyCon);
        joyConPointDir = joyConQuat * Camera.main.transform.up;
        if (joyConPointDir.z < 0) return;
        if (shouldReverseJoyConY)
        {
            joyConPointDir = new Vector3(joyConPointDir.x, -joyConPointDir.y, joyConPointDir.z).normalized;
        }
        else
        {
            joyConPointDir = joyConPointDir.normalized;
        }

        joyConPointDir = Camera.main.WorldToScreenPoint(joyConPointDir * pointerMoveSpeed
            + Camera.main.transform.position) + new Vector3(pointerAdjuster.x, pointerAdjuster.y, 0);

        // Detect hit
        eventDataCurrentPosition.position = joyConPointDir;
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (results.Count > 0)
        {
            bool hasIcon = false;
            foreach (var i in results)
            {
                var interfaceComp = i.gameObject.GetComponent<IJoyConSelectable>();
                // If not seletable icon
                if (interfaceComp == null)
                {
                    if (i.gameObject.name == "BG")
                    {
                        pointerMark.transform.position = i.screenPosition;
                    }
                }
                else
                {
                    i.gameObject.GetComponent<Image>().color = Color.red;
                    hasIcon = true;
                }
            }

            if (!hasIcon)
            {
                foreach (var i in iconObjs)
                {
                    i.GetComponent<Image>().color = Color.white;
                }
            }
        }

        HandleJoyConInput();
    }
    #endregion

    private void HandleJoyConInput()
    {
        // Recenter process
        if (JoyConInputManager.GetJoyConButtonDown(Joycon.Button.SHOULDER_1))
        {
            JoyConInputManager.Recenter(isUsingLeftJoyCon);
        }

        // Select icon process
        if (results.Count > 0 
            && JoyConInputManager.GetJoyConButtonDown(Joycon.Button.SHOULDER_2))
        {
            foreach (var i in results)
            {
                var interfaceComp = i.gameObject.GetComponent<IJoyConSelectable>();
                if (interfaceComp != null)
                {
                    interfaceComp.OnJoyConClicked();
                }
            }
        }
    }
}