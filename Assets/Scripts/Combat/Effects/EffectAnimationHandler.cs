using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

public class EffectAnimationHandler : MonoBehaviour
{
    public List<SpriteRenderer> sRenderers;
    public List<EffectAnimation> currentAnimation;
    public Queue<EffectAnimation> waitingList;
    public int waitingListIndex;

    public List<bool> playing;


    private void FixedUpdate()
    {
        for (int i = 0; i < playing.Count; i++)
        {
            if (i < currentAnimation.Count && currentAnimation[i] != null && playing[i])
            {
                playing[i] = currentAnimation[i].playing || currentAnimation[i].activate;
                currentAnimation[i].FixedUpdate(Time.deltaTime);
            }
            else if (i == waitingListIndex && waitingList != null && waitingList.Count > 0)
            {
                EffectAnimation e = waitingList.Dequeue();
                playing[i] = true;
                currentAnimation[i] = e;
                e.ActivateFromRenderer(sRenderers[i]);
                sRenderers[i].color = e.modifyColor.a > 0 ? e.modifyColor : new Color(1f, 1f, 1f, 1f);
                currentAnimation[i].FixedUpdate(Time.deltaTime);
            }
        }
    }

    private void OnDestroy()
    {
        TurnManager.NotifyAll -= TurnManager_NotifyAll;

    }

    public void Start() {
        waitingList = new Queue<EffectAnimation>();
        sRenderers = new List<SpriteRenderer>();
        currentAnimation = new List<EffectAnimation> { null};
        playing = new List<bool> { false};
        sRenderers.Add(GetComponentInChildren<SpriteRenderer>());
        TurnManager.NotifyAll += TurnManager_NotifyAll;
        waitingListIndex = 0;
    }

    private void TurnManager_NotifyAll(GeneralUtils.SUBJECT_TRIGGER trigger)
    {
        if(trigger == GeneralUtils.SUBJECT_TRIGGER.TIMESTEP_TICK)
        {
            for(int i = 0; i < playing.Count; i++) { playing[i] = false;  sRenderers[i].sprite = null; }
        }
    }

    private void AddRenderer()
    {
        SpriteRenderer nr = Instantiate(sRenderers[0]);
        nr.transform.SetParent(sRenderers[0].transform.parent);
        nr.transform.position = sRenderers[0].transform.position;
        sRenderers.Add(nr);
        playing.Add(false);
        currentAnimation.Add(null);
    }

    private int GetPlayableIndex(bool forcePlay)
    {
        for(int i = 0; i < playing.Count; i++)
        {
            if (!playing[i]) { return i; }
        }
        if (forcePlay)
        {
            AddRenderer();
            return playing.Count - 1;
        }return -1;
    }

    public void Play(EffectAnimation e, float timeFactor=1f, float forcedTime=0f, bool forcePlay=false)
    {
        if (e != null)
        {
            e = e.GetEffect();
            int index = GetPlayableIndex(forcePlay);
            if (forcePlay)
            {
                currentAnimation[index] = e;
                playing[index] = true;
                e.ActivateFromRenderer(sRenderers[index], timeFactor, forcedTime);
                sRenderers[index].color = e.modifyColor.a > 0 ? e.modifyColor : new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                if (waitingList == null) { waitingList = new Queue<EffectAnimation>(); }
                waitingList.Enqueue(e);
            }
        }
    }

    public void ClearQueue()
    {
        waitingList.Clear();
    }
}