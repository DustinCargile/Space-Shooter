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
    private TextMeshProUGUI _gameOverText, _restartText;

    [SerializeField]
    private GameObject _ammoDisplay;
    [SerializeField]
    private TextMeshProUGUI _rocketAmmoText;

    [SerializeField]
    private TextMeshProUGUI _noAmmoText,_ammoText;

    [SerializeField]
    private GameObject _ammoUnit, _ammoGridUsed;


    #region Speed Cooldown Display
 
    [SerializeField]
    private Slider _boostSlider;

    [SerializeField]
    private TextMeshProUGUI _boostOverheatText,_boostNoFuelText;
    [SerializeField]
    private Color _boostGoodColor,_boostBadColor;
    [SerializeField]
    private Image _boostFill;

    //temp
    private bool _isChangingGood = false;
    private bool _isChangingBad = false;
    [SerializeField]
    private Timer _clock;
    #endregion
    #region Shield Levels
    [SerializeField]
    private Material _shieldLvl1Mat, _shieldLvl2Mat,_shieldLvl3Mat;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _score = 0;
        UpdateScore();
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        UpdateBoostCooldown(0);
        
        

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
    public void UpdateBoostCooldown(float currentBoost) 
    {
        
        _boostSlider.value = currentBoost;
    }
    void GameOver() 
    {
        if (_gameManager == null)
        {
            Debug.Log("GameManager not found!");
        }
        else 
        {
            _gameManager.GameOver();
        }
        
        _isGameOver = true;
        _restartText.gameObject.SetActive(true);
        
        StartCoroutine(GameOverFlickerRoutine());
    }
    public void UpdateAmmo(int currentAmmo) 
    {
        _ammoText.text = currentAmmo.ToString();
        if (currentAmmo > _ammoDisplay.transform.childCount) 
        {
            for (int i = 0; _ammoDisplay.transform.childCount < currentAmmo; i++) 
            {
                Instantiate(_ammoUnit,_ammoDisplay.transform);
            }
        }
        if (currentAmmo < _ammoDisplay.transform.childCount) 
        {
            Destroy(_ammoDisplay.transform.GetChild(0).gameObject);
        }
        if (!_noAmmoText.gameObject.activeSelf && currentAmmo == 0) 
        {
            _ammoGridUsed.SetActive(false);
            _noAmmoText.gameObject.SetActive(true);
            StartCoroutine(FadeInText(_noAmmoText, 2f));
        }
        if (currentAmmo > 0) 
        {
            _ammoGridUsed.SetActive(true);
            _noAmmoText.gameObject.SetActive(false);
        }
    }
    public void UpdateRocketAmmo(int currentAmmo)
    {
         _rocketAmmoText.text = currentAmmo.ToString();
    }
    public void LevelComplete() 
    {
        _restartText.gameObject.SetActive(true);
        _clock.StopTimer();
    }
    public void ActivateSpeedBoostOverheat() 
    {
        
        _boostOverheatText.gameObject.SetActive(true);
        _boostFill.gameObject.SetActive(false);
        StartCoroutine(FadeInText(_boostOverheatText, 2f));
    }
    public void DeactivateSpeedBoostOverheat() 
    {
        _boostOverheatText.gameObject.SetActive(false);
        _boostFill.color = _boostGoodColor;
        _boostFill.gameObject.SetActive(true);
    }
    public void ActivateSpeedBoostNoFuelText() 
    {
        _boostNoFuelText.gameObject.SetActive(true);
        _boostFill.gameObject.SetActive(false);
        StartCoroutine(FadeInText(_boostNoFuelText, 2f));
    }
    public void DeactivateSpeedBoostNoFuelText() 
    {
        _boostNoFuelText.gameObject.SetActive(false);
        _boostFill.color = _boostGoodColor;
        _boostFill.gameObject.SetActive(true);
    }
    public void ChangeBoostColorBad(float timeInSeconds) 
    {
        
        if (_boostFill != null) 
        {
            _isChangingBad = true;
            _isChangingGood = false;
            StartCoroutine(FadeImageColorToBad(_boostFill,_boostFill.color,_boostBadColor, timeInSeconds,_isChangingBad));
        }
        
    }
    public void ChangeBoostColorGood(float timeInSeconds)
    {
        
        if (_boostFill != null)
        {
            _isChangingGood = true;
            _isChangingBad = false;
            StartCoroutine(FadeImageColorToGood(_boostFill, _boostFill.color, _boostGoodColor, timeInSeconds));
        }

    }

    IEnumerator FadeImageColorToGood(Image image, Color a, Color b, float timeInSeconds)
    {
        float timer = 0.0f;

        while (image.color != b && image.isActiveAndEnabled && _isChangingGood)
        {
            timer += timeInSeconds * Time.deltaTime;
            image.color = new Color(Mathf.Lerp(a.r, b.r, timer), Mathf.Lerp(a.g, b.g, timer), Mathf.Lerp(a.b, b.b, timer), Mathf.Lerp(a.a, b.a, timer));
            yield return null;
        }

    }
    IEnumerator FadeImageColorToBad(Image image, Color a, Color b, float timeInSeconds, bool conditional)
    {
        float timer = 0.0f;

        while (image.color != b && image.isActiveAndEnabled && _isChangingBad)
        {
            timer += timeInSeconds * Time.deltaTime;
            image.color = new Color(Mathf.Lerp(a.r, b.r, timer), Mathf.Lerp(a.g, b.g, timer), Mathf.Lerp(a.b, b.b, timer), Mathf.Lerp(a.a, b.a, timer));
            yield return null;
        }

    }
    IEnumerator FadeInText(TextMeshProUGUI objectToFade, float timeInSeconds) 
    {
        
        float timer = 0.0f;
        while (objectToFade.isActiveAndEnabled) 
        {
            timer += timeInSeconds * Time.deltaTime;
           
            

            objectToFade.alpha = Mathf.Lerp(0.0f, 1.0f, timer);
           
            if (objectToFade.alpha >= 1) 
            {
                timer = 0;               
            }
            yield return null;
        }      
    }
    public void UpdateShieldLevel(int level) 
    {
        switch (level) 
        {
            case 0:
                _shieldLvl1Mat.DisableKeyword("GLOW_ON");
                _shieldLvl2Mat.DisableKeyword("GLOW_ON");
                _shieldLvl3Mat.DisableKeyword("GLOW_ON");
                break;
            case 1:
                _shieldLvl1Mat.EnableKeyword("GLOW_ON");
                _shieldLvl2Mat.DisableKeyword("GLOW_ON");
                _shieldLvl3Mat.DisableKeyword("GLOW_ON");
                break;
            case 2:
                _shieldLvl1Mat.DisableKeyword("GLOW_ON");
                _shieldLvl2Mat.EnableKeyword("GLOW_ON");
                _shieldLvl3Mat.DisableKeyword("GLOW_ON");
                break;
            case 3:
                _shieldLvl1Mat.DisableKeyword("GLOW_ON");
                _shieldLvl2Mat.DisableKeyword("GLOW_ON");
                _shieldLvl3Mat.EnableKeyword("GLOW_ON");
                break;
        }
    
    }
    IEnumerator GameOverFlickerRoutine() 
    {
        _clock.StopTimer();
        while (_isGameOver) 
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(.5f);
        }
    }

}
