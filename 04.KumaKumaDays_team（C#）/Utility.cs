using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Some sweet utility
/// </summary>
/// Author :  2019/02/13 Sa
/// =============================================
/// History Log :
///		2019/02/13(Sa) Initial
public class Utility : MonoBehaviour
{
	/// <summary>
	/// If at the last scene, move to first scene
	/// </summary>
	public static void MoveNextScene()
	{
		SceneManager.LoadScene(GetNextSceneIndex(), LoadSceneMode.Single);
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}

    /// <summary>
    /// Use this in Start mothed, not Awake
    /// </summary>
    /// <returns></returns>
    public static AsyncOperation MoveNextSceneAsync()
    {
        var async = SceneManager.LoadSceneAsync(GetNextSceneIndex());
        async.allowSceneActivation = false;
        return async;
    }

    public static AsyncOperation MoveSceneAsync(int index)
    {
        var async = SceneManager.LoadSceneAsync(index);
        async.allowSceneActivation = false;
        return async;
    }

    public static int GetNextSceneIndex()
    {
        return (SceneManager.GetActiveScene().buildIndex + 1) %
                SceneManager.sceneCountInBuildSettings;
    }

	public static void ReloadCurrentScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}

    public static int GetCurretSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

	public static void MoveObjByWASD(Transform t, float s)
	{
		Vector3 moveDir = Vector3.zero;
		if (Input.GetKey(KeyCode.W)) moveDir += Vector3.forward;
		if (Input.GetKey(KeyCode.S)) moveDir -= Vector3.forward;
		if (Input.GetKey(KeyCode.A)) moveDir += Vector3.left;
		if (Input.GetKey(KeyCode.D)) moveDir -= Vector3.left;
		t.position += moveDir * s * Time.deltaTime;
	}

	public static void ResetInNearestPath(Transform t, Cinemachine.CinemachinePathBase p)
	{
		float pathPosition = p.FindClosestPoint(t.position, 0, -1, 10);
		t.position = p.EvaluatePosition(pathPosition);
	}
}