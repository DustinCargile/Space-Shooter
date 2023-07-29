using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _infoPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGame() 
    {
        SceneManager.LoadScene("Game");
    }
    public void ShowInfoPanel() 
    {
        _infoPanel.SetActive(true);
    }
    public void HideInfoPanel() 
    {
        _infoPanel?.SetActive(false);
    }
    public void ExitGame() 
    {
        Application.Quit();
    }
}
