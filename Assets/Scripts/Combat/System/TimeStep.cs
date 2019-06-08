using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;


public class TimeStep : MonoBehaviour
{
    public int index;
    public Image alternateMask;
    public Image image;
    public Image glow;
    public StretchyGridLayoutGroup enemiesIntents;
    public StretchyGridLayoutGroup compIntents;
    public float duration;

    public bool active;
    public bool activeBackwards;

    private void Start()
    {
        active = false;
        activeBackwards = false;
    }

    private void Update()
    {
        if (active)
        {
            activeBackwards = false;
            if(alternateMask.fillAmount >= 1) {
                active = false;
            }
            else
            {
                alternateMask.fillAmount += Time.deltaTime / duration;
                glow.color = new Color(glow.color.r, glow.color.g, glow.color.b, glow.color.a - Time.deltaTime / duration);
            }
        }
        else if (activeBackwards)
        {
            if (alternateMask.fillAmount <= 0)
            {
                activeBackwards = false;
            }
            else
            {
                alternateMask.fillAmount -= Time.deltaTime / duration;
            }
        }
    }

    public void Activate(float duration_ = 0.3f, bool backwards=false)
    {
        duration = duration_;
        if (backwards)
        {
            activeBackwards = true;
        }
        else
        {
            glow.color = new Color(glow.color.r, glow.color.g, glow.color.b, 1f);// (1f, 0f, true);
            active = true;
        }
    }


    public void Reset()
    {
        active = false;
        alternateMask.fillAmount = 0;
    }


}