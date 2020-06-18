using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Switch blocklines positon base on player position
/// </summary>
/// =======================================================
/// Author : 2020/02/23(Sa)
/// History Log :
///		2020/02/23(Sa) Initial
///		2020/02/24(Sa) Add spawn bridge process
public class MarathonTutGroundManager : MonoBehaviour
{
    [Header("SpawnBehaviour")]
    [SerializeField] float blockSpaceInterval = 4.0f;
    [SerializeField] Vector3 bridgePositionOffset = Vector3.zero;
    [SerializeField] int blockBetweenBridge = 4;
    [SerializeField] int alertLastIndex = 3;

    [Header("Items")]
    [SerializeField] GameObject[] itmesLists = null;
    [Range(0.0f, 1.0f)]
    [SerializeField] float itemSpawnProp = 0.45f;
    [SerializeField] Vector3 itemSpawnOffset = Vector3.zero;

    [Header("Bridge")]
    [SerializeField] GameObject bridgeBlockObj = null;

    private List<GameObject> lines = new List<GameObject>();
    private int blockAfterBridgeCount = 0;
    private bool haveBridgeBehind = false;
    private bool shouldMoveBridge = false;

    private void Awake()
    {
        shouldMoveBridge = false;
        haveBridgeBehind = false;
        blockAfterBridgeCount = 0;
        lines = new List<GameObject>();
        foreach (Transform i in transform)
        {
            lines.Add(i.gameObject);
        }
        Debug.Assert(lines.Count > alertLastIndex);
        Debug.Assert(blockBetweenBridge <= lines.Count - 1);
        lines[lines.Count - alertLastIndex].GetComponent<BlockLine>().
                                            OnPlayerEnter += OnPlayerEnterLastLine;
    }

    private void OnPlayerEnterLastLine()
    {
        // Switch block position
        lines[lines.Count - alertLastIndex].GetComponent<BlockLine>().
                                            OnPlayerEnter -= OnPlayerEnterLastLine;
        Vector3 offsetPosition;
        if (haveBridgeBehind)
        {
            offsetPosition = new Vector3(0.0f, 0.0f, bridgePositionOffset.z * 2.0f);
            haveBridgeBehind = false;
        }
        else
        {
            offsetPosition = new Vector3(0.0f, 0.0f, blockSpaceInterval);
        }
        lines[0].transform.position = lines[lines.Count - 1].transform.position + offsetPosition;
        lines.Add(lines[0]);
        lines.RemoveAt(0);
        lines[lines.Count - alertLastIndex].GetComponent<BlockLine>().
                                            OnPlayerEnter += OnPlayerEnterLastLine;

        // Move bridge
        if (MarathonGameManager.Instance.isBridgeTut)
        {   
            if (!shouldMoveBridge)
            {
                blockAfterBridgeCount++;
                if (blockAfterBridgeCount >= blockBetweenBridge)
                {
                    shouldMoveBridge = true;
                }
            }
            else
            {
                bridgeBlockObj.transform.position = lines[lines.Count - 1].transform.position +
                                                    bridgePositionOffset;
                blockAfterBridgeCount = 0;
                haveBridgeBehind = true;
                shouldMoveBridge = false;
            }
        }


        // Spawn item?
        if (MarathonGameManager.Instance.isBridgeTut) return;
        if (UnityEngine.Random.value > itemSpawnProp)
        {
            var itemIndex = UnityEngine.Random.Range(0, itmesLists.Length);
            var targetTrans = lines[lines.Count - 1].transform;
            var posChildIndex = UnityEngine.Random.Range(0, targetTrans.childCount - 1);
            Instantiate(itmesLists[itemIndex],
                        targetTrans.GetChild(posChildIndex).position + itemSpawnOffset,
                        Quaternion.identity);
        }
    }
}