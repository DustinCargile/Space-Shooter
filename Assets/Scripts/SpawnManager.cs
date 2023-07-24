using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _waitTime = 5;
    [SerializeField]
    GameObject[] _enemyPrefab, _powerupPrefab;
    [SerializeField]
    private float _powerupSpawnMin = 3f, _powerupSpawnMax = 8f;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private Powerup_Container _powerupContainer;
    //private float _spawnNumber = 0.0f;
    [SerializeField]
    private int _enemySpawnAmount = 1, _powerupSpawnAmount = 1;
    private bool _stopSpawning = false;
    [SerializeField]
    private int _spawnedEnemyCount = 0;
    private int _spawnedPowerupCount = 0;
    private int _enemiesDowned = 0;
    private int _totalEnemiesDowned = 0;
    [SerializeField]
    private Wave[] _levelWaves;
    [SerializeField]
    private int _currentWave = 0;
    private bool _waveComplete = true;
    [SerializeField]
    private TextMeshProUGUI _waveText;

    private GameManager _gameManager;

    
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _powerupContainer = FindObjectOfType<Powerup_Container>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator SpawnEnemyRoutine()
    {
        _enemyPrefab = new GameObject[_levelWaves[_currentWave].enemies.Length];
        for (int i = 0; i < _enemyPrefab.Length; i++) 
        {
            _enemyPrefab[i] = _levelWaves[_currentWave].enemies[i];
        }
        while (!_stopSpawning && _spawnedEnemyCount < _levelWaves[_currentWave].maxEnemies) 
        {
            for (int i = 0; i < _enemySpawnAmount; i++) 
            {
                SpawnRandomEnemy();
            }
            
            yield return new WaitForSeconds(_waitTime / _levelWaves[_currentWave].enemyFrequency);
            
        }
        
    }
    private void SpawnRandomEnemy() 
    {
        int r;
        float spawnNumber = Random.Range(0f, 101f);
        r = Random.Range(0, _enemyPrefab.Length);
        //Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
        Vector3 posToSpawn = new Vector3(0, 0, 0);
        ISpawnableEnemy randEnemy = _enemyPrefab[r].gameObject.GetComponent<ISpawnableEnemy>();
        //Enemy randEnemy = _enemyPrefab[r].gameObject.GetComponent<Enemy>(); 
        if (_levelWaves[_currentWave].maxEnemies > _spawnedEnemyCount) 
        {
            if (randEnemy == null)
            {
                Debug.Log("Failed to Load Enemy!");
            }
            if (randEnemy.SpawnWeight >= spawnNumber)
            {
                GameObject spawnedEnemy = Instantiate(_enemyPrefab[r], posToSpawn, Quaternion.identity);
                spawnedEnemy.transform.SetParent(_enemyContainer.transform);
                _spawnedEnemyCount++;
            }
            else
            {
                SpawnRandomEnemy();
            }
        }
        
    }
    IEnumerator SpawnPowerupRoutine() 
    {
        while (!_stopSpawning && _spawnedPowerupCount < _levelWaves[_currentWave].maxPowerups)
        {
            float multiplier = _levelWaves[_currentWave].powerupFrequency;
            yield return new WaitForSeconds(Random.Range(_powerupSpawnMin/multiplier,_powerupSpawnMax/multiplier));
            for (int i = 0; i < _powerupSpawnAmount; i++) 
            {
                if (!_stopSpawning) 
                {
                    SpawnRandomPowerup();
                }
            }            
        }
    }
    
    private void SpawnRandomPowerup() 
    {
        _powerupPrefab = new GameObject[_levelWaves[_currentWave].powerups.Length];
        for (int i = 0; i < _powerupPrefab.Length; i++)
        {
            _powerupPrefab[i] = _levelWaves[_currentWave].powerups[i];
        }
        int r;
        float spawnNumber = Random.Range(0f, 101f);
        r = Random.Range(0, _powerupPrefab.Length);
        Powerup randPowerup = _powerupPrefab[r].gameObject.GetComponent<Powerup>();
        if (_levelWaves[_currentWave].maxPowerups > _spawnedPowerupCount) 
        {
            if (randPowerup == null)
            {
                Debug.Log("Failed to get Powerup!");
            }
            if (randPowerup.SpawnWeight >= spawnNumber)
            {
                Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
                GameObject powerup = Instantiate(_powerupPrefab[r], posToSpawn, Quaternion.identity);
                powerup.transform.SetParent(_powerupContainer.gameObject.transform, true);
                _spawnedPowerupCount++;
            }
            else
            {
                if (!_stopSpawning)
                {
                    SpawnRandomPowerup();
                }
            }
        }

    }
    public void KilledEnemy() 
    {
        _enemiesDowned++;
        _totalEnemiesDowned++;
        if (_enemiesDowned >= _levelWaves[_currentWave].maxEnemies && _enemySpawnAmount != 0)
        {
            _currentWave++;
            _enemiesDowned = 0;
            _spawnedEnemyCount = 0;
            _spawnedPowerupCount = 0;
            _waveComplete = true;
            _stopSpawning = true;
            StartCoroutine(WaveCompleteRoutine());
        }
    }
    public void OnPlayerDeath() 
    {
        _stopSpawning = true;
    }
    IEnumerator WaveCompleteRoutine() 
    {
        _waveText.text = "Wave Complete!";
        _waveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        _waveText.gameObject.SetActive(false);
        if (_currentWave >= _levelWaves.Length)
        {
            StartCoroutine(LevelCompleteRoutine());
        }
        else 
        {
            StartCoroutine(WaveSpawnRoutine());
        }
    }
    IEnumerator LevelCompleteRoutine() 
    {
        _enemiesDowned = 0;
        _waveText.text = "Level Complete!";
        _waveText.gameObject.SetActive(true);
        _stopSpawning = true;
        yield return new WaitForSeconds(5f);
        _waveText.gameObject.SetActive(false);
        _gameManager.LevelComplete();
        UIManager uimanager = FindObjectOfType<UIManager>();//This needs to be cleaned up.
        if (uimanager != null) 
        {
            uimanager.LevelComplete();
        }

    }
    IEnumerator WaveSpawnRoutine() 
    {
        
        while (_currentWave < _levelWaves.Length && _waveComplete) 
        {
            _enemiesDowned = 0;
            _enemySpawnAmount = _levelWaves[_currentWave].enemySpawnAmount;
            _powerupSpawnAmount = _levelWaves[_currentWave].powerupSpawnAmount;
            _waveText.text = _levelWaves[_currentWave].waveName;
            _waveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(5f);
            _waveText.gameObject.SetActive(false);
            _stopSpawning = false;
            StartCoroutine(SpawnEnemyRoutine());
            StartCoroutine(SpawnPowerupRoutine());
            _waveComplete = false;
            
        }
    }
    public void StartSpawning() 
    {
        
        StartCoroutine(WaveSpawnRoutine());
        
    }
}
