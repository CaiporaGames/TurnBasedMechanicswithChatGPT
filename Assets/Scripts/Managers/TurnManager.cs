using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    private List<Character> turnOrder = new List<Character>();
    private int curTurnOrderIndex;
    private Character curTurnCharacter;

    [Header("Components")]
    public GameObject endTurnButton;

    // Singleton
    public static TurnManager instance;

    public event UnityAction onNewTurn;

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

    // Called when the game starts.
    public void Begin ()
    {
        GenerateTurnOrder(Character.Team.Player);
        NewTurn(turnOrder[0]);
    }

    // Decide the order of turns for the characters.
    void GenerateTurnOrder (Character.Team startingTeam)
    {
        if(startingTeam == Character.Team.Player)
        {
            turnOrder.AddRange(GameManager.instance.playerTeam);
            turnOrder.AddRange(GameManager.instance.enemyTeam);
        }
        else if(startingTeam == Character.Team.Enemy)
        {
            turnOrder.AddRange(GameManager.instance.enemyTeam);
            turnOrder.AddRange(GameManager.instance.playerTeam);
        }
    }

    // Called when a new turn is triggered.
    void NewTurn (Character character)
    {
        curTurnCharacter = character;
        onNewTurn?.Invoke();

        endTurnButton.SetActive(character.team == Character.Team.Player);
    }

    // Called when a character has ended their turn.
    public void EndTurn ()
    {
        curTurnOrderIndex++;

        if(curTurnOrderIndex == turnOrder.Count)
            curTurnOrderIndex = 0;

        while(turnOrder[curTurnOrderIndex] == null)
        {
            curTurnOrderIndex++;

            if(curTurnOrderIndex == turnOrder.Count)
                curTurnOrderIndex = 0;
        }

        NewTurn(turnOrder[curTurnOrderIndex]);
    }

    // Returns the character who's turn it currently is.
    public Character GetCurrentTurnCharacter ()
    {
        return curTurnCharacter;
    }
}