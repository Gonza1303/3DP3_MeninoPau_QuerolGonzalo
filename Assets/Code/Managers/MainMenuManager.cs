using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadLevel()
    {
        AudioManager.Instance.StopMusic();
        SceneManager.LoadScene("Castle");
    }
}
