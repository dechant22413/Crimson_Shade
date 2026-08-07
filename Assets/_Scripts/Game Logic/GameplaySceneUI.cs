using UnityEngine;
using UnityEngine.Audio;

public class GameplaySceneUI : MonoBehaviour
{
    public void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1f;
        if(GameManager.Instance != null)
        GameManager.Instance.ResumeGame();
        SceneTransitionManager.Instance.LoadScene(sceneIndex);
    }
}