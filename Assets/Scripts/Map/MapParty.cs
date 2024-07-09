using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MapParty : MonoBehaviour
{
    public float moveSpeed;
    private bool isMovingToEncounter;

    // Move to the requested encounter, then call the onArriveCallback function.
    public void MoveToEncounter (Encounter encounter, UnityAction<Encounter> onArriveCallback)
    {
        if(isMovingToEncounter)
            return;

        isMovingToEncounter = true;
        StartCoroutine(Move());

        IEnumerator Move ()
        {
            transform.LookAt(encounter.transform.position);

            while(transform.position != encounter.transform.position)
            {
                transform.position = Vector3.MoveTowards(transform.position, encounter.transform.position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            isMovingToEncounter = false;
            onArriveCallback?.Invoke(encounter);
        }
    }
}