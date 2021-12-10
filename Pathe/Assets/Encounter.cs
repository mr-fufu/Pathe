using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Encounter : MonoBehaviour
{
    public NodeType type;
    public Sprite cardSprite;
    public string cardTitle;
    public string cardText;
    public List<choice> choices;
}
