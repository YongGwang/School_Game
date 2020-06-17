using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetStageIcon : MonoBehaviour, IJoyConSelectable
{
    public void OnJoyConClicked()
    {
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}