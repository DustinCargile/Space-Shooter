using System.Collections;

using System.Collections.Generic;
using UnityEngine;

public class Dodger : MonoBehaviour, ISpawnableEnemy, IDamagable, ITargetable
{
    [SerializeField]
    private float _speed = 5f, _speedBoost = 10f;
    [SerializeField]
    private int _health = 1;

    public int Health { get { return _health; } }

    private bool _isTargeted = false;
    public bool IsTargeted { get { return _isTargeted; } set { _isTargeted = value; } }

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

    private float _leftbound = -14.3f,
                  _rightbound = 14.3f;

    private int _dir = 0;
    [SerializeField]
    private Player _player;

    [SerializeField]
    private SpawnManager _spawnManager;

    private PolygonCollider2D _polyCollider;

    private Animator _animator;
    private bool _isDead = false;

    [SerializeField]
    private AudioClip _explosionSound;
    private AudioSource _audioSource;

    [SerializeField]
    private GameObject _laserPrefab;

    private float _fireRate = 3.0f;
    private float _canFire = -1;
    private bool _projectileInFront = false;

    
    private int _rLeftRight = 0;

    private float arcAngle = 45f;
    private float raycastDist = 2f;
    private int rayCount = 10;

    
    private AfterImageEffect _afterImage;
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
        _animator = GetComponent<Animator>();
        _polyCollider = GetComponent<PolygonCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _spawnManager = FindObjectOfType<SpawnManager>();
        _afterImage = GetComponent<AfterImageEffect>();
        if (_spawnManager == null)
        {
            Debug.Log(this.gameObject.name + " failed to load SpawnManager!");
        }

       
        //null checks???
        if (_audioSource == null)
        {
            Debug.Log("Could not find Enemy Audio Source!");
        }

        if (_isTargeted) 
        {
            Debug.Log("Dodger is Targeted!");
        }
     
        GetNewPos();
    }

    // Update is called once per frame
    void Update()
    {
        
        MoveDown();
        
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;

            if (!_isDead)
            {
                FireLaser();

            }
        }
        CheckForIncomingProjectiles();
        
    }
    
    private void CheckForIncomingProjectiles() 
    {
        float angleIncrement = arcAngle / (rayCount - 1);

        for (int i = 0; i < rayCount; i++) 
        {
            float angle = -arcAngle / 2 + i * angleIncrement;
            float radians = Mathf.Deg2Rad * angle;
            Vector2 direction = new Vector2(Mathf.Sin(radians), -Mathf.Cos(radians));
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, raycastDist);
            Debug.DrawRay(transform.position, direction * raycastDist, Color.red, .1f);
            foreach (RaycastHit2D hit in hits) 
            {
                if (hit.collider != null)
                {

                    GameObject hitObject = hit.collider.gameObject;

                    if (hitObject.tag == "Laser")
                    {
                        Laser laser = hitObject.GetComponent<Laser>();
                        if (!laser.IsEnemyLaser)
                        {
                            Debug.DrawRay(transform.position, hit.point - (Vector2)transform.position, Color.red);
                            HandleCollision(hitObject);
                        }
                    }

                }
                else
                {
                    Debug.Log("No hit collider found!");
                }
            }
        }       
 
    }
    
    private void HandleCollision(GameObject projectile) 
    {
        if (!_projectileInFront) 
        {
            StartCoroutine(Dodge());
            _projectileInFront = true;
            var rPoint = transform.InverseTransformPoint(projectile.gameObject.transform.position);
            if (rPoint.x < 0)
            {
                _rLeftRight = 1;
            }
            else 
            {
                _rLeftRight = 0;
            }
        }
    }
    IEnumerator Dodge() 
    {
        _afterImage.MakeGhosts = true;
        
        yield return new WaitForSeconds(.05f);
        
        _projectileInFront = false;
       
        _afterImage.MakeGhosts = false;

    }
    private Vector3 GetPlayerFireDirection() 
    {
        Vector3 direction = _player.transform.position - transform.position;
        direction.Normalize();
        return direction;
    }
    private void FireLaser()
    {

        GameObject prefabToLoad;
        prefabToLoad = _laserPrefab;
        GameObject enemyLaser = Instantiate(prefabToLoad, transform.position, transform.localRotation);
        Laser[] laser = enemyLaser.GetComponentsInChildren<Laser>();
        foreach (Laser l in laser)
        {
            l.MakeEnemyLaser();
            
        }  


    }

   
    private void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < _lowerbound && !_isDead)
        {
            GetNewPos();
        }
        
        if (_projectileInFront) 
        {
            if (_rLeftRight == 0)
            {
                MoveLeft();
            }
            else
            {
                MoveRight();
            }
        }
    }
    private void MoveLeft() 
    {
        transform.Translate(Vector3.left * _speed * _speedBoost * Time.deltaTime);
        
    }
    private void MoveRight()
    {
        transform.Translate(Vector3.right * _speed * _speedBoost * Time.deltaTime);
        
    }
    private bool RangeOfFloats(float value, float minInclusive, float maxInclusive)
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
        

    }
    float RandomBetweenTwoNumber(float number1, float number2)
    {
        float n = 0.0f;
        if (Random.Range(0, 2) == 0)
            n = number1;
        else
            n = number2;

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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Laser laser = other.gameObject.GetComponent<Laser>();

            if (_player == null) { Debug.Log("Could not find Player Object!"); }
            else
            { if (!laser.IsEnemyLaser) { } }
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
        
        _health -= value;
        _currentTargets--;
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
            if (_polyCollider == null)
            {
                Debug.Log("Box Collider could not be loaded!");
            }
            else
            {
                _polyCollider.enabled = false;
            }

            _player.AdjustScore(10);
            _isDead = true;
            _speed = 1;
            _audioSource.PlayOneShot(_explosionSound);
            Destroy(gameObject.transform.GetChild(0).gameObject);
            Destroy(gameObject,.5f);
        }

    }
}
