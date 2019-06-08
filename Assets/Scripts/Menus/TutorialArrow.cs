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
    private Image image;
    public TutorialManager.TUTOTRIGGER trigger;

    private float currentTime = 0f;
    public float animationTime;
    public float maxScale = 1.2f;
    public float minScale = 1f;

    private void Update()
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
    private void OnEnable()
    {
        image = GetComponent<Image>();
        image.CrossFadeAlpha(0, 0, true);
    }
    private void Start()
    {
        animationTime = 2f;
        image = GetComponent<Image>();
        image.CrossFadeAlpha(0, 0, true);
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
            image.CrossFadeAlpha(1f, animationTime, true);
        }
    }
    private void Instance_EndTrigger(TutorialManager.TUTOTRIGGER trigger)
    {

        
        image.CrossFadeAlpha(0f, animationTime, true);
        
    }
}
