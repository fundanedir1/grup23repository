using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        // Build'te calisir: oyunu kapatir
        Application.Quit();

        // Eger editorda test ediyorsan asagidakini devre disi birakma ama
        // bu satir sadece editorde calissin:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
