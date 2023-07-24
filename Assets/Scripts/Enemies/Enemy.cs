using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour,ISpawnableEnemy,IDamagable,ITargetable
{
   
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private int _health = 1;

    public int Health { get { return _health; } }

    private bool _isTargeted = false;
    public bool IsTargeted { 
        get { return _isTargeted; }
        set
        {
            _isTargeted = value;
        } 
    }
    private int _currentTargets = 0;
    public int CurrentTargets { get { return _currentTargets; } set { _currentTargets = value; } }
    [SerializeField]
    private float _spawnWeight;
    public float SpawnWeight { get { return _spawnWeight; } }

    [SerializeField]
    private float _upperbound = 9.88f,
                  _lowerbound = -9.88f;

    [SerializeField]
    private float _timeDelay = 0.3f;
    private float _xPos;

    private float _leftbound = -16f,
                  _rightbound = 16f;

    private int _dir = 0;
    [SerializeField]
    private Player _player;

    [SerializeField]
    private SpawnManager _spawnManager;

    private BoxCollider2D _boxCollider;

    private Animator _animator;
    private bool _isDead = false;

    [SerializeField]
    private AudioClip _explosionSound;
    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _laserPrefab,_backLaserPrefab;

    private float _fireRate = 3.0f;
    private float _canFire = -1;
    [SerializeField]
    private bool _isTurning = false;
    [SerializeField]
    private bool _isSideways = false;

    private bool _hasShield = false;
    [SerializeField]
    private GameObject _shieldPrefab;
    private Animator _shieldAnimator;
    private Vector3 _toPlayer;
    private bool _playerIsBehind = false;

    
    private Powerup_Container _powerupContainer;
    [SerializeField]
    private Powerup[] _powerups;
    private bool _isInFrontOfPowerup = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
         _player = FindObjectOfType<Player>();
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spawnManager = FindObjectOfType<SpawnManager>();
        if (_spawnManager == null) 
        {
            Debug.Log(this.gameObject.name + " failed to load SpawnManager!");
        }

        _shieldAnimator = _shieldPrefab.GetComponent<Animator>();
        //null checks???
        if (_audioSource == null) 
        {
            Debug.Log("Could not find Enemy Audio Source!");
        }
        _powerupContainer = FindObjectOfType<Powerup_Container>();

        RandomizeAngle();
         GetNewPos();
        RandomHasShield();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (_powerupContainer != null)
        {
            _powerups = _powerupContainer.GetComponentsInChildren<Powerup>();
        }
        else 
        {
            Debug.Log("powerupsContainer is null");
        }
        if (_player != null)
        {
            _toPlayer = (_player.transform.position - transform.position).normalized;
            if (Vector3.Dot(_toPlayer, transform.up) > 0)
            {
                _playerIsBehind = true;
            }
            else 
            {
                _playerIsBehind = false;
            }
        }

        

        MoveDown();
        CheckForPowerup();
        if (Time.time > _canFire || (_isInFrontOfPowerup && Time.time >= (_canFire - 3f)))
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            
            if (!_isDead) 
            {
                FireLaser(!_playerIsBehind);
                
            }
        }
    }
    private void CheckForPowerup() 
    {
        foreach (Powerup p in _powerups) 
        {
            var rPoint = transform.InverseTransformPoint(p.gameObject.transform.position);
            if (rPoint.x >= -0.8f && rPoint.x <= 0.8f)
            {
                _isInFrontOfPowerup = true;

            }
            else 
            {
                _isInFrontOfPowerup = false;
            }

        }
    }
    

    private void FireLaser(bool down) 
    {

        GameObject prefabToLoad;
        if (down)
        {
            prefabToLoad = _laserPrefab;
            GameObject enemyLaser = Instantiate(prefabToLoad, transform.position, transform.localRotation);
            Laser[] laser = enemyLaser.GetComponentsInChildren<Laser>();
            foreach (Laser l in laser) 
            {
                l.MakeEnemyLaser();
                
            }
            
        }
        else 
        {
            prefabToLoad = _backLaserPrefab;
            GameObject enemyLaser = Instantiate(prefabToLoad, transform.position, transform.rotation);
            enemyLaser.transform.Rotate(transform.rotation.x, transform.rotation.y, transform.rotation.z + 180);
            Laser laser = enemyLaser.GetComponent<Laser>();
            laser.MakeEnemyLaser();

            
        }
                  
        
    }

    private void RandomHasShield() 
    {
        int r = Random.Range(0, 2);
        if (r == 1) 
        {
            _hasShield = true;
            _shieldPrefab.SetActive(true);
            _shieldAnimator.SetTrigger("Activate");
        }
    }
    private void MoveDown() 
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        
        if (transform.position.y < _lowerbound && !_isDead)
        {
            GetNewPos();
        }
        if (_isTurning && RangeOfFloats(transform.position.x,-1f,1f))
        {
            
            StartCoroutine(RotateToZero(.02f));
        }
        if (_isSideways && (transform.localPosition.x < _leftbound || transform.localPosition.x > _rightbound)) 
        {
            
            GetNewPos();
        }
    }
    IEnumerator RotateToZero(float timeInSeconds) 
    {
        float elapsed = 0.0f;
        while (elapsed < timeInSeconds) 
        {
            elapsed += Time.deltaTime;
            //transform.eulerAngles = new Vector3(transform.rotation.x,transform.rotation.y,Mathf.LerpAngle(transform.rotation.z,0f,elapsed));
            transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(transform.rotation.x, transform.rotation.y, 0f,transform.rotation.w),elapsed);
            yield return null;
           
            
        }
    }
    private bool RangeOfFloats(float value,float minInclusive, float maxInclusive) 
    {
        if (value >= minInclusive && value <= maxInclusive)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }
    private void GetNewPos() 
    {
      
        
        _xPos = Random.Range(_leftbound, _rightbound);
        transform.position = new Vector3(_xPos, _upperbound, 0);
        if (_isSideways)
        {
            float yPos = Random.Range(-6f, 6f);
            if (_dir == 0)
            {
                transform.position = new Vector3(_leftbound, yPos, 0);
            }
            else 
            {
                transform.position = new Vector3(_rightbound, yPos, 0);
            }
            
        }

    }
    float RandomBetweenTwoNumber(float number1, float number2) 
    {
        float n = 0.0f;
        if (Random.Range(0, 2)==0) 
            n =number1;
        else
            n= number2;

        return n;
    }
    int RandomBetweenTwoNumber(int number1, int number2)
    {
        int n = 0;
        if (Random.Range(0, 2) == 0)
            n = number1;
        else
            n = number2;

        return n;
    }
    private void RandomizeAngle() 
    {
        int isAngled = Random.Range(0, 3);
        
        switch (isAngled) 
        {
            case 0:
                break;
            case 1:
                _isTurning = true;
                transform.Rotate(Vector3.forward, Random.Range(-45f, 45f));
                break;
            case 2:
                _dir = Random.Range(0, 2);
                if (_dir == 0)
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 90f);
                }
                else 
                {
                    transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, -90f);
                }
                
                _isSideways = true;
                break;
            default:
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser") 
        {
            Laser laser = other.gameObject.GetComponent<Laser>();
            
            if (_player == null) { Debug.Log("Could not find Player Object!"); } else 
            { if (!laser.IsEnemyLaser) {  } }
            if (laser.IsDestroyedOnContact && !laser.IsEnemyLaser)
            {
                Destroy(other.gameObject);
                Damage(laser.Damage);
            }
            else if (!laser.IsEnemyLaser)
            {
                Damage(laser.Damage);
            }
            
        }
        if (other.tag == "Player") 
        {

            Player player = other.GetComponent<Player>();
            player.Damage(1);
            Damage(1);
        }
    }
   public void Damage(int value) 
    {
        _currentTargets--;
        if (_hasShield) 
        {
            _shieldAnimator.SetTrigger("Destroy");
            Destroy(_shieldPrefab, .2f);
            _hasShield = false;
            return;
        }
        _health -= value;
        if (_health <= 0) 
        {
            Kill();
        }
    }
    private void Kill() 
    {
        if (!_isDead) 
        {
            _spawnManager.KilledEnemy();
            if (_animator == null)
            {
                Debug.Log("Could not load animator for Enemy");
            }
            else
            {
                _animator.SetTrigger("Destroy");
            }
            if (_boxCollider == null)
            {
                Debug.Log("Box Collider could not be loaded!");
            }
            else
            {
                _boxCollider.enabled = false;
            }

            _player.AdjustScore(10);
            _isDead = true;
            _speed = 1;
            _audioSource.PlayOneShot(_explosionSound);
            Destroy(gameObject, 2.41f);
        }
        
    }
}
