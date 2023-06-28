using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageEffect : MonoBehaviour
{
    [SerializeField]
    private float _delay;
    private float _delaySeconds;
    [SerializeField]
    private GameObject _prefabGhost;
    private bool _makeGhosts = false;
    public bool MakeGhosts { get { return _makeGhosts; }set{ _makeGhosts = value; } }
    
    private SpriteRenderer _spriteRenderer;
    
    
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _delaySeconds = _delay;
    }

    private void Update()
    {
        
        if (_makeGhosts && _delaySeconds <= 0)
        {            
            GameObject currentGhost = Instantiate(_prefabGhost, transform.position, Quaternion.identity);
            SpriteRenderer cGhostRender = currentGhost.transform.gameObject.GetComponent<SpriteRenderer>();
            cGhostRender.sprite = _spriteRenderer.sprite;
            cGhostRender.transform.localScale = _spriteRenderer.transform.localScale;
            cGhostRender.transform.position = _spriteRenderer.transform.position;

            _delaySeconds = _delay;
        }
        else 
        {
            _delaySeconds -= Time.deltaTime;
        }
    }
   
    

}
    

   
   
