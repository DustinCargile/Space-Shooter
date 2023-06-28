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
    private float _xPos,_yPos
        ;

    private float _leftbound = -13.9f,
                  _rightbound = 13.9f;
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
    [SerializeField]
    private float tXpos = -10f, tYpos = -5f, frequency, amplitude, minWaitTime, maxWaitTime;
    [SerializeField]
    private Vector3 target;
    private SpawnManager _spawnManager;
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
                transform.position = new Vector3(-12f, 0f, 0f);
                tXpos = -10;
            }
            else //From Right
            {
                transform.position = new Vector3(12f, 0f, 0f);
                tXpos = 10;
            }
            
        }
            
        
        tYpos = -amplitude;
        target = new Vector3(tXpos, tYpos, 0);


    }

    // Update is called once per frame
    void Update()
    {

        if (_isHunter && _player != null)
        {
            
            target = _player.transform.position;
        }
        
            Move();
        


    }
    private void RandomizeDirection() 
    {
        _dir = Random.Range(0, 2);
    }
    private void Move() 
    {
        
        transform.position = Vector3.MoveTowards(transform.position, target, _speed * Time.deltaTime);
        if (transform.position == target && !_isWaiting)
        {
            _isWaiting = true;
           StartCoroutine(StayPutRoutine(minWaitTime,maxWaitTime));
        }
        if (_dir == 0)
        {
            if (transform.position.x > 12)
            {
                transform.position = new Vector3(-12f, 0f, 0f);
                tXpos = -10f;
                tYpos = -5f;
                target = new Vector3(tXpos, tYpos, 0);
            }
        }
        else 
        {
            if (transform.position.x < -12)
            {
                transform.position = new Vector3(12f, 0f, 0f);
                tXpos = 10f;
                tYpos = -5f;
                target = new Vector3(tXpos, tYpos, 0);
            }
        }
    }

    IEnumerator StayPutRoutine(float minTime,float maxTime) 
    {
        yield return new WaitForSeconds(Random.Range(minTime,maxTime));
        if (_dir == 0)
        {
            tXpos += frequency;
            tYpos *= -1;
        }
        else 
        {
            tXpos -= frequency;
            tYpos *= -1;
        }
        target = new Vector3(tXpos, tYpos, 0);
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
            _speed = 1;
            _audioSource.PlayOneShot(_explosionSound);
            Destroy(gameObject, 1.0f);
        }

    }
}
