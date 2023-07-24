using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzSiblings : MonoBehaviour,ISpawnableEnemy,IDamagable,ITargetable
{
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private int _health = 1;
    public int Health { get { return _health; } }
    [SerializeField]
    private float _spawnWeight;
    public float SpawnWeight { get { return _spawnWeight; } }
    [SerializeField]
    private int _damage = 1;

    private bool _isTargeted = false;
    public bool IsTargeted { get { return _isTargeted; } set { _isTargeted = value; } }
    private int _currentTargets = 0;
    public int CurrentTargets { get { return _currentTargets; } set { _currentTargets = value; } }
    [SerializeField]
    private float _upperbound = 7.6f,
                  _lowerbound = -7.6f;

    [SerializeField]
    private float _timeDelay = 0.3f;

    private float _leftbound = -16f,
                  _rightbound = 16f;
    [SerializeField]
    private Player _player;

    private CircleCollider2D _circleCollider;

    private Animator _animator;
    private bool _isDead = false;
    private bool _isWaiting = false;
    [SerializeField]
    private bool _isHunter = false;
    [SerializeField]
    private AudioClip _explosionSound;
    private AudioSource _audioSource;
    private int _dir = 0;
    private int _upDir = 1;
    [SerializeField]
    private float frequency, amplitude, minWaitTime, maxWaitTime;
    [SerializeField]
    private Vector3 target;
    private SpawnManager _spawnManager;

    private float _dirChangeAcc = 0;
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
        _animator = GetComponent<Animator>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spawnManager=FindObjectOfType<SpawnManager>();
        if (_audioSource == null)
        {
            Debug.Log("Could not find Enemy Audio Source!");
        }

        
        else 
        {
            RandomizeDirection();
            if (_dir == 0) //From Left
            {
                transform.position = new Vector3(_leftbound, Random.Range(_lowerbound,_upperbound), 0f);
                
            }
            else //From Right
            {
                transform.position = new Vector3(_rightbound, Random.Range(_lowerbound, _upperbound), 0f);
               
            }
            
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (_isHunter && _player != null)
        {
            target = _player.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);
        }
        else 
        {
            Move();
        }

    }
    private void RandomizeDirection() 
    {
        _dir = Random.Range(0, 2);
    }
    private void Move() 
    {
        float dirChangeTime = .5f;

        transform.Translate(Vector3.right * _speed * Time.deltaTime);
        transform.Translate(Vector3.up * frequency * _upDir* Time.deltaTime);
        if (transform.position.y > amplitude && _upDir == 1)
        {
            _upDir = -1;
            CheckIsWaiting();
        }
        else if (transform.position.y < -amplitude && _upDir == -1)
        {
            _upDir = 1;
            CheckIsWaiting();
        }
        if (transform.position.x < _leftbound && Time.time > _dirChangeAcc) 
        {
            _dir = 0;
            _speed *= -1;
            _dirChangeAcc = Time.time + dirChangeTime;
        }
        else if(transform.position.x > _rightbound && Time.time > _dirChangeAcc)
        {
            _dir = 1;
            _speed *= -1;
            _dirChangeAcc = Time.time + dirChangeTime;
        }

    }
    private void CheckIsWaiting() 
    {
        if (!_isWaiting)
        {
            _isWaiting = true;
            StartCoroutine(StayPutRoutine(minWaitTime, maxWaitTime));
        }
    }

    IEnumerator StayPutRoutine(float minTime,float maxTime) 
    {
        float tmpSpeed = _speed;
        float tmpFreq = frequency;
        _speed = 0;
        frequency = 0;
        yield return new WaitForSeconds(Random.Range(minTime,maxTime));
        frequency = tmpFreq;
        _speed = tmpSpeed;
        _isWaiting = false;
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Laser laser = other.gameObject.GetComponent<Laser>();
            if (laser == null) 
            {
                Debug.Break();
            }
            if (!laser.IsEnemyLaser) 
            {
                Damage(laser.Damage);
                if (laser.IsDestroyedOnContact) 
                {
                    Destroy(other.gameObject);
                }
            }
        }
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            player.Damage(_damage);
            Damage(1);
        }
    }
    public void Damage(int value) 
    {
        _currentTargets--;
        _health -= value;
        if (_health <= 0)
        {
            Kill();
            
        }
        else 
        {
            _animator.SetTrigger("Hit");
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
            if (_circleCollider == null)
            {
                Debug.Log("Box Collider could not be loaded!");
            }
            else
            {
                _circleCollider.enabled = false;
            }
            _player.AdjustScore(10);

            _isDead = true;
            _speed = 0;
            frequency = 0;
            _audioSource.PlayOneShot(_explosionSound);
            Destroy(gameObject, 1.0f);
        }

    }
}
