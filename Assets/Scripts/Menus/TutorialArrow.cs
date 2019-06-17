using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;


public class TutorialArrow : MonoBehaviour
{
    private List<Image> images;
    public TutorialManager.TUTOTRIGGER trigger;

    private float currentTime = 0f;
    public float delay = 0f;
    public float animationTime;
    public float maxScale = 1.2f;
    public float minScale = 1f;

    private void Update()
    {
        foreach (Image image in images)
        {
            if (image.color.a > 0)
            {
                float progression = 0f;
                currentTime += Time.deltaTime;
                if (currentTime > animationTime * 2)
                {
                    currentTime = 0f;
                }
                else if (currentTime > animationTime)
                {
                    progression = 2 - (currentTime / animationTime);
                }
                else
                {
                    progression = currentTime / animationTime;
                }
                transform.localScale = new Vector3(maxScale * progression + minScale * (1 - progression),
                    maxScale * progression + minScale * (1 - progression),
                    1f);

            }
        }

    }
    private void OnEnable()
    {
        images = GetComponentsInChildren<Image>().ToList();
        images.Add(GetComponent<Image>());
        foreach (Image image in images)
        {
            image.CrossFadeAlpha(0, 0, true);
        }
    }
    private void Start()
    {
        animationTime = 2f;
        images = GetComponentsInChildren<Image>().ToList();
        images.Add(GetComponent<Image>());
        foreach (Image image in images)
        {
            image.CrossFadeAlpha(0, 0, true);
            image.raycastTarget = false;
        }
        foreach (Text t in GetComponentsInChildren<Text>())
        {
            t.enabled = false;
        }
        TutorialManager.Instance.StartTrigger += Instance_StartTrigger;
        TutorialManager.Instance.EndTrigger += Instance_EndTrigger;
    }
    private void OnDestroy()
    {
        TutorialManager.Instance.StartTrigger -= Instance_StartTrigger;
        TutorialManager.Instance.EndTrigger -= Instance_EndTrigger;
    }
    private void Instance_StartTrigger(TutorialManager.TUTOTRIGGER trigger)
    {
        if(trigger == this.trigger && !PlayerSettings.Instance.disableTutorial)
        {
            foreach (Image image in images)
            {
                image.CrossFadeAlpha(1f, animationTime + delay, true);
            }
            foreach (Text t in GetComponentsInChildren<Text>())
            {
                t.enabled = true;
            }
        }
    }
    private void Instance_EndTrigger(TutorialManager.TUTOTRIGGER trigger)
    {

        foreach (Image image in images)
        {
            image.CrossFadeAlpha(0f, animationTime, true);
            image.raycastTarget = false;
        }
        foreach(Text t in GetComponentsInChildren<Text>())
        {
            t.enabled = false;
        }
        
    }
}
