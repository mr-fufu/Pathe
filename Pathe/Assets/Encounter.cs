using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Encounter : MonoBehaviour
{
    public List<string> preTrackers;
    public NodeType type;
    public Sprite cardSprite;
    public string cardTitle;
    //[System.NonSerialized]
    public Text cardText;
    [System.NonSerialized]
    public List<choice> choices = new List<choice>();
    [System.NonSerialized]
    public bool seen = false;

    public bool zoom;

    public void Init()
    {
        choices.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            choices.Add(transform.GetChild(i).GetComponent<choice>());
        }

        foreach (choice choiceToInit in choices)
        {
            choiceToInit.Init();
        }

        cardText = gameObject.GetComponent<Text>();
    }
}
