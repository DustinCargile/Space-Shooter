using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour, ISpawnableEnemy, IDamagable, ITargetable
{
    private enum State
    {
        Idle, ActivateTeeth, BiteAttack,ElectricAttack, Charge, PowerShot, Multishot, RegularShot, Critical, Cooldown, Moving,Die
    }
    private State _currentState;
    private BossStateMachine stateMachine;
        
    private Player _player;
    private SpawnManager _spawnManager;

    private Vector3 _centerOffset = new Vector3(-1.11f, -.9f, 0);
    [SerializeField]
    private string _name = "Eyegore", _title = "Necro-Mechanoid All-Seeing-Eye";
    private Animator bossAnimator;
    [SerializeField]
    private AudioSource _bgMusic, _sfx;
    [SerializeField]
    private AudioClip _bgMusicClipStart, _bgMusicClipLoop, _sfxHit,_sfxDeflect,_sfxBite,_sfxTeethOut,_sfxTeethIn, _sfxDestroy;
    private AudioClip _originalBGMusic;
    [SerializeField]
    private GameObject _laserCharge, _bigLaser,_multishotFX,_electircalChargeFX, _electricalDischargeFX, _regularLaser, _largeCollider;
    [SerializeField]
    private int _multishotProjectileAmt = 8;
    private int _isBiting = 0;

    private bool _lowSine = false;
    private bool _isPursuingPlayer = false;
    private bool _didAnElectricalAttackLast = false;
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private int _regularHitCounter = 5;
    [SerializeField]
    private bool _isShootable = false;
    [SerializeField]
    private bool _isCritical = false;
    private bool _canStart = false;
    private bool _isDead = false;
    [SerializeField]
    private BossHealthBar healthBar;
    [SerializeField]
    private int _health = 500;
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _chargeTime = 1.75f;

    private float _dirChangeAcc = -1;

    private Vector3 _initPos = new Vector3(1.22f,16,0);
    private Vector3 _startPos = new Vector3(1.22f, 2.03f, 0);
    private float _leftBounds = -9.76f;
    private float _rightBounds = 12.19f;
    #region Implementing Interfaces
    [SerializeField]
    private float _spawnWeight;
    public float SpawnWeight { get { return _spawnWeight; } }
    private bool _isTargeted = false;
    public bool IsTargeted
    {
        get { return _isTargeted; }
        set
        {
            _isTargeted = value;
        }
    }
    private int _currentTargets = 0;
    public int CurrentTargets { get { return _currentTargets; } set { _currentTargets = value; } }
    public int Health { get { return _health; } }
    #endregion

    private AfterImageEffect _aeEffect;
    [SerializeField]
    private GameObject _explosionFX;
    [SerializeField]
    private GameObject _critPartFX;
    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<Player>();
        _spawnManager = FindObjectOfType<SpawnManager>();
        _bgMusic = FindObjectOfType<BackgroundAudioManager>().GetComponent<AudioSource>();
        _aeEffect = GetComponent<AfterImageEffect>();
        //Instantiate(healthBar);
        _sfx = GetComponent<AudioSource>();
        healthBar.SetHealth(_health);
        healthBar.SetNameTitle(_name, _title);
        stateMachine = new BossStateMachine();
        bossAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        transform.position = _initPos;
        _originalBGMusic = _bgMusic.clip;
        _bgMusic.clip = _bgMusicClipStart;
        _bgMusic.Play();


        TransitionToIdleState();
        
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();

        

        if (!_canStart)
        {
            if (Numbers.RangeOfFloats(transform.position.y, _startPos.y - .01f, _startPos.y + .01f))
            {

                transform.position = _startPos;
                _canStart = true;
                TransitionToIdleState();
                _bgMusic.clip = _bgMusicClipLoop;
                _bgMusic.Play();

            }
            else
            {
                float timeToPos = _bgMusicClipStart.length / (_initPos.y - _startPos.y)/2;
                transform.position = new Vector3(Random.Range(_initPos.x - .03f, _initPos.x + .03f)
                                                , Mathf.Lerp(transform.position.y, _startPos.y, Time.deltaTime * timeToPos)
                                                ,0);
            }
        }
        else 
        {
            CalculateMovement();
        }
        if(_isShootable)
            _largeCollider.SetActive(false);
        else
            _largeCollider.SetActive(true);
    }
    private void CalculateMovement() 
    {
        float dirChangeTime = .5f;
        
        if (_currentState == State.ElectricAttack) 
        {
            SineWavePattern();
        }
        if (_currentState == State.Idle && transform.position.y != _startPos.y) 
        {
            transform.position = new Vector3(transform.position.x
                                            , Mathf.Lerp(transform.position.y,_startPos.y,Time.deltaTime * _speed)
                                            ,transform.position.z);
            if (Numbers.RangeOfFloats(transform.position.y, _startPos.y - .01f, _startPos.y + .01f)) 
            {
                transform.position = new Vector3(transform.position.x, _startPos.y, transform.position.z);
            }
        }
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
        if ((transform.position.x < _leftBounds || transform.position.x > _rightBounds) && Time.time > _dirChangeAcc) 
        {

            _dirChangeAcc = Time.time + dirChangeTime;
            _speed *= -1f;
        }
        if (_isPursuingPlayer && _player != null) 
        {
            
            float pSpeed = 10f;
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, _player.transform.position.x, Time.deltaTime * pSpeed),
                                             Mathf.Lerp(transform.position.y, _player.transform.position.y, Time.deltaTime * pSpeed),
                                             Mathf.Lerp(transform.position.z, _player.transform.position.z, Time.deltaTime * pSpeed));
            
        }
        if (_isPursuingPlayer)
        {
            _aeEffect.MakeGhosts = true;
        }
        else 
        {
            _aeEffect.MakeGhosts = false;
        }
    }

    private void SineWavePattern() 
    {

        Vector3 highPosition = new Vector3(transform.position.x, _startPos.y, transform.position.z);
        Vector3 lowPosition = new Vector3(transform.position.x, -2f, transform.position.z);
        if (Numbers.RangeOfFloats(transform.position.y, lowPosition.y - .3f, lowPosition.y + .3f) && !_lowSine)
        {
            transform.position = lowPosition;
            _lowSine = true;
            
        }
        if (Numbers.RangeOfFloats(transform.position.y, highPosition.y - .3f, highPosition.y + .3f)&&_lowSine)
        {
            transform.position = highPosition;
            _lowSine = false;

        }
        if (_lowSine)
        {
            float y = Mathf.Lerp(transform.position.y, highPosition.y, Time.deltaTime * Mathf.Abs(_speed));
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        else 
        {
            float y = Mathf.Lerp(transform.position.y, lowPosition.y, Time.deltaTime * Mathf.Abs(_speed));
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
    IEnumerator Hit() 
    {
        
        spriteRenderer.color = Color.red;
        if (!_isCritical) 
        {
            _regularHitCounter--;
            if (_regularHitCounter <= 0)
            {
                TransitionToCriticalState();
            }
        }
        yield return new WaitForSeconds(.15f);
        spriteRenderer.color = Color.white;
    }
   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && (!_isDead || _isBiting == 1))
        {
            Player player = other.GetComponent<Player>();
            player.Damage(1);
            _isPursuingPlayer = false;
        }

        if (other.tag == "Laser" && !_isDead) 
        {
            Laser laser = other.GetComponent<Laser>();
            if (!laser.IsEnemyLaser) 
            {
                if (_isShootable)
                {
                    if (_isCritical)
                    {
                        Damage(5 * laser.Damage);
                    }
                    else
                    {
                        Damage(laser.Damage);
                    }
                }
                else 
                {
                    //deflect the laser
                    
                    Vector2 collisionPoint = other.transform.position;           
                    
                    Vector2 triggerCenter = transform.position + _centerOffset;
                
                    Vector2 collisionDirection = collisionPoint - triggerCenter;
                    float angle = Mathf.Atan2(collisionDirection.y,collisionDirection.x) * Mathf.Rad2Deg;
                    //Debug.Log("This is the Deflection angle: " + angle);
                    other.transform.rotation = Quaternion.Euler(0f,0f,angle + 253);
                    _sfx.PlayOneShot(_sfxDeflect);
                }
            }
        }
    }
    public void Damage(int damage) 
    {
        
        _sfx.PlayOneShot(_sfxHit);
        _health -= damage;
        healthBar.SetDamage(damage);
        StartCoroutine(Hit());
        if (_health <= 0) 
        {
            Kill();
        }
    }
    private void MakeDecision() 
    {
        int rnd = Random.Range(0, 3);
        
        if (rnd == 0)
        {
            _didAnElectricalAttackLast = false;
            TransitionToActivateTeethState();
        }
        if (rnd == 1)
        {
            _didAnElectricalAttackLast = false;
            TransitionToChargeState();
        }
        if (rnd == 2 ) 
        {
            if (!_didAnElectricalAttackLast)
            {
                _didAnElectricalAttackLast = true;
                TransitionToElectricalChargeState();
            }
            else 
            {
                MakeDecision();
            }
            

        }
        
    }
    public void SetIsBiting(int biting) 
    {
        _isBiting = biting;
    }
    public void SetPursuit() 
    {
        _isPursuingPlayer = true;
    }
    public void StopPursuit() 
    {
        _isPursuingPlayer = false;
    }
    IEnumerator IdleTimer() 
    {
        float rnd = Random.Range(1f, 6f);
        yield return new WaitForSeconds(rnd);
        if (_canStart) 
        {
            MakeDecision();
        }
    }
    IEnumerator ShakeRoutine(float intensity, float frequency, float duration) 
    {
        Vector3 originalPos = transform.position;
        float time = Time.time;
        while (time + duration > Time.time)
        {
            Debug.Log("Shaking!");
            transform.position = new Vector3(Random.Range(-intensity,intensity), Random.Range(-intensity,intensity), 0);
            yield return new WaitForSeconds(frequency);
        }      
        
        transform.position = originalPos;
        yield return null;
        
    }
    private void Kill() 
    {
        StopAllCoroutines();
        TransitionToDeathState();
    }
    IEnumerator KillRoutine(float duration)
    {
        _bgMusic.clip = _originalBGMusic;
        _bgMusic.Stop();
        bossAnimator.SetTrigger("Critical");
        float time = Time.time;
        while (time + duration > Time.time) 
        {            
            GameObject explosion = Instantiate(_explosionFX,transform);
            explosion.transform.position = 
           new Vector2(Random.Range(transform.position.x +_centerOffset.x - 2, transform.position.x +_centerOffset.x + 2)
                     , Random.Range(transform.position.y + _centerOffset.y - 2, transform.position.y + _centerOffset.y + 2));
            yield return new WaitForSeconds(.2f);
        }

        //Play DestroyAnimation
        spriteRenderer.color = Color.white;
        bossAnimator.SetTrigger("Destroy");
        _sfx.PlayOneShot(_sfxDestroy);
        _critPartFX.SetActive(false);
        yield return new WaitForSeconds(4f);
        _bgMusic.Play();
        _player.AdjustScore(200);
        _spawnManager.KilledEnemy();
        Destroy(gameObject);

    }

    public void PlayBiteSound() 
    {
        _sfx.PlayOneShot(_sfxBite);
    }
    public void PlayBringTeethOutSFX() 
    {
        _sfx.PlayOneShot(_sfxTeethOut);
    }
   

    //===========================================State Machine Methods===================================
    private void TransitionToCriticalState() 
    {
        _critPartFX.SetActive(true);
        _speed = 0;
        _currentState = State.Critical;
        _isCritical = true;
        _isShootable = true;
        CriticalState critState = new CriticalState();
        critState.Initialize(bossAnimator);
        stateMachine.SetState(critState);
        StopAllCoroutines();
        StartCoroutine(CritStateTimer());   
    }
    IEnumerator CritStateTimer() 
    {
        _laserCharge.SetActive(false);
        _isCritical = true;
        _isShootable = true;
        yield return new WaitForSeconds(5f);
        _regularHitCounter = 5;
        _isCritical = false;
        TransitionToIdleState();
    }
    private void TransitionToIdleState() 
    {   
        
        _speed = 5;
        _currentState = State.Idle;
        _isShootable = false;
        _critPartFX.SetActive(false);
        _regularHitCounter = 5;
        IdleState idleState = new IdleState();
        idleState.Initialize(bossAnimator);
        stateMachine.SetState(idleState);
        StartCoroutine(IdleTimer());

    }
    private void TransitionToActivateTeethState() 
    {
        _speed = 0;
        _currentState = State.ActivateTeeth;
        _isShootable = true;
        ActivateTeethState teethState = new ActivateTeethState();
        teethState.Initialize(bossAnimator);
        stateMachine.SetState(teethState);
        StartCoroutine(BiteAttack());
    }
    IEnumerator BiteAttack() 
    {        
        _currentState = State.BiteAttack;
        yield return new WaitForSeconds(1.3f);
        _isShootable = false;
        BiteAttackState biteState = new BiteAttackState();
        biteState.Initialize(bossAnimator);
        stateMachine.SetState(biteState);
        StartCoroutine(MovementForBiteAttack(.2f));
        yield return new WaitForSeconds(1.3f);
        //transform.position = new Vector3(transform.position.x, _originalPosition.y, transform.position.z);
        _sfx.PlayOneShot(_sfxTeethIn);
        
        TransitionToIdleState();
    }
    IEnumerator MovementForBiteAttack(float newPosition) 
    {
        Vector3 originalPosition = transform.position;
        float transitionTime = 60f;
        float animationTime = 1.3f;
        float distTolerance = .1f;
        float yieldTime = .01f;
        while ( !Numbers.RangeOfFloats(transform.position.y,newPosition - distTolerance,newPosition+ distTolerance)) 
        {
            //transform.position = new Vector3(transform.position.x, transform.position.y - .3f, transform.position.z);
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y,newPosition,Time.deltaTime * transitionTime));
            yield return new WaitForSeconds(yieldTime);
        }
        
        yield return new WaitForSeconds(animationTime);
        while (!Numbers.RangeOfFloats(transform.position.y, originalPosition.y - distTolerance, originalPosition.y + distTolerance)) 
        {
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, originalPosition.y, Time.deltaTime * (transitionTime - 30)));
            yield return new WaitForSeconds(yieldTime);
        }
        transform.position = new Vector3(transform.position.x,originalPosition.y,originalPosition.z);
        
    }
    private void TransitionToChargeState() 
    {
        _speed = 0;
        _currentState = State.Charge;
        ChargeState chargeState = new ChargeState();
        chargeState.Initialize(bossAnimator);
        stateMachine.SetState(chargeState);
        _isShootable = true;
        StartCoroutine(ChargingPowerShotTimer());
    }
    IEnumerator ChargingPowerShotTimer() 
    {
        _laserCharge.SetActive(true);
        yield return new WaitForSeconds(_chargeTime);
        int r = Random.Range(0, 2);
        if (r == 0)
        {
            TransistionToPowerShotState();
        }
        else 
        {
            TransitionToMultishotState();
        }
        
        _laserCharge.SetActive(false);
    }
    private void TransistionToPowerShotState() 
    {
        
        _speed = 11;
        _currentState = State.PowerShot;
        _isShootable = false;
        PowerShootState shootState = new PowerShootState();
        shootState.Initialize(bossAnimator);
        stateMachine.SetState(shootState);
        StartCoroutine(ShootingPowerShotTimer());

    }
    IEnumerator ShootingPowerShotTimer() 
    {
        _bigLaser.SetActive(true);
        yield return new WaitForSeconds(5f);
        TransitionToIdleState();
        _bigLaser.SetActive(false);
    }

    private void TransitionToMultishotState() 
    {
        _currentState = State.Multishot;
        _isShootable = false;
        _speed = 0;
        MultiShotState multiShotState = new MultiShotState();
        multiShotState.Initialize(bossAnimator);
        _multishotFX.SetActive(true);
        for (int i = 0; i < _multishotProjectileAmt; i++) 
        {
            GameObject goLaser = Instantiate(_regularLaser);
            goLaser.transform.position = transform.position + _centerOffset;
            goLaser.transform.localScale = new Vector3(2, 2, 2);
            Laser laser = goLaser.GetComponent<Laser>();
            laser.MakeEnemyLaser();
            laser.SetSpeed(20);
            float angle = 360 / _multishotProjectileAmt;
            laser.transform.Rotate(Vector3.forward,angle * i);
        }
        StartCoroutine(ExitingMultiShot());
    }
    IEnumerator ExitingMultiShot() 
    {
        bossAnimator.SetTrigger("Shoot");
        yield return new WaitForSeconds(1f);
        _multishotFX.SetActive(false);
        TransitionToIdleState();
    }
    private void TransitionToDeathState() 
    {
        _largeCollider.SetActive(false);
        _laserCharge.SetActive(false);
        _bigLaser.SetActive(false);
        
        _isDead = true;
        _currentState = State.Die;
        _isShootable = true;
        DeathState deathState = new DeathState();
        deathState.Initialize(bossAnimator);
        stateMachine.SetState(deathState);
        StartCoroutine(KillRoutine(5));
    }

    private void TransitionToElectricalChargeState() 
    {

        _electircalChargeFX.SetActive(true);
        _isShootable = false;
        ElectrialChargeState chargeState = new ElectrialChargeState();
        chargeState.Initialize(bossAnimator);
        stateMachine.SetState(chargeState);
        StartCoroutine(ElectricChargeRoutine());
    }
    IEnumerator ElectricChargeRoutine() 
    {
        yield return new WaitForSeconds(1.5f);
        _electircalChargeFX.SetActive(false);
        TransitionToElectricalDischargeState();
    }
    private void TransitionToElectricalDischargeState() 
    {

        _speed = 11f * ChooseLeftOrRight();
        _currentState = State.ElectricAttack;
        _electricalDischargeFX.SetActive(true);
        _isShootable = false;
        ElectricalAttackState elecAtkState = new ElectricalAttackState();
        elecAtkState.Initialize(bossAnimator);
        stateMachine.SetState(elecAtkState);
        StartCoroutine(ElectricAttackRoutine());
    }
    private int ChooseLeftOrRight() 
    {
        int multiplier = Random.Range(-1, 2);
        if (multiplier == 0)
        {
            ChooseLeftOrRight();
        }
        else 
        {
            return multiplier;
        }
        return 1;
    }
        
    IEnumerator ElectricAttackRoutine() 
    {
        yield return new WaitForSeconds(10f);
        _electricalDischargeFX.SetActive(false);
        TransitionToIdleState();
    }

}
