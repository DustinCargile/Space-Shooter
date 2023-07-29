using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Clock : MonoBehaviour
{
    private Timer _timer;
    [SerializeField]
    private TextMeshProUGUI _clock;

    [SerializeField]
    private int _minutes=0, _seconds=0,_milliseconds=0;
    // Start is called before the first frame update
    void Start()
    {
        _timer=GetComponent<Timer>();
        _timer.Multiplier = 60;
        _timer.StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        
        //_timer.incTimer();
        if (_milliseconds >= 60)
        {
            _seconds++;
            if (_seconds >= 60) 
            {
                _seconds = 0;
                _minutes++;
            }
            _timer.ResetTimer();
        }
        _milliseconds = _timer.ITimer;
        
        _clock.text = string.Format("{0:d2}:{1:d2}:{2:d2}",_minutes,_seconds,_milliseconds);

       

    }
}
