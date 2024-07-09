using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUI : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public Image healthFill;
    public TextMeshProUGUI healthText;
    public Image turnVisual;

    void Update ()
    {
        transform.forward = transform.position - Camera.main.transform.position;
    }

    public void ToggleTurnVisual (bool toggle)
    {
        turnVisual.gameObject.SetActive(toggle);
    }

    public void SetCharacterNameText (string characterName)
    {
        characterNameText.text = characterName;
    }

    public void UpdateHealthBar (int curHp, int maxHp)
    {
        healthText.text = $"{curHp} / {maxHp}";
        healthFill.fillAmount = (float)curHp / (float)maxHp;
    }
}