using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    private float _duration = 3f;
    private float _intensity = 10f;

    private Vector3 _originalPosition;
    private float _timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        _originalPosition = transform.position;
        _timer = _duration;
    }
    // Update is called once per frame
    void Update()
    {
        if (_timer > 0f) 
        {
            transform.position = _originalPosition + Random.insideUnitSphere * _intensity;
            _timer -= Time.deltaTime;
        } 
    }
    private void OnDisable()
    {
        _timer = 0f;
        transform.position = _originalPosition;
    }
}
