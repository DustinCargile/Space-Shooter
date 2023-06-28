using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekingRocket : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyContainer;
    private SpawnManager _spawnManager;

    [SerializeField]
    private GameObject _target,_targetReticle;

    [SerializeField]
    private float _rotSpeed = 10f;

    private bool _isBlowingUp = false;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        _enemyContainer = _spawnManager.gameObject.transform.GetChild(0).gameObject;
        _target = FindTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != null) 
        {
            LookAt(_target.transform.position);
            if (Numbers.RangeOfFloats(Vector2.Distance(transform.position,_target.transform.position),-.1f,.1f)) 
            {
                BlowUp();
            }
        }
        
    }

    private void BlowUp() 
    {
        if (!_isBlowingUp) 
        {
            _isBlowingUp = true;
            Laser l = GetComponent<Laser>();
            l.BlowUp();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Asteroid")
        {
            BlowUp();
        }
    }

    private GameObject FindTarget()
    {
        
        ITargetable[] enemies = _enemyContainer.GetComponentsInChildren<ITargetable>();
        int ctr = 0;
        float closestDist = 12;
        int closestIndex = 0;
        GameObject target = null;
        for (int i = 0; i < enemies.Length; i++)
        {
            Collider2D eCollider = enemies[i].gameObject.GetComponent<Collider2D>();
            float distance = Vector2.Distance(transform.position, enemies[i].gameObject.transform.position);

            if (eCollider.enabled && distance <= closestDist)
            {

                closestDist = distance;
                closestIndex = i;
                target = enemies[closestIndex].gameObject;
                if (!enemies[closestIndex].IsTargeted) 
                {
                    enemies[closestIndex].IsTargeted = true;
                    Instantiate(_targetReticle, target.transform);
                }
                
                
            }

        }

        
        if (enemies.Length < 1)
        {
            
            LookAt(new Vector3(transform.position.x, 20, transform.position.z));
        }      
        if (target != null && !target.GetComponent<Collider2D>().enabled) 
        {
            target = null;
            
        }

        return target;
    }
    public void LookAt(Vector3 target)
    {
        Vector3 lookAtPos = target - transform.position;
        float rotZ = Mathf.Atan2(lookAtPos.y, lookAtPos.x) * Mathf.Rad2Deg;
        Quaternion newRot = Quaternion.Euler(0, 0, rotZ - 90);

        transform.rotation = Quaternion.Slerp(Quaternion.identity, newRot, Time.deltaTime * _rotSpeed);
    }
}
