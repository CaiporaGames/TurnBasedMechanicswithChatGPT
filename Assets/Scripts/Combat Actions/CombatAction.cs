using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatAction : ScriptableObject
{
    public string displayName;// big fireball
    public string description;

    [Header("AI Section")]
    public string aiName;//BIGFIREBALL
    public string aiDescription;    
    public abstract void Cast(Character caster, Character target);
}