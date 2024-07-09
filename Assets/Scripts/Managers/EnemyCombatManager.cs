using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyCombatManager : MonoBehaviour
{
    [Header("AI")]
    public float minWaitTime = 0.2f;
    public float maxWaitTime = 1.0f;

    [Header("Attacking")]
    public float attackWeakestChance = 0.7f;

    [Header("Chance Curves")]
    public AnimationCurve healChanceCurve;

    private Character curEnemy;

    void OnEnable ()
    {
        TurnManager.instance.onNewTurn += OnNewTurn;
    }

    void OnDisable ()
    {
        TurnManager.instance.onNewTurn -= OnNewTurn;
    }

    // Called when a new turn is triggered.
    void OnNewTurn ()
    {
        // Is it an enemy character's turn?
        if(TurnManager.instance.GetCurrentTurnCharacter().team == Character.Team.Enemy)
        {
            curEnemy = TurnManager.instance.GetCurrentTurnCharacter();
            Invoke(nameof(DecideCombatAction), Random.Range(minWaitTime, maxWaitTime));
        }
    }

    // Decide which combat action to cast.
    async void DecideCombatAction ()
    {
        EnemyCombatDecision decision = await AIManager.Instance.DecideCombatAction(curEnemy);
        CastCombatAction(decision.combatAction, decision.target);
    }

    // Casts the requested combat action upon the requested target.
    void CastCombatAction (CombatAction combatAction, Character target)
    {
        if(curEnemy == null)
        {
            EndTurn();
            return;
        }
        string combatLog = $"{TurnManager.instance.GetCurrentTurnCharacter().aiCharacterName} casted " +
            $"{combatAction.aiName} upon {target.aiCharacterName}";

        AIManager.Instance.AddCombatLog(combatLog);

        curEnemy.CastCombatAction(combatAction, target);
        Invoke(nameof(EndTurn), Random.Range(minWaitTime, maxWaitTime));
    }

    // Called once the enemy has finished with their turn.
    void EndTurn ()
    {
        TurnManager.instance.EndTurn();
    }

    // Returns the percentage of health remaining for the requested character.
    // e.g. 15/20 hp = 0.75f
    float GetHealthPercentage (Character character)
    {
        return (float)character.curHp / (float)character.maxHp;
    }

    // Does the enemy have a combat action of the requested type? (melee, ranged, heal, etc).
    bool HasCombatActionOfType (System.Type type)
    {
        foreach(CombatAction ca in curEnemy.combatActions)
        {
            if(ca.GetType() == type)
                return true;
        }

        return false;
    }

    // Returns a random melee or ranged combat action from the enemy's combat action list.
    CombatAction GetDamageCombatAction ()
    {
        CombatAction[] ca = curEnemy.combatActions.Where(x => x.GetType() == typeof(MeleeCombatAction) || x.GetType() == typeof(RangedCombatAction)).ToArray();

        if(ca == null || ca.Length == 0)
            return null;

        return ca[Random.Range(0, ca.Length)];
    }

    // Returns a random heal combat action from the enemy's combat action list.
    CombatAction GetHealCombatAction ()
    {
        CombatAction[] ca = curEnemy.combatActions.Where(x => x.GetType() == typeof(HealCombatAction)).ToArray();

        if(ca == null || ca.Length == 0)
            return null;

        return ca[Random.Range(0, ca.Length)];
    }

    // Returns a random effect combat action from the enemy's combat action list.
    CombatAction GetEffectCombatAction ()
    {
        CombatAction[] ca = curEnemy.combatActions.Where(x => x.GetType() == typeof(EffectCombatAction)).ToArray();

        if(ca == null || ca.Length == 0)
            return null;

        return ca[Random.Range(0, ca.Length)];
    }

    // Returns the weakest character from the requested team (lowest health).
    Character GetWeakestCharacter (Character.Team team)
    {
        int weakestHp = 999;
        int weakestIndex = 0;

        Character[] characters = team == Character.Team.Player ? GameManager.instance.playerTeam : GameManager.instance.enemyTeam;

        for(int i = 0; i < characters.Length; i++)
        {
            if(characters[i] == null)
                continue;

            if(characters[i].curHp < weakestHp)
            {
                weakestHp = characters[i].curHp;
                weakestIndex = i;
            }
        }

        return characters[weakestIndex];
    }

    // Returns a random character from the requested team.
    Character GetRandomCharacter (Character.Team team)
    {
        Character[] characters = null;

        if(team == Character.Team.Player)
            characters = GameManager.instance.playerTeam.Where(x => x != null).ToArray();
        else if(team == Character.Team.Enemy)
            characters = GameManager.instance.enemyTeam.Where(x => x != null).ToArray();

        return characters[Random.Range(0, characters.Length)];
    }
}