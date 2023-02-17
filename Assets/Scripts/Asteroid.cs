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
    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<CircleCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _spawnManager = FindObjectOfType<SpawnManager>();
        
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
            Destroy(other.gameObject);
            Instantiate(_explosionPrefab, this.transform);
            StartCoroutine(AsteroidExplodeRoutine());
        }

    }

    IEnumerator AsteroidExplodeRoutine() 
    {
        yield return new WaitForSeconds(1f);
        _collider.enabled = false;
        _renderer.enabled = false;
        yield return new WaitForSeconds(1.61f);
        _spawnManager.StartSpawning();
        Destroy(gameObject);
    }
}
