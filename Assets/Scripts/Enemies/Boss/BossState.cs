using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossState
{
    protected Animator animator;

    public void Initialize(Animator bossAnimator) 
    {
        animator = bossAnimator;
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();

}
public class BasicAttackState : BossState 
{
    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }
}
public class IdleState : BossState 
{
    public override void Enter()
    {
        animator.SetTrigger("Idle");
    }
    public override void Update()
    {
        
    }
    public override void Exit()
    {
        animator.ResetTrigger("Idle");
    }

}
public class ActivateTeethState : BossState
{
    public override void Enter()
    {
        animator.SetTrigger("ActivateTeeth");
    }

    public override void Exit()
    {
        animator.ResetTrigger("ActivateTeeth");
    }

    public override void Update()
    {
        
    }
}
public class BiteAttackState : BossState
{
    public override void Enter()
    {
        animator.SetTrigger("Bite");
    }

    public override void Exit()
    {
        animator.ResetTrigger("Bite");
    }

    public override void Update()
    {
        
    }
}
public class ChargeState : BossState 
{
    public override void Enter()
    {
        animator.SetTrigger("Charge");
    }

    public override void Exit()
    {
        animator.ResetTrigger("Charge");
    }

    public override void Update()
    {

    }
}
public class PowerShootState : BossState 
{
    public override void Enter()
    {
        animator.SetTrigger("Shoot");
    }

    public override void Exit()
    {
        animator.ResetTrigger("Shoot");
    }

    public override void Update()
    {

    }
}
public class CriticalState : BossState 
{
    public override void Enter()
    {
        animator.SetTrigger("Critical");
    }

    public override void Exit()
    {
        animator.ResetTrigger("Critical");
    }

    public override void Update()
    {

    }
}
public class MultiShotState : BossState
{
    public override void Enter()
    {
        animator.SetTrigger("Shoot");
    }

    public override void Exit()
    {
        animator.ResetTrigger("Shoot");
    }

    public override void Update()
    {

    }
}
public class ElectrialChargeState : BossState 
{
    public override void Enter()
    {
        animator.SetTrigger("ElectricalCharge");
    }

    public override void Exit()
    {
        animator.ResetTrigger("ElectricalCharge");
    }

    public override void Update()
    {

    }
}
public class ElectricalAttackState : BossState 
{
    public override void Enter()
    {
        animator.SetTrigger("ElectricalDischarge");
    }

    public override void Exit()
    {
        animator.ResetTrigger("ElectricalDischarge");
    }

    public override void Update()
    {

    }
}
public class DeathState : BossState 
{
    public override void Enter()
    {
        animator.SetTrigger("Critical");
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }
}