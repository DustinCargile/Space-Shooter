using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceWorm : MonoBehaviour,ISpawnableEnemy,IDamagable,ITargetable
{
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private int _health = 3;
    public int Health { get { return _health; } }
    [SerializeField]
    private float _spawnWeight;
    public float SpawnWeight { get { return _spawnWeight; } }

    private bool _isTargeted = false;
    public bool IsTargeted { get { return _isTargeted; } set { _isTargeted = value; } }
    private int _currentTargets = 0;
    public int CurrentTargets { get { return _currentTargets; } set { _currentTargets = value; } }

    [SerializeField]
    private Player _player;

    [SerializeField]
    private SpawnManager _spawnManager;

    private BoxCollider2D _boxCollider;
    private CapsuleCollider2D _capsuleCollider;

    private Animator _animator;
    private bool _isDead = false;

    [SerializeField]
    private AudioClip _explosionSound, _hitSound;
    private AudioSource _audioSource;

    [SerializeField]
    private Vector3 _targetPos;
    private bool _reachedTarget = false;

    private Material _material;
    private float _redColoring = 0;


    [SerializeField]
    private Vector3[] _targets;
    private int _currentTargetindex = 0;

    [SerializeField]
    private Sensor _sensor;
    private float _speedMultiplier = 1f;
    private bool _canDoDamage = true;

    private Transform _parentTransform;

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spawnManager = FindObjectOfType<SpawnManager>();
        _material = GetComponent<SpriteRenderer>().material;
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        

        //null checks???
        if (_audioSource == null)
        {
            Debug.Log("Could not find Enemy Audio Source!");
        }


        _targetPos = _targets[_currentTargetindex];
        transform.position = new Vector3(Random.Range(-15f, 15f), 8f, 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (!_isDead) 
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speedMultiplier * _speed * Time.deltaTime);

            _sensor.transform.position = transform.position;
            _sensor.transform.rotation = transform.rotation;
        }
        

        if (transform.position == _targetPos)
        {
            if (!_reachedTarget)
            {
 
                StartCoroutine(ReachedTarget());
            }
        }
        else
        {
            LookAt(_targetPos);
        }
        if (_sensor.AggroLevel == 0 && _canDoDamage && !_reachedTarget)
            _speedMultiplier = 1;
        if (_sensor.AggroLevel > 0 && !_reachedTarget) 
        {
            if (_player != null) 
            {
                _targetPos = _player.transform.position;
            }
        }
        if (_sensor.AggroLevel > 2 && _canDoDamage && !_reachedTarget) 
        {
            _speedMultiplier = 3;
            _animator.SetTrigger("Charge");
        }
        

       
    }
    IEnumerator AttackCooldown() 
    {
        _animator.SetTrigger("Normal");
        _canDoDamage = false;
        _speedMultiplier = 0f;
        StartCoroutine(ReachedTarget());
        yield return new WaitForSeconds(5f);        
        _canDoDamage = true;
        _speedMultiplier = 1;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Laser laser = other.gameObject.GetComponent<Laser>();

            if (!laser.IsEnemyLaser) 
            {
                Damage(laser.Damage);
                _targetPos = _player.transform.position;
                if (laser.IsDestroyedOnContact) 
                {
                    Destroy(other.gameObject);
                }
            }

        }
        if (other.tag == "Player" && _sensor.AggroLevel >4 && _canDoDamage)
        {
            StartCoroutine(AttackCooldown());
            Player player = other.GetComponent<Player>();
            player.Damage(1);

        }
    }

    public void LookAt(Vector3 target)
    {
        Vector3 lookAtPos = target - transform.position;
        float rotZ = Mathf.Atan2(lookAtPos.y, lookAtPos.x) * Mathf.Rad2Deg;
        Quaternion nRot = Quaternion.Euler(0, 0, rotZ + 90);
        transform.rotation = Quaternion.Slerp(transform.rotation, nRot, _speed * Time.deltaTime);
    }
    public void Damage(int value)
    {
        _currentTargets--;
        _audioSource.PlayOneShot(_hitSound, .5f);
        _animator.SetTrigger("Hit");
        StartCoroutine(ResetHit());
        _redColoring += (.1f * value * _health);
        _material.SetFloat("_OverlayBlend", _redColoring);
        _health -= value;
        if (_health <= 0)
        {
            Kill();
        }
    }
    private void Attack1()
    {
     
    }
    private void Attack2()
    {
       
    }
    IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(.05f);
        _animator.ResetTrigger("Hit");

    }
    private void Kill()
    {
        if (!_isDead) 
        {
            _spawnManager.KilledEnemy();
            if (_player == null) { Debug.Log("Could not find Player Object!"); }
            else
            { _player.AdjustScore(10); }
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
            if (_capsuleCollider == null)
            {
                Debug.Log("Capsule Collider could not be loaded!");
            }
            else
            {
                _capsuleCollider.enabled = false;
            }

            _sensor.gameObject.SetActive(false);
            _isDead = true;
            _speed = 1;
            _audioSource.PlayOneShot(_explosionSound);
            Destroy(gameObject, 2.41f);
        }

    }
    IEnumerator ReachedTarget()
    {
        _reachedTarget = true;
        _currentTargetindex++;
        if (_currentTargetindex >= _targets.Length)
        {
            _currentTargetindex = 0;
        }
        if (_player != null)
        {
            
        }
        yield return new WaitForSeconds(3f);
        _targetPos = _targets[_currentTargetindex];
        _reachedTarget = false;
    }
}
