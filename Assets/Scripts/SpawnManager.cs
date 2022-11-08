using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _waitTime = 5;
    [SerializeField]
    GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;

    private bool _stopSpawning = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Spawn an enemy every 5 seconds
    //Create a coroutine of type IEnumerator -- Yield Events
    //while loop

    IEnumerator SpawnRoutine()
    {
        //while loop(infinite loop)
        //Instantiate enemy prefab
        //yield wait for 5 seconds
        while (!_stopSpawning) 
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject spawnedEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            spawnedEnemy.transform.SetParent(_enemyContainer.transform);
            yield return new WaitForSeconds(_waitTime);
            
        }
        
    }

    public void OnPlayerDeath() 
    {
        _stopSpawning = true;
    }
}
