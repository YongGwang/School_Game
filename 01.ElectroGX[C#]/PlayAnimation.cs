using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
	public const string DEFAULT_FACE = "default@sd_hmd";

	[Tooltip("表情のアニメーションクリップを設定")]
	public AnimationClip[] animations;

	//アニメーションイベントから呼び出される表情切り替え用のコールバック
	public void OnCallChangeFace(string str)
	{
		int ichecked = 0;
		foreach (var animation in animations)
		{
			if (str == animation.name)
			{
				ChangeFace(str);
				break;
			}
			else if (ichecked <= animations.Length)
			{
				ichecked++;
			}
			else
			{
				//str指定が間違っている時にはデフォルトの表情に設定
				ChangeFace(DEFAULT_FACE);
			}
		}
	}
}
