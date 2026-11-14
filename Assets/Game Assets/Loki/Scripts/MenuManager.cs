using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        AudioManager.Instance.PlayMusic(AudioID.BGM);
    }
    public void StartGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
