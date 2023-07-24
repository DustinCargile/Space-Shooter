using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour,ISpawnableEnemy,IDamagable,ITargetable
{
    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private int _health = 3;
    public int Health { get { return _health; } }
    [SerializeField]
    private float _spawnWeight;
    public float SpawnWeight { get { return _spawnWeight; } }
    [SerializeField]
    private Player _player;
    private bool _isTargeted = false;
    public bool IsTargeted { get { return _isTargeted; } set { _isTargeted = value; } }
    private int _currentTargets = 0;
    public int CurrentTargets { get { return _currentTargets; } set { _currentTargets = value; } }
    [SerializeField]
    private SpawnManager _spawnManager;

    private BoxCollider2D _boxCollider;

    private Animator _animator;
    private bool _isDead = false;

    [SerializeField]
    private AudioClip _explosionSound,_hitSound;
    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _projectile1Prefab,projectile2Prefab;

    private float _fireRate = 3.0f;
    private float _canFire1 = -1;
    private float _canFire2 = -1;

    [SerializeField]
    private Vector3 _targetPos;
    private bool _reachedTarget = false;

    private Material _material;
    private float _redColoring = 0;
    

    [SerializeField]
    private Vector3[] _targets;
    private int _currentTargetindex = 0;
    [SerializeField]
    private GameObject[] _bombTubes;

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
        _animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spawnManager = FindObjectOfType<SpawnManager>();
        _material = GetComponent<SpriteRenderer>().material;
        
        //null checks???
        if (_audioSource == null)
        {
            Debug.Log("Could not find Enemy Audio Source!");
        }


        _targetPos = _targets[_currentTargetindex];
        transform.position = new Vector3(Random.Range(-15f,15f),8f,0);
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speed*Time.deltaTime);
        
        if (transform.position == _targetPos)
        {
            if (!_reachedTarget) 
            {
                _reachedTarget = true;
                StartCoroutine(ReachedTarget());
            }
        }
        else 
        {
            LookAt(_targetPos);
        }

        if (Time.time > _canFire1)
        {
            _fireRate = Random.Range(2f, 5f);
            _canFire1 = Time.time + _fireRate;
            if (!_isDead)
            {

                Attack1();
            }
        }
        if (Time.time > _canFire2)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire2 = Time.time + _fireRate;
            if (!_isDead)
            {
                
                Attack2();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Laser laser = other.gameObject.GetComponent<Laser>();

            
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
            
        }
    }

    public void LookAt(Vector3 target) 
    {
        Vector3 lookAtPos = target - transform.position;
        float rotZ = Mathf.Atan2(lookAtPos.y, lookAtPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ + 90);
    }
    public void Damage(int value)
    {
        _currentTargets--;
        _audioSource.PlayOneShot(_hitSound, .5f);
        _animator.SetTrigger("Hit");
        StartCoroutine(ResetHit());
        _redColoring += (.1f * value);
        _material.SetFloat("_OverlayBlend", _redColoring);
        //_material.color = new Color(_material.color.r + _redColoring,_material.color.g,_material.color.b,_material.color.a);
        _health -= value;
        if (_health <= 0)
        {
            Kill();
        }
    }
    private void Attack1() 
    {
        GameObject bombTubeParticles;
        GameObject bombFlash;
        int bombTubeSelector = Random.Range(0, _bombTubes.Length);
        GameObject enemyBomb = Instantiate(_projectile1Prefab, 
            _bombTubes[bombTubeSelector].transform.position, 
            transform.localRotation);
        bombTubeParticles = _bombTubes[bombTubeSelector].transform.GetChild(0).gameObject;

        bombTubeParticles.SetActive(false);
        bombTubeParticles.SetActive(true);
    
        Laser bomb = enemyBomb.GetComponent<Laser>();
        bomb.MakeEnemyLaser();

    }
    
    private void Attack2() 
    {
        for (int i = 0; i < 8; i++) 
        {
            float rotZ = 45 * i;
            GameObject spike = Instantiate(projectile2Prefab, 
                transform.position,
                Quaternion.Euler(0,0,rotZ));
        }
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


            _isDead = true;
            _speed = 1;
            _audioSource.PlayOneShot(_explosionSound);
            Destroy(gameObject, 2.41f);
        }

    }
    IEnumerator ReachedTarget()
    {
        _currentTargetindex++;
        if (_currentTargetindex >= _targets.Length) 
        {
            _currentTargetindex = 0;
        }
        if (_player != null)
        {
            LookAt(_player.transform.position);
            Attack1();
        }
        yield return new WaitForSeconds(3f);        
        _targetPos = _targets[_currentTargetindex];
        _reachedTarget = false;
    }
}
