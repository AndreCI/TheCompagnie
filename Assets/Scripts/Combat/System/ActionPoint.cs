using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ActionPoint : MonoBehaviour
{

    public Image image;
    public int index;

    public Unit unit;

    private float currentTime = 0f;
    private float animationTime = 1f;
    public float minScale = 0.9f;
    public float maxScale = 1.3f;
    private bool activeAndAnimated = false;
    private bool lastTick = false;
    public bool ActiveAndAnimated
    {
        get => activeAndAnimated; set
        {
            image.gameObject.SetActive(value);
            activeAndAnimated = value;
        }
    }

    private void Update()
    {
        if (ActiveAndAnimated || lastTick)
        {
            if (!lastTick)
            {
                image.CrossFadeAlpha(1f, 0.2f, true);
            }
            float progression = 0f;
            currentTime += Time.deltaTime;
            lastTick = true;
            if (currentTime > animationTime * 2)
            {
                currentTime -= animationTime * 2;
                lastTick = false;
                image.CrossFadeAlpha(0f, 0.2f, true);
            }
            else if (currentTime > animationTime)
            {
                progression = 2 - (currentTime / animationTime);
            }
            else
            {
                progression = currentTime / animationTime;
            }
            image.gameObject.transform.localScale = new Vector3(maxScale * progression + minScale * (1 - progression),
                maxScale * progression + minScale * (1 - progression),
                1f);
        }
    }


    public ActionPoint Setup(Unit u)
    {
        if(u.maxAction <= index)
        {
            gameObject.SetActive(false);
           // Destroy(gameObject);
            return null;
        }
        u.NotifyUpdate += UpdateInfos;
        unit = u;
        lastTick = 1 <= index;
        return this;
    }

    public void UpdateInfos()
    {
        //  image.gameObject.SetActive(unit.CurrentAction > index);
        activeAndAnimated = unit.CurrentAction > index;
    }

    private void OnDestroy()
    {
        if(unit!=null)
            unit.NotifyUpdate -= UpdateInfos;
    }
}
