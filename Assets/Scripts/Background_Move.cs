using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Move : MonoBehaviour
{
    [SerializeField]
    private float _speed = .01f;
    [SerializeField]
    private bool _isRepeating = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * _speed);
        if (_isRepeating && transform.position.y <= -30.7f) 
        {
            transform.position = new Vector3(0, 29f, 0);
        }
    }
}
