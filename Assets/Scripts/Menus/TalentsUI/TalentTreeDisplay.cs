using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class TalentTreeDisplay : MonoBehaviour
{
    public Image branch1;
    public Image branch2;


    public Compagnion linkedUnit;

    public List<TalentDisplay> b1;
    public List<TalentDisplay> b2;
    public Sprite locked;
    public float progression;
    public float maxScale = 1.1f;
    public float minScale = 1f;
    public float currentTime;
    public float animationTime;

    private void OnEnable()
    {
        Setup(PlayerInfos.Instance.unitsWindow.unit as Compagnion);
    }

    public void Setup(Compagnion u)
    {
        b1 = new List<TalentDisplay>();
        b2 = new List<TalentDisplay>();
        b1.AddRange(branch1.GetComponentsInChildren<TalentDisplay>(true));
        b2.AddRange(branch2.GetComponentsInChildren<TalentDisplay>(true));
        linkedUnit = u;
        foreach(TalentDisplay t in b1)
        {
            t.Setup(u.talentTree.Get(t.index, 1), this);
        }
        foreach (TalentDisplay t in b2)
        {
            t.Setup(u.talentTree.Get(t.index, 2), this);
        }
        StartCoroutine(delayedUpdate(0.05f));
        
    }
    public IEnumerator delayedUpdate(float t)
    {
        yield return new WaitForSeconds(t);

        UpdateTalents();
    }

    public void UpdateTalents()
    {
        foreach(TalentDisplay d in b1)
        {
            d.DiscoverChild();
        }
        foreach(TalentDisplay d in b1)
        {
            d.UpdateInfo();
        }
        foreach (TalentDisplay d in b2)
        {
            d.DiscoverChild();
        }
        foreach (TalentDisplay d in b2)
        {
            d.UpdateInfo();
        }
        PlayerInfos.Instance.unitsWindow.portrait.UpdatePortrait();
    }

    private void Update()
    {

        currentTime += Time.deltaTime;
        if (currentTime > animationTime * 2)
        {
            currentTime -= animationTime * 2;
            progression = currentTime / animationTime;
        }
        else if (currentTime > animationTime)
        {
            progression = 2 - (currentTime / animationTime);
        }
        else
        {
            progression = currentTime / animationTime;
        }
    }
}