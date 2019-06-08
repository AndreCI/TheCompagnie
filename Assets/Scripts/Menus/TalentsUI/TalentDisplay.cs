using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class TalentDisplay : BasicToolTipActivator
{
    public Button button;
    public Image icon;
    public Image glow;
    public TalentDisplay UIParent;
    public int index;

    [HideInInspector] public List<TalentDisplay> UIchildren;
    private TalentTreeDisplay display;
    public Talent linkedTalent;

    private void Start()
    {
        UIchildren = new List<TalentDisplay>();
        foreach(TalentDisplay t in transform.parent.transform.parent.GetComponentsInChildren<TalentDisplay>(true))
        {
            if(t.UIParent != null && t.UIParent == this)
            {
                UIchildren.Add(t);
            }
        }
    }

    public void Setup(Talent t, TalentTreeDisplay display_)
    {
        display = display_;
        linkedTalent = t;
        glow.color = display.linkedUnit.GetCurrentColor() ;
        glow.transform.localScale = new Vector3(0f, 0f, 1f);
        description = t.GetDescriptions();
    }

    public void UpdateInfo()
    {
        button.enabled = true;
        icon.sprite = linkedTalent.icon;
        button.interactable = false;
        switch (linkedTalent.state)
        {
            case Talent.STATE.LOCKED:
                icon.sprite = display.locked;
                break;
            case Talent.STATE.UNLOCKED:
                glow.transform.localScale = new Vector3(0f, 0f, 1f);
                button.targetGraphic.gameObject.SetActive(false);
                break;
            case Talent.STATE.AVAILABLE:

                button.interactable = linkedTalent.Unlockable;
                break;
            case Talent.STATE.UNAVAILABLE:
                break;
        }
        description = linkedTalent.GetDescriptions();
    }

    public void DiscoverChild()
    {
        if(linkedTalent.state == Talent.STATE.AVAILABLE && display.linkedUnit.talentTree.talentPoint > 0)
        {
            foreach(TalentDisplay d in UIchildren)
            {
                d.linkedTalent.state = Talent.STATE.UNAVAILABLE;
            }
        }
    }

    public void TalentAdd()
    {
        if (linkedTalent.state == Talent.STATE.AVAILABLE && display.linkedUnit.talentTree.talentPoint > 0)
        {
            display.linkedUnit.talentTree.talentPoint -= 1;
            linkedTalent.state = Talent.STATE.UNLOCKED;
            linkedTalent.OnUnlock();
            foreach (TalentDisplay d in UIchildren)
            {
                d.linkedTalent.state = Talent.STATE.AVAILABLE;
                d.DiscoverChild();
            }
            display.UpdateTalents();
        }
    }
     void Update()
    {
        if(linkedTalent.state == Talent.STATE.AVAILABLE)
        {
            glow.transform.localScale = new Vector3(display.maxScale * display.progression + display.minScale * (1 - display.progression),
                display.maxScale * display.progression + display.minScale * (1 - display.progression),
                1f);
            glow.color = new Color(glow.color.r, glow.color.g, glow.color.b, display.linkedUnit.talentTree.talentPoint > 0 ? 1f : 0.5f);
        }
    }

}