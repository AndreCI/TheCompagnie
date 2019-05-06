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
    public Slider mask;
    public Image image;
    public GridLayoutGroup enemiesIntents;
    public GridLayoutGroup compIntents;
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
            if(mask.value >= 1) {
                active = false;
            }
            else
            {
                mask.value += Time.deltaTime / duration;
            }
        }
        else if (activeBackwards)
        {
            if (mask.value <= 0)
            {
                activeBackwards = false;
            }
            else
            {
                mask.value -= Time.deltaTime / duration;
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
            active = true;
        }
    }


    public void Reset()
    {
        active = false;
        mask.value = 0;
    }


}