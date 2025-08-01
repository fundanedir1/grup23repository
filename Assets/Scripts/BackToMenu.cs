using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMenu : MonoBehaviour
{
    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenuScene"); // menu sahnenizin adini buraya yazin
    }
}
