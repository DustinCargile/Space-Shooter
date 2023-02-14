using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    private int _score;
    [SerializeField]
    private Player player;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private Image _livesImage;

    private bool _isGameOver = false;
    
    [SerializeField]
    private GameManager _gameManager;

    [SerializeField]
    private TextMeshProUGUI _gameOverText;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _score = 0;
        UpdateScore();
        _gameOverText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdjustScore(int val) 
    {
        _score += val;
        player.AdjustScore(_score);
        UpdateScore();
    }
    public void SetScore(int val) 
    {
        _score = val;
        UpdateScore();
    }
    void UpdateScore() 
    {
        _scoreText.text = "Score: " + _score;
    }
    public void UpdateLives(int currentLives) 
    {
        _livesImage.sprite = _livesSprites[currentLives];
        if (currentLives == 0) 
        {
            GameOver();
        }
    }
    void GameOver() 
    {
        if (_gameManager == null) 
        {
            Debug.Log("GameManager not found!");
        }
        _gameManager.GameOver();
        _isGameOver = true;
        StartCoroutine(GameOverFlickerRoutine());
    }
    IEnumerator GameOverFlickerRoutine() 
    {
        
        while (_isGameOver) 
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(.5f);
        }
    }

}
