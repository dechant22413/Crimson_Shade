using UnityEngine;

public class GameplaySceneUI : MonoBehaviour
{
    public void LoadScene(int sceneIndex)
    {
        Time.timeScale = 1f;
        SceneTransitionManager.Instance.LoadScene(sceneIndex);
    }
}