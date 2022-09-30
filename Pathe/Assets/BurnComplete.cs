using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnComplete : MonoBehaviour
{
    public bool burnComplete;

    void BurnDone()
    {
        burnComplete = true;
    }
}
