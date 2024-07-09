using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectInstance
{
    public Effect effect;
    public int turnsRemaining;

    public GameObject curActiveGameObject;
    public ParticleSystem curTickParticle;

    public EffectInstance (Effect effect)
    {
        this.effect = effect;
        turnsRemaining = effect.durationOfTurns;
    }
}