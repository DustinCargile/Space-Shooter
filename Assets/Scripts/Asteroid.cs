using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    

    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private GameObject _explosionPrefab;

    private CircleCollider2D _collider;
    private SpriteRenderer _renderer;
    private SpawnManager _spawnManager;

    [SerializeField]
    private AudioClip _explosionSound;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<CircleCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _spawnManager = FindObjectOfType<SpawnManager>();
        _audioSource = GetComponent<AudioSource>();
        
        if (_collider == null) 
        {
            Debug.Log("Could not find Asteroid Collider!");
        }
        if (_renderer == null) 
        {
            Debug.Log("Could not find Asteroid Renderer!");
        }
        if (_spawnManager == null) 
        {
            Debug.Log("Could not find SpawnManager");
        }
        if (_audioSource == null) 
        {
            Debug.Log("Could not find Asteroid Audio Source");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, 1 * Time.deltaTime * _speed) ;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser") 
        {
            Laser laser = other.GetComponent<Laser>();
            if (!laser.IsEnemyLaser) 
            {
                Destroy(other.gameObject);
                Instantiate(_explosionPrefab, this.transform);
                StartCoroutine(AsteroidExplodeRoutine());
                if (_audioSource == null)
                {
                    Debug.Log("Could not find Player Audio Source!");
                }
                else
                {
                    _audioSource.PlayOneShot(_explosionSound);
                }
            }
            
        }

    }

    IEnumerator AsteroidExplodeRoutine() 
    {
        _collider.enabled = false;
        yield return new WaitForSeconds(1f);
        _renderer.enabled = false;
        yield return new WaitForSeconds(1.61f);
        _spawnManager.StartSpawning();
        Destroy(gameObject);
    }
}
