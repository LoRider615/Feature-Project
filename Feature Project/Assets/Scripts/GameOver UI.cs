using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void onRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void OnQuit()
    {
        Application.Quit();
    }
}
