using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityArrow : MonoBehaviour
{
    [SerializeField]
    private bool _isOnPowerup = false;
    public bool IsOnPowerup { get { return _isOnPowerup; } }

    private Player _player;
    private Transform _target;
    [SerializeField]
    private float _distance;
    public float Distance { get { return _distance; } }
    private void Start()
    {
        if (_isOnPowerup)
        {
            _player = FindObjectOfType<Player>();
            if (_player != null)
            {
                _target = _player.transform;
            }
        }
    }
    private void Update()
    {
        if (_target != null) 
        {
            LookAt(_target.position);
            _distance = FindPlayerDistance();
        }
        
        transform.localScale = new Vector3(transform.localScale.x,(_distance/2),transform.localScale.z);
    }
    public float FindPlayerDistance() 
    {
        return Vector3.Distance(transform.position, _target.position);
    }
    public void AttachToPowerup() 
    {
        _isOnPowerup = true;
    }
    public void MakeGravityConnection(Transform target) 
    {
        _target = target;
    }
    private void LookAt(Vector3 target)
    {
        Vector3 lookAtPos = target - transform.position;
        float rotZ = Mathf.Atan2(lookAtPos.y, lookAtPos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ + 90);
    }
}
