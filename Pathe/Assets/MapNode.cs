using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType { CLEARING, STRUCTURE, FOREST, WETLAND, CASTLE }

public class MapNode : MonoBehaviour
{
    public List<node> NodeMappings;
    public NodeType selfType { get; private set; }
    public int direction;

    [System.Serializable] public struct node {public NodeType type; public Sprite sprite; }

    public void SetNodeType(NodeType setType)
    {
        foreach (node checkNode in NodeMappings)
        {
            if (checkNode.type == setType)
            {
                GetComponent<SpriteRenderer>().sprite = checkNode.sprite;
                selfType = setType;
                return;
            }
        }
    }
}
