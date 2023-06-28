using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;
    private bool _isLevelComplete = false;

    private void Update()
    {
        if (_isGameOver && Input.GetKeyDown(KeyCode.R)) 
        {
            SceneManager.LoadScene(1);
        }
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            SceneManager.LoadScene(0);
        }
        if (_isLevelComplete && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }
    }
    public void GameOver() 
    {
        _isGameOver = true;
    }
    public void OnClickExit() 
    {
        Application.Quit();
    }
    public void LevelComplete() 
    {
        _isLevelComplete = true;
        
    }
}
