using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f, _tripleShotCooldown = 0, _speedBoostCooldown = 0;

    [SerializeField]
    private GameObject _prefabLaser, _prefabTriple, _shieldVisualizer;
    private Vector3 _laserOffset = new Vector3(0, .7f, 0);
    private Vector3 _tripleshotOffset = new Vector3(0, 3.5f, 0);

    [SerializeField]
    private float _fireRate = 2f;
    private float _fireTimer = -1f;

    [SerializeField]
    private AudioClip _laserSound,_expolsionSound,_powerupSound;
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private int _lives = 3, _score = 0;

    [SerializeField]
    private int _shieldLevel = 0;

    [SerializeField]
    private GameObject[] _Engines;
   
    [SerializeField]
    private UIManager ui;

    [SerializeField]
    private float _speedBoost = 3.5f;
    [SerializeField]
    private bool _isTripleShotEnabled = false, _isSpeedBoostEnabled = false, _isShieldsActive = false;
    
    private float upperbounds = 0,
                lowerbounds = -5.02f,
                leftbounds = -11.88f,
                rightbounds = 11.88f;

    [SerializeField]
    private Transform LaserContainer;
    private SpawnManager _spawnManager;


    #region ShieldColors
    [SerializeField]
    private Color32 _shieldlvl1, _shieldlvl2,_shieldlvl3;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        ui = FindObjectOfType<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        //null checks????
        //take the current position = starting position(0,0,0)
        transform.position = new Vector3(0, -4f, 0);
        Debug.Log(Screen.width + "," + Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        Calculate_Movement();

        
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _fireTimer)
        {
            FireLaser();
        }
    }
    private void Calculate_Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        if (!_isSpeedBoostEnabled)
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
        else 
        {
            transform.Translate(direction * (_speed + _speedBoost) * Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, lowerbounds, upperbounds));
        if (transform.position.y > upperbounds)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        if (transform.position.y < lowerbounds)
        {
            transform.position = new Vector3(transform.position.x, lowerbounds, 0);
        }
        if (transform.position.x < leftbounds)
        {
            transform.position = new Vector3(rightbounds, transform.position.y, 0);
        }
        if (transform.position.x > rightbounds)
        {
            transform.position = new Vector3(leftbounds, transform.position.y, 0);
        }
    }
    private void FireLaser() 
    {
        _fireTimer = Time.time + _fireRate;
        GameObject laser;
        if (_isTripleShotEnabled)
        {
            laser = Instantiate(_prefabTriple, transform.position + _tripleshotOffset, Quaternion.identity);
        }
        else 
        {
            laser = Instantiate(_prefabLaser, transform.position + _laserOffset, Quaternion.identity);
        }
        laser.transform.SetParent(LaserContainer);

        _audioSource.PlayOneShot(_laserSound);
        
    }

    public void Damage(int dmg)
    {
        if (_isShieldsActive)
        {
            
            Animator _animator = _shieldVisualizer.gameObject.GetComponent<Animator>();
            if (_animator == null)
            {
                Debug.Log("Could not find the Shield Animator!");
            }
            else
            {
                
                if (_shieldLevel == 1)
                {
                    _isShieldsActive = false;
                    _animator.SetTrigger("Destroy");
                    // _shieldVisualizer.transform.SetParent(null);
                    shieldLevelDown();
                    StartCoroutine(DisableShield());
                }
                else 
                {
                    _animator.SetTrigger("Destroy");
                    _animator.SetTrigger("LevelDown");
                    StartCoroutine(shieldLevelDownRoutine());
                    //shieldLevelDown();
                }
            }
            return;
        }
        _lives--;
        
        ui.UpdateLives(_lives);
        if (_lives <= 0)
        {
            Kill();
        }
        else 
        {
            RandomEngineDamage(Random.Range(0, _Engines.Length));
        }
        
    }
    IEnumerator shieldLevelDownRoutine() 
    {
        yield return new WaitForSeconds(0.3f);
        shieldLevelDown();
    }
    void RandomEngineDamage(int val)
    {
        if (_Engines[val].active)
        {
            RandomEngineDamage(Random.Range(0, _Engines.Length));
        }
        else
        {
            _Engines[val].SetActive(true);
        }
    }
    IEnumerator DisableShield() 
    {
        yield return new WaitForSeconds(.25f);
        _shieldVisualizer.SetActive(false);
        //_shieldVisualizer.transform.SetParent(this.transform);
        //_shieldVisualizer.transform.position = new Vector3(0, 0, 0);
    }
    public void AdjustScore(int val) 
    {
        _score += val;
        ui.SetScore(_score);

    }


    #region Powerups
    public void setTripleShot_Enabled()
    {
        if (!_isTripleShotEnabled) 
        {            
            _isTripleShotEnabled = true;
            StartCoroutine(TripleShotPowerDownRoutine(1f));
            
        }
        
        _tripleShotCooldown += 5f;
        _audioSource.PlayOneShot(_powerupSound);
    }
    public void setSpeed_Enabled() 
    {
        if (!_isSpeedBoostEnabled) 
        {
            _isSpeedBoostEnabled = true;
            StartCoroutine(SpeedBoostPowerDownRoutine(1f));
            
        }
        _speedBoostCooldown += 5;
        _audioSource.PlayOneShot(_powerupSound);
    }
    public void setShieldActive() 
    {
        if (!_isShieldsActive)
        {
            //this can be rewritten for efficency
            _isShieldsActive = true;
            _shieldVisualizer.SetActive(true);
            shieldLevelUp();
        }
        else 
        {
            shieldLevelUp();
            
        }
        //powerup playClipAtPoint
        _audioSource.PlayOneShot(_powerupSound);
    }
    public void shieldLevelUp() 
    {
        
        if (_shieldLevel < 3) 
        {
            Animator animator = _shieldVisualizer.gameObject.GetComponent<Animator>();
            _shieldLevel++;
            if (_shieldLevel > 1) 
            {
                animator.SetTrigger("Activate");
            }            
            changeShieldColor();
        }
        
    }
    public void shieldLevelDown()
    {

        if (_shieldLevel > 0)
        {
            _shieldLevel--;
            changeShieldColor();
        }

    }
    public void changeShieldColor() 
    {
        Material shieldmat = _shieldVisualizer.gameObject.GetComponent<SpriteRenderer>().material;
        switch (_shieldLevel)
        {
            case 0:
                break;
            case 1:
                shieldmat.color = _shieldlvl1;
                break;
            case 2:
                shieldmat.color = _shieldlvl2;
                break;
            case 3:
                shieldmat.color = _shieldlvl3;
                break;
            default:
                break;
        }
    }

    IEnumerator TripleShotPowerDownRoutine(float timeInSeconds)
    {
        while (_isTripleShotEnabled)
        {
            yield return new WaitForSeconds(timeInSeconds);
            _tripleShotCooldown--;
            if (_tripleShotCooldown <= 0)
            {
                _tripleShotCooldown = 0;
                _isTripleShotEnabled = false;
            }
        }
    }
    IEnumerator SpeedBoostPowerDownRoutine(float timeInSeconds)
    {
        while (_isSpeedBoostEnabled)
        {
            yield return new WaitForSeconds(timeInSeconds);
            _speedBoostCooldown--;
            if (_speedBoostCooldown <= 0)
            {
                _speedBoostCooldown = 0;
                _isSpeedBoostEnabled = false;

            }
        }
    }
    #endregion
    public void Kill() 
    {
        if (_spawnManager != null)
        {
            _spawnManager.OnPlayerDeath();
        }
        else 
        {
            Debug.LogError("Spawn Manager did not load!");
        }
        _audioSource.PlayOneShot(_expolsionSound);
        Destroy(gameObject);
    }

}
