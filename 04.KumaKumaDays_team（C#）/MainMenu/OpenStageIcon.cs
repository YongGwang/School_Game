using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenStageIcon : MonoBehaviour, IJoyConSelectable
{
    public enum GameStage
    {
        MainMenu,
        BalloonGame,
        MarathonGame,
        FishGame,
        None
    }

    public GameStage TargetStage = GameStage.None;

    public void OnJoyConClicked()
    {
        Debug.Assert(TargetStage != GameStage.None, gameObject.name);
        SceneManager.LoadScene((int)TargetStage, LoadSceneMode.Single);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}