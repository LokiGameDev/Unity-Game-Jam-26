using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            Debug.Log("Quitting application");
        #elif UNITY_EDITOR
            Debug.Log("Quitting application");
        #else
            Application.Quit();
        #endif
    }
}
