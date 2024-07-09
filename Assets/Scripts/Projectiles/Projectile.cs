using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public int heal;
    public Effect effectToApply;

    public float moveSpeed;

    private Character target;

    // Called once the projectile has been spawned in.
    public void Initialize (Character targetChar)
    {
        target = targetChar;
    }

    void Update ()
    {
        // Move towards the target.
        if(target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position + new Vector3(0, 0.5f, 0), moveSpeed * Time.deltaTime);
        }
    }

    // Called when we hit our target.
    void ImpactTarget ()
    {
        if(damage > 0)
            target.TakeDamage(damage);

        if(heal > 0)
            target.Heal(heal);

        if(effectToApply != null)
            target.GetComponent<CharacterEffects>().AddNewEffect(effectToApply);
    }

    void OnTriggerEnter (Collider other)
    {
        // If we hit the correct target.
        if(target != null && other.gameObject == target.gameObject)
        {
            ImpactTarget();
            Destroy(gameObject);
        }
    }
}