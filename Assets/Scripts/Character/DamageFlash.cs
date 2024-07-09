using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    private Renderer[] rs;

    void Start ()
    {
        rs = GetComponentsInChildren<Renderer>();
    }

    // Called when the character takes damage.
    public void Flash ()
    {
        StartCoroutine(FlashCoroutine());

        IEnumerator FlashCoroutine ()
        {
            SetMREmission(Color.white);

            yield return new WaitForSeconds(0.05f);

            SetMREmission(Color.black);
        }
    }

    // Sets the renderer's material emission color.
    void SetMREmission (Color color)
    {
        for(int i = 0; i < rs.Length; i++)
        {
            rs[i].material.SetColor("_EmissionColor", color);
        }
    }
}