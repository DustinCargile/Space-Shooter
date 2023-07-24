using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool _isInvincible = false, _hasInfiniteAmmo = false;
    [SerializeField]
    private float _speed = 10f, _tripleShotCooldown = 0, _speedBoostCooldown = 0, _vulcanCooldown = 0;
    private float _overheatSpeed = 1f;
    [SerializeField]
    private float _maxSpeedBoost = 10;
    private float _currentBoostPercent;
    private bool _canUseSpeedBoost = true;

    [SerializeField]
    private Animator _animation;
    [SerializeField]
    private GameObject _prefabLaser, _prefabTriple,_prefabRocket, _shieldVisualizer, _prefabVulcan,_gravityVisualizer;
    private Vector3 _laserOffset = new Vector3(0, 1.3f, 0);
    private Vector3 _tripleshotOffset = new Vector3(-.53f, .4f, 0);
    private Vector3 _rocketOffset = new Vector3(-.48f, 0, 0);
    private BoxCollider2D _boxCollider;
    private CapsuleCollider2D _capsuleCollider;

    [SerializeField]
    private List<GameObject> _gravityConnections = new List<GameObject>();
    [SerializeField]
    private float _fireRate = 2f;
    private float _fireTimer = -1f;

    [SerializeField]
    private AudioClip _laserSound,_rocketSound, _expolsionSound,_hitSound;
    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private int _lives = 3, _score = 0, _ammo = 15, _maxAmmo = 15, _rocketAmmo = 0, _maxRocketAmmo = 20;    

    [SerializeField]
    private GameObject[] _Engines;

    [SerializeField]
    private UIManager ui;

    [SerializeField]
    private float _speedBoost = 3.5f;
    [SerializeField]
    private bool _isTripleShotEnabled = false, _isSpeedBoostEnabled = false,
        _isShieldsActive = false, _isVulcanEnabled =false;

    private float upperbounds = 0,
                lowerbounds = -7.4f,
                leftbounds = -15.62f,
                rightbounds = 15.62f;

    [SerializeField]
    private Transform LaserContainer;
    private SpawnManager _spawnManager;


    [Space(10)]
    [Header("Shield Settings")]
    #region ShieldLevels
    [SerializeField]
    private int _shieldLevel = 0;

    [SerializeField]
    private Color32 _shieldlvl1, _shieldlvl2, _shieldlvl3;

    private Timer _thrusterTimer = new Timer();
    #endregion
    [Space(10)]

    [SerializeField]
    private CameraShake _cameraShake;

    
    [SerializeField]
    private GameObject _thrusters;
    private Animator _thrustersAnimator;
    void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        ui = FindObjectOfType<UIManager>();
        _animation = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _thrustersAnimator = _thrusters.gameObject.GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        
        

        //null checks
        if (ui == null) 
        {
            Debug.Log("Player could not find UI!");
        }
        if (_thrustersAnimator == null) 
        {
            Debug.Log("Could not find Thruster Animator");
        }
        _thrusterTimer.StartTimer();
        transform.position = new Vector3(0, -4f, 0);
        

        _currentBoostPercent = _speedBoostCooldown / _maxSpeedBoost;
        ui.UpdateBoostCooldown(_currentBoostPercent);
        ui.UpdateAmmo(_ammo);
        ui.UpdateRocketAmmo(_rocketAmmo);
        ui.UpdateShieldLevel(_shieldLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
        Calculate_Movement();

        
        if (_ammo > 0 && Input.GetKeyDown(KeyCode.Space) && (Time.time > _fireTimer))
        {
            FireLaser();
        }
        if (_rocketAmmo > 0 && Input.GetKeyDown(KeyCode.R) && (Time.time > _fireTimer))
        {
            FireRocket();
        }
        if (Input.GetKeyDown(KeyCode.G)) { Damage(1); } //for testing purposes
        CalculateGravityVisualizer();
    }
    
    private void Calculate_Movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        
        
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canUseSpeedBoost)
        {
            SetSpeed_Enabled();
            ui.ChangeBoostColorBad(.5f);
           
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            SetSpeed_Disabled();
            if (_canUseSpeedBoost) 
            {
                ui.ChangeBoostColorGood(_thrusterTimer.FTimer);
            }
            _thrusterTimer.ResetTimer();
        }

        if (Input.GetKey(KeyCode.LeftShift) && _canUseSpeedBoost) 
        {
            _thrusterTimer.incTimer();
            if (_speedBoostCooldown > 0 && _thrusterTimer.ITimer >= 2)
            {
                SetSpeed_Disabled();
                StartCoroutine(SpeedBoostOverheatRoutine());                
            }
            if (_speedBoostCooldown <= 0)
            {
                StartCoroutine(SpeedBoostNoFuelRoutine());
            }
        }
        if (!_isSpeedBoostEnabled)
        {
            transform.Translate(direction * _speed *_overheatSpeed * Time.deltaTime);
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
        if (!_hasInfiniteAmmo) 
        {
            _ammo--;
        }
        ui.UpdateAmmo(_ammo);
        _fireTimer = Time.time + _fireRate;
        GameObject laser;
        //_laserOffset *= -1;
        if (_isTripleShotEnabled)
        {
            laser = Instantiate(_prefabTriple, transform.position + _tripleshotOffset, Quaternion.identity);
        }
        else if (_isVulcanEnabled) 
        {
            laser = Instantiate(_prefabVulcan, transform.position + _laserOffset, Quaternion.identity);
        }
        else 
        {
            laser = Instantiate(_prefabLaser, transform.position + _laserOffset, Quaternion.identity);
        }
        laser.transform.SetParent(LaserContainer);

        if (_audioSource == null)
        {
            Debug.Log("Could not find Player Audio Source!");
        }
        else 
        {
            _audioSource.PlayOneShot(_laserSound);
        }
        
    }
    private void FireRocket() 
    {
        
        _rocketOffset *= -1;
       
        
        if (_rocketAmmo > 0) 
        {
            GameObject rocket = null;
            rocket = Instantiate(_prefabRocket, transform.position + _rocketOffset, Quaternion.identity);
            rocket.transform.SetParent(LaserContainer);
            if (!_hasInfiniteAmmo)
            {
                _rocketAmmo--;
                ui.UpdateRocketAmmo(_rocketAmmo);
            }
        }
        
        if (_audioSource == null)
        {
            Debug.Log("Could not find Player Audio Source!");
        }
        else
        {
            _audioSource.PlayOneShot(_rocketSound);
        }
    }
    private void CalculateGravityVisualizer() 
    {
        if (Input.GetKeyDown(KeyCode.C)) 
        {
            for (int i = 0; i < 4; i++) 
            {
                GameObject ga = Instantiate(_gravityVisualizer,transform);
                ga.transform.position = new Vector3(ga.transform.position.x,transform.position.y - .3f,transform.position.z);
                ga.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 * i));
                _gravityConnections.Add(ga);
                
            }
            
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            foreach(GameObject gc in _gravityConnections.ToArray())
            {
                _gravityConnections.Remove(gc);
                Destroy(gc);
               
            }
        }
    }
    public void CreateGravityConnection(Transform target) 
    {
        GameObject gao = Instantiate(_gravityVisualizer, transform);
        _gravityConnections.Add(gao);
        GravityArrow ga = gao.GetComponent<GravityArrow>(); 
        
        ga.MakeGravityConnection(target);
    }
    public void Damage(int dmg)
    {
        
        if (_isShieldsActive)
        {
            
            Animator _shieldAnimator = _shieldVisualizer.gameObject.GetComponent<Animator>();
            if (_shieldAnimator == null)
            {
                Debug.Log("Could not find the Shield Animator!");
            }
            else
            {
                
                if (_shieldLevel <= 1)
                {
                    _isShieldsActive = false;
                    StartCoroutine(DisableShield());
                    
                }
                else 
                {
                    StartCoroutine(ShieldLevelDownRoutine());
                    
                }
                _shieldAnimator.SetTrigger("Destroy");
                _shieldAnimator.SetTrigger("LevelDown");
            }
            return;
        }
        if (!_isInvincible)
        {
            _lives -= dmg;
        }
        else 
        {
            return;
        }
        _audioSource.PlayOneShot(_hitSound,.6f);
        StartCoroutine(_cameraShake.Shake(.1f, 1f));

        if (_lives <= 0) 
        {
            _lives = 0;
        }
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
    public void Heal(int hp) 
    {        
        if (_lives < 3) 
        {
            _lives++;
            ui.UpdateLives(_lives);
            RandomEngineRepair(Random.Range(0, _Engines.Length));
        }
        
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

    void RandomEngineRepair(int val) 
    {
        if (!_Engines[val].active)
        {
            RandomEngineRepair(Random.Range(0, _Engines.Length));
        }
        else
        {
            _Engines[val].SetActive(false);
        }
    }
    
    public void AdjustScore(int val) 
    {
        _score += val;
        ui.SetScore(_score);

    }


    #region Powerups
    
    #region Speed Methods
    public void SetSpeed_Enabled()
    {
        if (_speedBoostCooldown > 0)
        {
            if (!_isSpeedBoostEnabled)
            {
                _isSpeedBoostEnabled = true;
                StartCoroutine(SpeedBoostPowerDownRoutine(.01f));
                _thrusters.GetComponent<ThrusterDamage>().BoostActivated();
            }

            if (_thrustersAnimator != null)
            {
                _thrustersAnimator.SetBool("hasSpeed", true);
            }
        }
        

    }
    private void SetSpeed_Disabled()
    {
        _isSpeedBoostEnabled = false;
        _thrusters.GetComponent<ThrusterDamage>().BoostDeactivated();
        if (_thrustersAnimator != null)
        {
            _thrustersAnimator.SetBool("hasSpeed", false);
        }
    }
    IEnumerator SpeedBoostNoFuelRoutine()
    {

        _canUseSpeedBoost = false;
        ui.ActivateSpeedBoostNoFuelText();
        yield return new WaitForSeconds(5f);
        ui.DeactivateSpeedBoostNoFuelText();
        _canUseSpeedBoost = true;
    }
    IEnumerator SpeedBoostOverheatRoutine() 
    {
        
        _canUseSpeedBoost = false;
        _overheatSpeed = .25f;
        ui.ActivateSpeedBoostOverheat();
        yield return new WaitForSeconds(5f);
        _overheatSpeed = 1;
        ui.DeactivateSpeedBoostOverheat();
        _canUseSpeedBoost = true;
    }
    public void AddSpeedCoolDown(float amt)
    {
        _speedBoostCooldown += amt;
        if (_speedBoostCooldown > _maxSpeedBoost) 
        {
            _speedBoostCooldown = _maxSpeedBoost;
        }
        ui.UpdateBoostCooldown(CalcCurrentSpeedBoost());
    }

    public float CalcCurrentSpeedBoost() 
    {
        _currentBoostPercent = _speedBoostCooldown / _maxSpeedBoost;
        return _currentBoostPercent;
    }
    IEnumerator SpeedBoostPowerDownRoutine(float timeInSeconds)
    {
        while (_isSpeedBoostEnabled && _canUseSpeedBoost)
        {
            yield return new WaitForSeconds(timeInSeconds);
            if (_speedBoostCooldown > 0)
            {
                _speedBoostCooldown -= timeInSeconds;
            }
            else
            {
                SetSpeed_Disabled();
            }

            ui.UpdateBoostCooldown(CalcCurrentSpeedBoost());


        }
    }
    #endregion

    #region TripleShot Methods
    public void setTripleShot_Enabled()
    {
        if (!_isTripleShotEnabled)
        {
            _isTripleShotEnabled = true;
            StartCoroutine(TripleShotPowerDownRoutine(1f));

        }

        _tripleShotCooldown += 5f;

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
    #endregion

    #region Ammo Methods

    public void RefillAmmo() 
    {
        _ammo = _maxAmmo;
        ui.UpdateAmmo(_ammo);
    }

    #endregion

    #region Shields Methods

    public void SetShieldActive()
    {
        ui.UpdateShieldLevel(_shieldLevel);
        if (!_isShieldsActive)
        {
            _isShieldsActive = true;
            _shieldVisualizer.SetActive(true);
        }
        ShieldLevelUp();
    }

    public void SetShieldInactive()
    {
        ui.UpdateShieldLevel(_shieldLevel);

        if (_isShieldsActive)
        {
            Animator _shieldAnimator = _shieldVisualizer.gameObject.GetComponent<Animator>();//This is called too many times to NOT be global
            if (_shieldAnimator == null)
            {
                Debug.Log("Could not find the Shield Animator!");
            }
            else 
            {
                _shieldAnimator.SetTrigger("Destroy");
                StartCoroutine(DisableShield());
            }               
        } 
    
    }
    public void ShieldLevelUp()
    {
        ui.UpdateShieldLevel(_shieldLevel);
        if (_shieldLevel < 3)
        {
            
            Animator animator = _shieldVisualizer.gameObject.GetComponent<Animator>();
            _shieldLevel++;
            if (_shieldLevel >= 1)
            {
                animator.SetTrigger("Activate");
            }
            ChangeShieldColor();

        }
        
    }
    public void ShieldLevelDown()
    {
        ui.UpdateShieldLevel(_shieldLevel);
        if (_shieldLevel > 0)
        {
            _shieldLevel--;

            ChangeShieldColor();
        }
        else 
        {
            StartCoroutine(DisableShield());
        }
        
    }

    public void ChangeShieldColor()
    {
        
        Material shieldmat = _shieldVisualizer.gameObject.GetComponent<SpriteRenderer>().material;
        switch (_shieldLevel)
        {
            case 0:
                SetShieldInactive();
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
        ui.UpdateShieldLevel(_shieldLevel);
    }
    IEnumerator ShieldLevelDownRoutine()
    {
        yield return new WaitForSeconds(0.3f);
        ShieldLevelDown();
    }

    IEnumerator DisableShield()
    {
        _shieldLevel = 0;
        yield return new WaitForSeconds(.25f);
        _isShieldsActive = false; ;
        _shieldVisualizer.SetActive(false);
        ui.UpdateShieldLevel(_shieldLevel);
    }
    #endregion

    #region Vulcan Weapon Methods
    public void SetVulcanActive() 
    {
        if (!_isVulcanEnabled)
        {
            _isVulcanEnabled = true;
            StartCoroutine(VulcanCooldownRoutine());

        }

        _vulcanCooldown += 5f;
    }

    IEnumerator VulcanCooldownRoutine() 
    {
        while (_isVulcanEnabled)
        {
            yield return new WaitForSeconds(1f);
            _vulcanCooldown--;
            if (_vulcanCooldown <= 0)
            {
                _vulcanCooldown = 0;
                _isVulcanEnabled = false;
            }
        }

    }
    #endregion

    #region Rockets Methods
    public void AddRockets() 
    {
        int rocketInc = 5;

        if (_rocketAmmo < _maxRocketAmmo) 
        {
            _rocketAmmo += rocketInc;
            
        }
        if (_rocketAmmo > _maxRocketAmmo) 
        {
            _rocketAmmo = _maxRocketAmmo;
        }
        ui.UpdateRocketAmmo(_rocketAmmo);
    }
    #endregion
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
        AudioSource.PlayClipAtPoint(_expolsionSound, transform.position);
        _thrusters.gameObject.SetActive(false);
        for (int i = 0; i < _Engines.Length; i++) 
        {
            _Engines[i].SetActive(false);
        }
        _boxCollider.enabled = false;
        _capsuleCollider.enabled = false;
       
        _animation.SetTrigger("Destroy");
        Destroy(gameObject,.5f);
    }

}
