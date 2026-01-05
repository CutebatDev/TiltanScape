using System;
using TMPro;
using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI victoryText;


    private void Start()
    {
        victoryText.text = "";
    }


    public void SetVictoryText(string text)
    {
        StartCoroutine(SetTextTimed(text, 2.0f));
    }
    
    
    IEnumerator SetTextTimed(string text, float seconds)
    {
        victoryText.text = text;
        yield return new WaitForSeconds(seconds);
        victoryText.text = "";
    }
}
