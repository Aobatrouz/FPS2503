using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gamemanager.instance.StateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.StateUnpause();
    }

    public void quit()
    {
    #if UNITY_EDITOR
             UnityEditor.EditorApplication.isPlaying = false;
    #else
             Application.Quit();
    #endif
    }
}
