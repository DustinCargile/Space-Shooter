using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BossHealthBar : MonoBehaviour
{
    private int _health;
    private int _damage = 0;
    [SerializeField]
    private TextMeshProUGUI _damageDisplay;
    private string _name="";
    private string _title="";
    [SerializeField]
    private TextMeshProUGUI _nameTitleDisplay;
    [SerializeField]
    private Slider _redSlider;
    [SerializeField]
    private Slider _yellowSlider;
    private bool _damageStop = false;
    private Timer _hitTimer;
    // Start is called before the first frame updat

   
    void Start()
    {
        _hitTimer = new Timer();
        _damageDisplay.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _hitTimer.incTimer();
        if (_hitTimer.FTimer > 1f) 
        {
            
            _hitTimer.StopTimer();
            _hitTimer.ResetTimer();

            SetYellowSlider();

        }
        if (_damageStop) 
        {
            _yellowSlider.value = Mathf.Lerp(_yellowSlider.value, _redSlider.value, Time.deltaTime * 3f);
            if (Numbers.RangeOfFloats(_yellowSlider.value,_redSlider.value - 1,_redSlider.value+1)) 
            {
                _damageStop = false;
                _yellowSlider.value = _redSlider.value;
            }
        }
    }
    

    public void SetNameTitle(string name, string title) 
    {
        string result = "";
        _name = name;
        _title = title;


        if (_name.Length > 0)
        {
            result += _name;
        }
        else 
        {
           
        }
        if (_title.Length > 0 && _name.Length > 0) 
        {
            result += ", ";
        }
        if (_title.Length > 0) 
        {
            result += _title;
        }
        _nameTitleDisplay.text = result;
    }
    public void SetHealth(int health)
    {
        _health = health;
        _redSlider.maxValue = health;
        _redSlider.value = health;
        _yellowSlider.maxValue = health;
        _yellowSlider.value = health;
    }
    public void SetDamage(int damage) 
    {
        if (_hitTimer.FTimer == 0)
        {
            _hitTimer.StartTimer();
        }
        else 
        {
            _hitTimer.ResetTimer();
        }
        _redSlider.value -= damage;
        _damageDisplay.gameObject.SetActive(true);
        _damage += damage;
        _damageDisplay.text = _damage.ToString();
        

    }
    IEnumerator YellowSliderOffset(int damage) 
    {
        yield return new WaitForSeconds(.5f);
        _yellowSlider.value -= damage;
        _damageDisplay.gameObject.SetActive(false);
    }
    private void SetYellowSlider() 
    {



        _damageStop = true;
        _damage = 0;
        _damageDisplay.gameObject.SetActive(false);
        
    }
}
