using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public abstract class Unit
{
    public enum DAMAGE_SOURCE_TYPE { NONE, ATTACK, SELF_ATTACK, STATUS};
    public enum UNIT_SPECIFIC_TRIGGER { NONE, ATTACKS, DAMAGE_BLOCKED, ATTACKED, DAMAGE_DEALT, DAMAGE_INSTANCE_END, HEAL };
    public CardDatabase.CARDCLASS availableCards;
    public string unitName;
    public int maxHealth;
    public int maxMana;
    public int maxAction;
    private int currentHealth;
    private int currentVoidPoints = 0;
    private int currentTalentPoints = 0;
    private int currentMana;
    private int currentAction;
    public int baseManaRegen;
    private int currentManaRegen;
    public Sprite combatSprite;
    public Sprite portraitSprite;
    public bool switchSprite;


    public abstract Color GetCurrentColor();
    public delegate void InfoUpdate();
    public event InfoUpdate NotifyUpdate;

    public delegate void SpecificUpdateDelegate(UNIT_SPECIFIC_TRIGGER trigger, Unit source);
    public event SpecificUpdateDelegate SpecificUpdate;

    public Leveling level;
    public int id;
    public static int currentId = 0;

    public int CurrentManaRegen { get => currentManaRegen + baseManaRegen; set => currentManaRegen = value; }

    public void TriggerSpecificUpdate(UNIT_SPECIFIC_TRIGGER trigger, Unit source=null)
    {
        Debug.Log("-------------------------");
        Debug.Log(trigger);
        Debug.Log(ToString());
        Debug.Log(source !=null ? source.ToString() : "");
        SpecificUpdate?.Invoke(trigger, source);
    }

    public int CurrentVoidPoints { get { return currentVoidPoints; }
        set { currentVoidPoints = value;
            if (currentVoidPoints < 0) { currentVoidPoints = 0; } }
    }

    public int CurrentBlock
    {
        get
        {
            int baseVal = 0;
            if (currentStatus != null)
            {
                foreach (CombatStatus s in currentStatus)
                {
                    if (s.status == CombatStatus.STATUS.BLOCK)
                    {
                        baseVal += s.value;
                    }
                }
            }
            return baseVal;
        }
    }

    public int CurrentStrength { get
        {
            int baseVal = 0;
            foreach(CombatStatus s in currentStatus)
            {
                if(s.status == CombatStatus.STATUS.BUFF_STR)
                {
                    baseVal += s.value;
                }else if(s.status == CombatStatus.STATUS.REDUCE_STR)
                {
                    baseVal -= s.value;
                //    if(baseVal < 0) { baseVal = 0; }
                }
            }return baseVal;
        } }
    public virtual Unit Setup()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentAction = maxAction;
        baseManaRegen = 2;
        Unit copy = GeneralUtils.Copy<Unit>(this);
        return copy;

    }

    public int CurrentHealth {  get => currentHealth; set {
            if(value > maxHealth)
            {
                value = maxHealth;
            }
            if (value < 0)
            {
                value = 0;
            }
            if (value == 0) { }
            else
            {
                CombatManager.Instance?.GetUnitUI(this)?.portraitInfos?.
                    EnqueueNotification((value - currentHealth).ToString(), (value - currentHealth) < 0 ? Color.red : Color.green);
            }
            currentHealth = value;
            NotifyUpdate?.Invoke();
        }
    }
    public int CurrentMana
    {
        get => currentMana; set
        {

            if (value > maxMana) { value = maxMana; }
            if (value < 0) { value = 0; }
            if(value == 0) { }
            else { CombatManager.Instance?.GetUnitUI(this)?.portraitInfos?.EnqueueNotification((value - currentMana).ToString(), Color.blue); }
            currentMana = value;
            NotifyUpdate?.Invoke();
        }
    }
    public int CurrentAction
    {
        get => currentAction; set
        {
            if (value > maxAction) { value = maxAction; }
            if (value < 0) { value = 0; }
            currentAction = value;
            NotifyUpdate?.Invoke();
        }
    }
    public int CurrentTalentPoints
    { get => currentTalentPoints; set
        {
            if(value < 0) { return; }
            currentTalentPoints = value;
            NotifyUpdate?.Invoke();
        }
    }
    public int baseSpeed;
    public int currentSpeed
    {
        get
        {
            int baseVal = 0;
            foreach (CombatStatus s in CurrentStatus)
            {
                if (s.status == CombatStatus.STATUS.BUFF_SPEED)
                {
                    baseVal += s.value;
                }
                else if (s.status == CombatStatus.STATUS.RECUDE_SPEED)
                {
                    baseVal -= s.value;
                }
            }
            return baseSpeed - baseVal;
        }
    }
    public int CurrentChannelValue
    {
        get
        {
            int baseVal = 0;
            foreach (CombatStatus cs in CurrentStatus)
            {
                if (cs.status == CombatStatus.STATUS.CHANNEL)
                {
                    baseVal += cs.value;
                }
            }
            return baseVal;
        }
    }

    public PersistentUnitDeck persistentDeck;

    private List<CombatStatus> currentStatus;

       public List<CombatStatus> CurrentStatus
    {
        get
        {
            if(currentStatus == null)
            {
                currentStatus = new List<CombatStatus>();
            }
            return currentStatus;
        }
        set { currentStatus = value; }
    }

    public void AddStatus(CombatStatus status)
    {
        if (currentStatus == null)
        {
            currentStatus = new List<CombatStatus>();
        }
        currentStatus.Add(status);
        NotifyUpdate?.Invoke();
    }

    public void RemoveStatus(CombatStatus status)
    {
        if (currentStatus == null)
        {
            currentStatus = new List<CombatStatus>();
        }
        else
        {
            currentStatus.Remove(status);
        }
        NotifyUpdate?.Invoke();
    }
    public CombatUnitDeck GetNewDeck()
    {
        return persistentDeck.GenerateCombatDeck();
    }


    public virtual void TakeDamage(int amount, DAMAGE_SOURCE_TYPE source_type, Unit source=null)
    {
        if(source_type == DAMAGE_SOURCE_TYPE.NONE) { Debug.Log("Issue with siurce tyoe"); Debug.Log(ToString()); Debug.Log(source.ToString());
        }
        foreach(CombatStatus cs in CurrentStatus.FindAll(x => x.status == CombatStatus.STATUS.PARRY ||
        x.status == CombatStatus.STATUS.BLOCK))
        {
            if(cs.status == CombatStatus.STATUS.BLOCK && 
                amount > 0 &&
                source_type != DAMAGE_SOURCE_TYPE.STATUS &&
                source_type != DAMAGE_SOURCE_TYPE.SELF_ATTACK)
            {
                cs.value -= amount;
                if(cs.value < 0) { amount = -1 * cs.value; cs.value = 0; }
                else
                {
                    AudioManager.Instance?.PlayFromSet(AudioSound.AUDIO_SET.ON_BLOCK);
                    TriggerSpecificUpdate(UNIT_SPECIFIC_TRIGGER.DAMAGE_BLOCKED, source);
                    CombatManager.Instance?.GetUnitUI(this).portraitInfos.EnqueueNotification(amount.ToString(), Color.gray);
                    amount = 0; }
                cs.CheckUpdate(forceAnimation:true);
            }

            if (cs.status == CombatStatus.STATUS.PARRY && 
                amount >0 && 
                source != null &&
                source_type != DAMAGE_SOURCE_TYPE.STATUS &&
                source_type != DAMAGE_SOURCE_TYPE.SELF_ATTACK)
            {
                amount = 0;
                cs.value -= 1;
                cs.CheckUpdate(forceAnimation: true);

                CombatManager.Instance?.GetUnitUI(this).portraitInfos.EnqueueNotification("Parry", Color.grey);
                AudioManager.Instance?.PlayFromSet(AudioSound.AUDIO_SET.ON_PARRY);
            }
        }
        if (amount > 0)
        {
            AudioManager.Instance?.PlayFromSet(AudioSound.AUDIO_SET.ON_HIT);
            if (source_type == DAMAGE_SOURCE_TYPE.ATTACK)
            {
                TriggerSpecificUpdate(UNIT_SPECIFIC_TRIGGER.DAMAGE_DEALT, source);
            }
            CurrentHealth -= amount;
            if (CurrentHealth <= 0)
            {
                Debug.Log("Unit is dead");
                Debug.Log(ToString());
                Debug.Log("Source: " + (source != null ? source.ToString() : "null"));
            }
        }
        TriggerSpecificUpdate(UNIT_SPECIFIC_TRIGGER.DAMAGE_INSTANCE_END, source);
    }


    public override string ToString()
    {
        if(this == null) { return "FOUND NULL"; }
        string data = "";
        data = "["+id + "] Unit of class " + availableCards.ToString() + "\n";
        data += "HP: " + (CurrentHealth).ToString() + "/" + maxHealth.ToString();
        data += "\n";
        data += "Mana: " + CurrentMana.ToString() + "/" + maxMana.ToString();
        data += "\n";
        data += "Update targets: ";
        data += "\n";
        if (NotifyUpdate != null) {
            foreach(Delegate d in NotifyUpdate.GetInvocationList())
            {
                data += "\t" +( d == null ? "null" : d.ToString());
                data += "\n";
            }
        }
        data += "Specific Update targets: ";
        data += "\n";
        if (SpecificUpdate != null)
        {
            foreach (Delegate d in SpecificUpdate.GetInvocationList())
            {
                data += "\t" + (d == null ? "null" : d.ToString());
                data += "\n";
            }
        }

        return data;
    }

    public void Heal(int amount)
    {
        if (amount > 0)
        {
            TriggerSpecificUpdate(UNIT_SPECIFIC_TRIGGER.HEAL, this);
            CurrentHealth += amount;
        }
    }

    public void GainMana(int amount)
    {
        CurrentMana += amount;
    }
    public void GainAction(int amount)
    {
        CurrentAction += amount;
    }

    public void GainXp(int amount)
    {
        level.GainXP(amount);
        NotifyUpdate?.Invoke();
    }
}
