using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basher_container : MonoBehaviour,ISpawnableEnemy
{
    [SerializeField]
    private float _spawnWeight;
    public float SpawnWeight { get { return _spawnWeight; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount < 2) 
        {
            Destroy(gameObject,2f);
        }
    }
}
