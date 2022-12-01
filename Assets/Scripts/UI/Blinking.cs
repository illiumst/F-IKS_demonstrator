using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blinking : MonoBehaviour
{
    public float spriteBlinkingTimer = 0.0f;
    public float spriteBlinkingMiniDuration = 2f;
    public float spriteBlinkingTotalTimer = 0.0f;
    public float spriteBlinkingTotalDuration = 3.0f;
    public bool startBlinking = false;
    void Update()
    {
        if (startBlinking == true)
        {
            SpriteBlinkingEffect();
        }
    }
    private void SpriteBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            startBlinking = false;
            spriteBlinkingTotalTimer = 0.0f;
            this.gameObject.GetComponent<Image>().enabled = true;   // according to your sprite
            return;
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {
            spriteBlinkingTimer = 0.0f;
            if (this.gameObject.GetComponent<Image>().enabled == true)
            {
                this.gameObject.GetComponent<Image>().enabled = false;  //make changes
            }
            else
            {
                this.gameObject.GetComponent<Image>().enabled = true;   //make changes
            }
        }
    }
}
