using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class EventWindow : MonoBehaviour
{
    private static EventWindow instance;
    public static EventWindow Instance { get => instance; }
    public GameObject titleHeader;
    public List<Button> options;
    public Text description;
    public Image background;
    public Image frontground;

    private OverworldEvent oEvent;

    void Start()
    {
        if(instance != null) { Destroy(gameObject); return; }
        instance = this;
        options[0].onClick.AddListener(() => Activate(0));
        options[1].onClick.AddListener(() => Activate(1));
        options[2].onClick.AddListener(() => Activate(2));
        options[3].onClick.AddListener(() => Activate(3));

        gameObject.SetActive(false);
    }
    public void Setup(OverworldEvent oEvent_){
        oEvent = oEvent_;
        foreach(Text t in titleHeader.GetComponentsInChildren<Text>())
        {
            t.text = oEvent.eventName;
        }
        description.text = oEvent.GetDescription();
        background.sprite = oEvent.background;
        frontground.sprite = oEvent.frontground;
        for(int i = 0; i < options.Count; i++)
        {
            if (i < oEvent.effects.Count)
            {
                options[i].gameObject.SetActive(true);
                options[i].interactable = oEvent.effects[i].ValidateCondition();
                options[i].GetComponentInChildren<Text>().text = oEvent.buttonsTexts[i];
                if (options[i].interactable)
                {
                    options[i].GetComponent<BasicToolTipActivator>().enabled = false;
                }
                else
                {
                    options[i].GetComponent<BasicToolTipActivator>().enabled = true;
                    options[i].GetComponent<BasicToolTipActivator>().description = oEvent.effects[i].condition.GetDescripton();
                }
            }
            else
            {
                options[i].gameObject.SetActive(false);
            }
        }
    }

    private void Activate(int i)
    {
        if(oEvent != null)
        {
            oEvent.ButtonClick(i);
        }
    }
    /*
    public Slider slider;
    public Button wait;
    public Button leave;

    public bool waiting;
    public float waitingTime;
    public float currentTime;
    public bool addComp = false;
    public bool tavern = false;
    public void Update()
    {
        if (waiting)
        {
            currentTime += Time.deltaTime;
            slider.value = currentTime / waitingTime;
            if(currentTime>= waitingTime)
            {
                if (!tavern)
                {
                    AddCompagnion();
                wait.interactable = true;
                }
                else
                {
                    FullHeal();
                }
                waiting = false;
                slider.value = 0;
                wait.interactable = true;
                leave.interactable = true;
            }
        }
    }

    public void Activate()
    {
        wait.interactable = true;
        leave.interactable = true;
        currentTime = 0;
        waiting = true;
    }

    public void AddCompagnion()
    {
        if (!addComp)
        {
            addComp = true;
            if (!tavern)
                PlayerInfos.Instance.AddCompagnion(PlayerInfos.Instance.compagnionsDatabase.Get(1));
        }
    }

    private void FullHeal()
    {
        foreach (Compagnion c in PlayerInfos.Instance.compagnions)
        {
            c.CurrentHealth = c.maxHealth;
            c.CurrentMana = c.maxMana;
        }
    }
}*/
}