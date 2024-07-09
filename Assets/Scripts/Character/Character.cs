using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    public enum Team
    {
        Player,
        Enemy
    }

    [Header("Stats")]
    public Team team;
    public string displayName;
    public int curHp;
    public int maxHp;

    [Header("Combat Actions")]
    public CombatAction[] combatActions;

    [Header("Components")]
    public CharacterEffects characterEffects;
    public CharacterUI characterUI;
    public GameObject selectionVisual;
    public DamageFlash damageFlash;

    [Header("Prefabs")]
    public GameObject healParticlePrefab;

    public string aiCharacterName;

    private Vector3 standingPosition;

    public static UnityAction<Character> onCharacterDeath;

    void OnEnable ()
    {
        TurnManager.instance.onNewTurn += OnNewTurn;
    }

    void OnDisable ()
    {
        TurnManager.instance.onNewTurn -= OnNewTurn;
    }

    void Start ()
    {
        standingPosition = transform.position;

        characterUI.SetCharacterNameText(displayName);
        characterUI.UpdateHealthBar(curHp, maxHp);
    }

    // Called when a new turn has triggered.
    void OnNewTurn ()
    {
        characterUI.ToggleTurnVisual(TurnManager.instance.GetCurrentTurnCharacter() == this);
        characterEffects.ApplyCurrentEffects();
    }

    // Makes the character cast the requested combat action.
    public void CastCombatAction (CombatAction combatAction, Character target = null)
    {
        if(target == null)
            target = this;

        combatAction.Cast(this, target);
    }

    // Called when the character takes damage.
    public void TakeDamage (int damage)
    {
        curHp -= damage;

        characterUI.UpdateHealthBar(curHp, maxHp);

        damageFlash.Flash();

        if(curHp <= 0)
            Die();
    }

    // Called when the character is healed.
    public void Heal (int amount)
    {
        curHp += amount;

        if(curHp > maxHp)
            curHp = maxHp;

        characterUI.UpdateHealthBar(curHp, maxHp);
        Instantiate(healParticlePrefab, transform);
    }

    // Called when hp reaches 0.
    void Die ()
    {
        onCharacterDeath.Invoke(this);
        Destroy(gameObject);
    }

    // Used for the melee combat action.
    public void MoveToTarget (Character target, UnityAction<Character> arriveCallback)
    {
        StartCoroutine(MeleeAttackAnimation());

        IEnumerator MeleeAttackAnimation ()
        {
            // Move to the target.
            while(transform.position != target.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 10 * Time.deltaTime);
                yield return null;
            }

            // Attack the target.
            arriveCallback?.Invoke(target);

            // Move back to the standing position.
            while(transform.position != standingPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, standingPosition, 10 * Time.deltaTime);
                yield return null;
            }
        }
    }

    // Enable or disable the mouse selection visual.
    public void ToggleSelectionVisual (bool toggle)
    {
        selectionVisual.SetActive(toggle);
    }
}