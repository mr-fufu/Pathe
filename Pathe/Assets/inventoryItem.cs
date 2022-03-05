using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventoryItem : MonoBehaviour
{
    private SpriteRenderer thisSprite;
    private Color spriteColor;
    private float alpha;
    // Start is called before the first frame update
    void Start()
    {
        thisSprite = GetComponent<SpriteRenderer>();
        spriteColor = thisSprite.color;
        alpha = 0;
        colorSprite();
    }

    // Update is called once per frame
    void Update()
    {
        alpha += Time.deltaTime * 1.2f;
        colorSprite();
    }

    void colorSprite()
    {
        spriteColor.a = alpha;
        thisSprite.color = spriteColor;
    }
}
