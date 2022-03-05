using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class riseAndDisappear : MonoBehaviour
{
    bool startRise;
    bool stayRise;
    bool endRise;
    float fadeAlpha;
    float waitTime;

    SpriteRenderer changeSprite;
    TextMeshPro changeText;

    Color spriteColor;
    Color textColor;

    Vector2 risePosition;
    float centerOffset;
    float riseDist;

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
                fadeAlpha += 2f * Time.deltaTime;
                riseDist += 1f * Time.deltaTime;
                setAlpha(fadeAlpha);

                transform.localPosition = new Vector2(risePosition.x - centerOffset, risePosition.y + riseDist * 3f);
            }
            else
            {
                fadeAlpha = 1;
                startRise = false;
                stayRise = true;
            }
        }
        else if (endRise)
        {
            if (fadeAlpha > 0)
            {
                fadeAlpha -= 0.8f * Time.deltaTime;
                riseDist += 1f * Time.deltaTime;
                setAlpha(fadeAlpha);

                transform.localPosition = new Vector2(risePosition.x - centerOffset, risePosition.y + riseDist * 3f);
            }
        }
        else if (stayRise)
        {
            if (waitTime < 1f)
            {
                waitTime += 2f * Time.deltaTime;

                riseDist += 1f * Time.deltaTime;
                transform.localPosition = new Vector2(risePosition.x - centerOffset, risePosition.y + riseDist * 3f);
            }
            else
            {
                endRise = true;
            }
        }
    }

    public void initiateRise(int valueToSet, bool center)
    {
        waitTime = 0;

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
