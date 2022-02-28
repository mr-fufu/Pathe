using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class riseAndDisappear : MonoBehaviour
{
    bool startRise;
    bool endRise;
    float fadeAlpha;
    float waitTime;

    SpriteRenderer changeSprite;
    TextMeshPro changeText;

    Color spriteColor;
    Color textColor;

    Vector2 risePosition;
    float centerOffset;

    void Start()
    {
        changeSprite = GetComponent<SpriteRenderer>();
        changeText = transform.GetChild(0).GetComponent<TextMeshPro>();

        spriteColor = changeSprite.color;
        textColor = changeText.color;

        risePosition = transform.localPosition;
        setAlpha(0);
    }

    void Update()
    {
        if (startRise)
        {
            if (fadeAlpha < 1)
            {
                fadeAlpha += 5f * Time.deltaTime;
                setAlpha(fadeAlpha);

                transform.localPosition = new Vector2(risePosition.x - centerOffset, risePosition.y + fadeAlpha * 6f);
            }
            else
            {
                fadeAlpha = 1;
                startRise = false;
                waitTime = 0;
            }
        }
        else if (endRise)
        {
            if (fadeAlpha > 0)
            {
                fadeAlpha -= 0.5f * Time.deltaTime;
                setAlpha(fadeAlpha);

                transform.localPosition = new Vector2(risePosition.x - centerOffset, risePosition.y + (1 - fadeAlpha) * 3f + 6f);
            }
        }
        else
        {
            if (waitTime < 3)
            {
                waitTime += 2f * Time.deltaTime;
            }
            else
            {
                endRise = true;
            }
        }
    }

    public void initiateRise(int valueToSet, bool center)
    {
        if (valueToSet != 0)
        {
            startRise = true;

            string mod;
            if (valueToSet > 0)
            {
                mod = "+";
            }
            else
            {
                mod = "";
            }

            changeText.text = mod + valueToSet;

            setAlpha(0);
            fadeAlpha = 0;

            transform.localPosition = risePosition;
        }
        else
        {
            setAlpha(0);
        }

        if (center)
        {
            centerOffset = 6f + risePosition.x;
        }
        else
        {
            centerOffset = 0;
        }
    }

    public void completeRise()
    {
        startRise = false;
        setAlpha(0);
    }

    private void setAlpha(float setAlpha)
    {
        spriteColor.a = setAlpha;
        textColor.a = setAlpha;

        changeSprite.color = spriteColor;
        changeText.color = textColor;
    }
}
