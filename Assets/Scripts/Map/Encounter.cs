using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Encounter : MonoBehaviour, IPointerDownHandler
{
    public enum State
    {
        Locked,
        CanVisit,
        Visited
    }

    public CharacterSet enemySet;
    private State state;

    public Color lockedColor;
    public Color canVisitColor;
    public Color visitedColor;

    public Renderer mr;

    // Sets the encounter's state and changes color.
    public void SetState (State newState)
    {
        state = newState;

        switch(state)
        {
            case State.Locked:
            {
                mr.material.color = lockedColor;
                break;
            }
            case State.CanVisit:
            {
                mr.material.color = canVisitColor;
                break;
            }
            case State.Visited:
            {
                mr.material.color = visitedColor;
                break;
            }
        }
    }

    // Called when we click on the encounter collider.
    public void OnPointerDown(PointerEventData eventData)
    {
        if(state == State.CanVisit)
        {
            MapManager.instance.MoveParty(this);
        }
    }
}