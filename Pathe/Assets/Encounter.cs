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
    public Text cardText;
    public List<choice> choices;
    public bool seen = false;

    private void Start()
    {
        choices.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            choices.Add(transform.GetChild(i).GetComponent<choice>());
        }
    }
}
