using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    public Image blackBG;
    public bool ActivateFadeIn, ActivateFadeOut;

    // Start is called before the first frame update
    void Start()
    {
        ActivateFadeIn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ActivateFadeIn)
        {
            FadeIn();
        }

        if (ActivateFadeOut)
        {
            FadeOut();
        }
    }

    public void FadeIn()
    {
        Color color = blackBG.color;
        color.a -= Time.fixedDeltaTime / 2f;
        color.a = Mathf.Clamp01(color.a);
        blackBG.color = color;
        if (blackBG.color.a == 0f)
        {
            ActivateFadeIn = false;
        }
    }

    public void FadeOut()
    {
        Color color = blackBG.color;
        color.a += Time.fixedDeltaTime / 2f;
        color.a = Mathf.Clamp01(color.a);
        blackBG.color = color;
        if (blackBG.color.a == 1f)
        {
            ActivateFadeOut = false;
        }
    }
}
