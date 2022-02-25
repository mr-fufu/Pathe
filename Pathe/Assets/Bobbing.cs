using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobbing : MonoBehaviour
{
    private float bob = 0;
    private float offset;
    private Vector3 hold;
    private float ampl = 2;

    private void Start()
    {
        hold = transform.localPosition;
        offset = hold.y;
    }

    void Update()
    {
        bob = ampl * Mathf.Cos(Time.time * 6f);
        hold.y = bob + offset + ampl/2;
        transform.localPosition = hold;
    }
}
