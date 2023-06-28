using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float _fTimer = 0.0f;
    public float FTimer { get { return _fTimer; } }

    private int _iTimer = 0;
    public int ITimer { get { return CalcITimer(); } }

    private bool _running = false;

    private float _multiplier = 1.0f;
    public float Multiplier { get { return _multiplier; } set { _multiplier = value; } }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        incTimer();
    }

    /// <summary>
    /// Allows incrementation of timer if not a GameObject Component. If this is not a GameObject Component, put this in the Update(). 
    /// If timer is a GameObject Component, DO NOT USE!!
    /// </summary>
    public void incTimer() 
    {
        if (_running)
        {
            _fTimer += (Time.deltaTime * _multiplier);
        }
    }
    public void StartTimer() 
    {
        _running = true;
    }
    public void StopTimer()
    {
        _running = false;
    }

    public void ResetTimer() 
    {
        _fTimer = 0.0f;
    }
    private int CalcITimer() 
    {
        return (int)_fTimer;
    }
}
