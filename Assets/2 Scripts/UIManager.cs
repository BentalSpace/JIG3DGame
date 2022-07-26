using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject inputPanel;
    [SerializeField]
    ItemCreate createManager;
    public void GoMain() {
        SceneManager.LoadScene(0);
    }
    public void GameRetry() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameStartReady() {
        inputPanel.SetActive(true);
    }
    public void GameStartUnReady() {
        inputPanel.SetActive(false);
    }
    public void GameStart() {
        SceneManager.LoadScene(1);
        createManager.CntRemember();
        DontDestroyOnLoad(createManager);
    }
    public void DeveloperMode() {
        SceneManager.LoadScene(2);
    }
    public void GameExit() {
        Application.Quit();
    }
}
