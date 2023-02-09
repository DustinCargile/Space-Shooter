using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f, _tripleShotCooldown = 0, _speedBoostCooldown = 0;

    [SerializeField]
    private GameObject _prefabLaser, _prefabTriple;
    private Vector3 _laserOffset = new Vector3(0, .7f, 0);
    private Vector3 _tripleshotOffset = new Vector3(0, 3.5f, 0);

    [SerializeField]
    private float _fireRate = 2f;
    private float _fireTimer = -1f;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private float _speedBoost = 3.5f;
    [SerializeField]
    private bool _isTripleShotEnabled = false, _isSpeedBoostEnabled = false;
    
    private float upperbounds = 0,
                lowerbounds = -5.02f,
                leftbounds = -11.88f,
                rightbounds = 11.88f;

    [SerializeField]
    private Transform LaserContainer;
    private SpawnManager _spawnManager;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        //take the current position = starting position(0,0,0)
        transform.position = new Vector3(0, 0, 0);
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
    }

    public void Damage(int dmg) 
    {
        _lives--;
        if (_lives <= 0) 
        {
            Kill();
        }
    }
    public void setTripleShot_Enabled()
    {
        if (!_isTripleShotEnabled) 
        {            
            _isTripleShotEnabled = true;
            StartCoroutine(TripleShotPowerDownRoutine(1f));
            
        }
        
        _tripleShotCooldown += 5f;
        
    }
    public void setSpeed_Enabled() 
    {
        if (!_isSpeedBoostEnabled) 
        {
            _isSpeedBoostEnabled = true;
            StartCoroutine(SpeedBoostPowerDownRoutine(1f));
            
        }
        _speedBoostCooldown += 5;

    }
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
        Destroy(gameObject);
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
}
