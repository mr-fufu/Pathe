using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splatter : MonoBehaviour
{
    public List<Sprite> splatters;
    private SpriteMask selfMask;

    void Start()
    {
        selfMask = gameObject.GetComponent<SpriteMask>();
        selfMask.sprite = splatters[Random.Range(0, splatters.Count)];
    }
}
