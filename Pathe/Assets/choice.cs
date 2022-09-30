using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class choice : MonoBehaviour
{
    public List<string> preTrackers;
    public string choiceText;
    public int healthChange;
    public int wealthChange;
    public List<string> trackers;

    [System.NonSerialized]
    public bool subChoice;
    [System.NonSerialized]
    public List<choice> subChoices = new List<choice>();
    [System.NonSerialized]
    public Text resultText;

    public void Init()
    {
        subChoices.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            choice childChoice = transform.GetChild(i).gameObject.GetComponent<choice>();
            if (childChoice != null)
            {
                subChoices.Add(childChoice);
            }
        }

        if (subChoices.Count > 0)
        {
            subChoice = true;
        }

        resultText = gameObject.GetComponent<Text>();

        foreach (choice choiceToInit in subChoices)
        {
            choiceToInit.Init();
        }
    }
}

