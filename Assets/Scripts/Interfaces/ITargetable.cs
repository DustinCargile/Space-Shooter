using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetable
{    
    public bool IsTargeted { get; set; }
    public int CurrentTargets { get; set; }
    public GameObject gameObject { get;}
}
