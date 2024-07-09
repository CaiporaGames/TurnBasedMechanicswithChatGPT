using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatActionsUI : MonoBehaviour
{
    public GameObject panel;
    public CombatActionButton[] buttons;
    public GameObject descriptionPanel;
    public TextMeshProUGUI descriptionText;

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
        // Enable the UI if it's a player character's turn.
        if(TurnManager.instance.GetCurrentTurnCharacter().team == Character.Team.Player)
        {
            DisplayCombatActions(TurnManager.instance.GetCurrentTurnCharacter());
        }
        // Otherwise disable it.
        else
        {
            DisableCombatActions();
        }
    }

    // Display the requested character's combat actions.
    public void DisplayCombatActions (Character character)
    {
        panel.SetActive(true);

        for(int i = 0; i < buttons.Length; i++)
        {
            if(i < character.combatActions.Length)
            {
                buttons[i].gameObject.SetActive(true);
                buttons[i].SetCombatAction(character.combatActions[i]);
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    public void DisableCombatActions ()
    {
        panel.SetActive(false);
        DisableCombatActionDescription();
    }

    // Called when we hover over a combat action button.
    public void SetCombatActionDescription (CombatAction combatAction)
    {
        descriptionPanel.SetActive(true);
        descriptionText.text = combatAction.description;
    }

    public void DisableCombatActionDescription ()
    {
        descriptionPanel.SetActive(false);
    }
}