using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatManager : MonoBehaviour
{
    public float selectionCheckRate = 0.02f;
    private float lastSelectionCheckTime;
    public LayerMask selectionLayerMask;

    private bool isActive;

    private CombatAction curSelectionCombatAction;
    private Character curSelectedCharacter;

    // Selection Flags
    private bool canSelectSelf;
    private bool canSelectTeam;
    private bool canSelectEnemies;

    // Singleton
    public static PlayerCombatManager instance;

    [Header("Components")]
    public CombatActionsUI combatActionsUI;

    void Awake ()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void OnEnable ()
    {
        TurnManager.instance.onNewTurn += OnNewTurn;
    }

    void OnDisable ()
    {
        TurnManager.instance.onNewTurn -= OnNewTurn;
    }

    // Called when a new turn has triggered.
    void OnNewTurn ()
    {
        // Enable player combat if it's our turn.
        if(TurnManager.instance.GetCurrentTurnCharacter().team == Character.Team.Player)
        {
            EnablePlayerCombat();
        }
        // Disable it otherwise.
        else
        {
            DisablePlayerCombat();
        }
    }

    // Allow the player to select combat actions and select targets.
    void EnablePlayerCombat ()
    {
        curSelectedCharacter = null;
        curSelectionCombatAction = null;
        isActive = true;
    }

    void DisablePlayerCombat ()
    {
        isActive = false;
    }

    void Update ()
    {
        // Only run update function if combat is enabled.
        if(!isActive || curSelectionCombatAction == null)
            return;

        // Check to see what the mouse is hovering over.
        if(Time.time - lastSelectionCheckTime > selectionCheckRate)
        {
            lastSelectionCheckTime = Time.time;
            SelectionCheck();
        }

        // When we click, cast the combat action.
        if(Mouse.current.leftButton.isPressed && curSelectedCharacter != null)
        {
            CastCombatAction();
        }
    }

    // See what we're hovering over.
    void SelectionCheck ()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 999, selectionLayerMask))
        {
            Character character = hit.collider.GetComponent<Character>();

            if(curSelectedCharacter != null && curSelectedCharacter == character)
            {
                return;
            }

            if(canSelectSelf && character == TurnManager.instance.GetCurrentTurnCharacter())
            {
                SelectCharacter(character);
                return;
            }
            else if(canSelectTeam && character.team == Character.Team.Player)
            {
                SelectCharacter(character);
                return;
            }
            else if(canSelectEnemies && character.team == Character.Team.Enemy)
            {
                SelectCharacter(character);
                return;
            }
        }

        UnSelectCharacter();
    }

    // Called when we click on a target character with a combat action selected.
    void CastCombatAction ()
    {
        string combatLog = $"{TurnManager.instance.GetCurrentTurnCharacter().aiCharacterName} casted " +
            $"{curSelectionCombatAction.aiName} upon {curSelectedCharacter.aiCharacterName}";

        AIManager.Instance.AddCombatLog(combatLog);

        TurnManager.instance.GetCurrentTurnCharacter().CastCombatAction(curSelectionCombatAction, curSelectedCharacter);
        curSelectionCombatAction = null;

        UnSelectCharacter();
        DisablePlayerCombat();
        combatActionsUI.DisableCombatActions();
        TurnManager.instance.endTurnButton.SetActive(false);

        Invoke(nameof(NextTurnDelay), 1.0f);
    }

    // Initiate the next character's turn.
    void NextTurnDelay ()
    {
        TurnManager.instance.EndTurn();
    }

    // Called when we hover over a character.
    void SelectCharacter (Character character)
    {
        UnSelectCharacter();
        curSelectedCharacter = character;
        character.ToggleSelectionVisual(true);
    }

    // Called when we stop hovering over a character.
    void UnSelectCharacter ()
    {
        if(curSelectedCharacter == null)
            return;

        curSelectedCharacter.ToggleSelectionVisual(false);
        curSelectedCharacter = null;
    }

    // Called when we click on a combat action in the UI panel.
    public void SetCurrentCombatAction (CombatAction combatAction)
    {
        curSelectionCombatAction = combatAction;

        canSelectSelf = false;
        canSelectTeam = false;
        canSelectEnemies = false;

        if(combatAction as MeleeCombatAction || combatAction as RangedCombatAction)
        {
            canSelectEnemies = true;
        }
        else if(combatAction as HealCombatAction)
        {
            canSelectSelf = true;
            canSelectTeam = true;
        }
        else if(combatAction as EffectCombatAction)
        {
            canSelectSelf = (combatAction as EffectCombatAction).canEffectSelf;
            canSelectTeam = (combatAction as EffectCombatAction).canEffectTeam;
            canSelectEnemies = (combatAction as EffectCombatAction).canEffectEnemy;
        }
    }
}