using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool _stopSpawning = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator SpawnEnemyRoutine()
    {
        //add a yield return to delay spawn start
        int r;
        while (!_stopSpawning) 
        {
            r = Random.Range(0,_enemyPrefab.Length);
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject spawnedEnemy = Instantiate(_enemyPrefab[r], posToSpawn, Quaternion.identity);
            spawnedEnemy.transform.SetParent(_enemyContainer.transform);
            yield return new WaitForSeconds(_waitTime);
            
        }
        
    }
    IEnumerator SpawnPowerupRoutine() 
    {
        //add a yield return to delay spawn start
        int r;
        while (!_stopSpawning)
        {
            
            yield return new WaitForSeconds(Random.Range(3f,8f));
            r = Random.Range(0, _powerupPrefab.Length);
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            Instantiate(_powerupPrefab[r], posToSpawn, Quaternion.identity);
            

        }
    }
    public void OnPlayerDeath() 
    {
        _stopSpawning = true;
    }
    public void StartSpawning() 
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }
}
