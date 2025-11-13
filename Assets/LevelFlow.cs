using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFlow : MonoBehaviour
{
    [Header("Scene Flow")]
    public string nextSceneName = "";   // Sätt Level_2 i scen 1, Level_3_Boss i scen 2

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        if (string.IsNullOrEmpty(nextSceneName)) return;
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    // Anropas när vi vill visa WIN manuellt (t.ex. efter boss-död)
    public void ShowWinPanel(GameObject winPanel)
    {
        if (winPanel) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }
}
