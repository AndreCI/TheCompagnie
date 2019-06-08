using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class NotificationDisplay : MonoBehaviour
{
    public Image background;
    public Image glow;
    public List<Text> texts;
    public Image frontground;

    private bool active;
    private bool textActive;
    private float currentTime;
    public float fixedDuration;
    private float currentDuration;
    public float maxScale;

    private void Start()
    {
        CrossFadeAlpha(0, 0, false, new Color(1,1,1,0f));
    }

    public void Activate(string text, Color color, float fixedDuration)
    {
        fixedDuration = fixedDuration / 2f;
        foreach(Text t in texts)
        {
            t.text = text;
        }
        CrossFadeAlpha(1f, fixedDuration, false, color);
        active = true;
        textActive = true;
        currentDuration = fixedDuration;
    }

    private void CrossFadeAlpha(float alpha, float duration, bool ignoreTime, Color color)
    {
        background.CrossFadeAlpha(alpha, duration, ignoreTime);
        foreach(Text t in texts)
        {
            t.CrossFadeAlpha(alpha, duration, ignoreTime);
        }
        texts[0].color = color;
        frontground.CrossFadeAlpha(alpha, duration, ignoreTime);
    }

    private void Update()
    {
        if (active)
        {
            currentTime += Time.deltaTime;
            if (currentTime > currentDuration && textActive)
            {
                textActive = false;
                CrossFadeAlpha(0f, currentDuration, false, texts[0].color);
            }
            float progression = 0f;
            if (currentTime > currentDuration * 2)
            {
                active = false;
                currentTime = 0f;
            }
            else if (currentTime > currentDuration)
            {
                progression = 2 - (currentTime / currentDuration);
            }
            else
            {
                progression = currentTime / currentDuration;
            }
            glow.transform.localScale = new Vector3(maxScale * progression,
                maxScale * progression,
                1f);
        }
    }
}
