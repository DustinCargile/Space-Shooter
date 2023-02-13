using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    // Start is called before the first frame update
    void Start()
    {
        _score = 0;
        UpdateScore();
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
    }
}
