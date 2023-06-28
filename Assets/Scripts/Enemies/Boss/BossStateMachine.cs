using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine
{
    private BossState currentState;
    public void SetState(BossState state)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = state;
        currentState.Enter();
        
    }

    public void Update() 
    {
        currentState.Update();
    }
}
