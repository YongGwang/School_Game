using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Read sprite from resource folder
/// </summary>
/// =======================================================
/// Author : 2019/01/28 Sa
/// History Log :
///		2019/01/28(Sa) Initial.
///		2019/02/26(Sa) Add function for spawning obj with
///		               image component.
public class UIResourceController : MonoBehaviour
{
    private static Sprite[] numberImage = null;

    private void Awake()
    {
        numberImage = Resources.LoadAll<Sprite>("Images/Number/");
    }

    public static List<Sprite> GetNumImage(string num)
    {
        Debug.Assert(numberImage != null);
        List<Sprite> list = new List<Sprite>();
        foreach (var i in num)
        {
            // i is '+' or '-'
            if(char.IsSymbol(i))
            {
                if (i == '+')       list.Add(numberImage[11]);
                else if (i == '-')  list.Add(numberImage[10]);
            }
            else if (char.IsNumber(i))
            {
                list.Add(numberImage[(int)char.GetNumericValue(i)]);
            }
        }
        return list;
    }

    public static List<GameObject> DisplayNumImage(Transform rootRectTrans,
                                                   string num,
                                                   Color textCol,
                                                   float textSpaceInterval,
                                                   bool isOneOffObj = false,
                                                   string objName = "NumElement")
    {
        List<GameObject> spawnedObjs = new List<GameObject>();
        var sprites = GetNumImage(num);

        // Lambda methond. Spawn obj that be setted. Return imageComp of spawned new obj.
        System.Func<Vector3, Image> SpawnUIObj = (localPos) =>
        {
            var newObj = Instantiate(new GameObject(objName), rootRectTrans, false);
            var transComp = newObj.AddComponent<RectTransform>();
            transComp.localPosition = localPos;
            var imageComp = newObj.AddComponent<Image>();
            imageComp.color = textCol;
            return imageComp;
        };

        if (isOneOffObj)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                var imageComp = SpawnUIObj(new Vector3(i * textSpaceInterval, 0.0f, 0.0f));
                spawnedObjs.Add(imageComp.gameObject);
                imageComp.sprite = sprites[i];
            }
        }
        else
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                Image imageComp = null;
                if (i > rootRectTrans.childCount - 1)
                {
                    imageComp = SpawnUIObj(new Vector3(i * textSpaceInterval, 0.0f, 0.0f));
                }
                else
                {
                    imageComp = rootRectTrans.GetChild(i).GetComponent<Image>();
                }

                imageComp.sprite = sprites[i];
                spawnedObjs.Add(imageComp.gameObject);
            }
        }

        return spawnedObjs;
    }
}