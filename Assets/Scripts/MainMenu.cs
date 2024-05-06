using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SavedData savedData;

    public void StartNewGame()
    {
        SceneManager.LoadScene(1);
        savedData.isNewGame = true;
    }

    public void ContinueGame()
    {
        savedData.isNewGame = false;
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
