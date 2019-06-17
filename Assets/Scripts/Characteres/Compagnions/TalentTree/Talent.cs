using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Talent : UnitStatus
{
    public enum UNIT_MODIFIER { HEALTH, MANA, ACTIONPOINT, MANAREGEN};
    public enum STATE { UNLOCKED, LOCKED, AVAILABLE, UNAVAILABLE}
    public string name;
    public List<CombatEffect> effects;
    public List<UNIT_MODIFIER> onUnlockEffect;
    public List<int> amounts;
    public Sprite icon;
    public GeneralUtils.SUBJECT_TRIGGER trigger;
    [HideInInspector] public STATE state;

    [HideInInspector] public bool inFightActivated;
   [HideInInspector]  public CombatStatusUI ui;

    [HideInInspector] public Compagnion unit;
    public void Setup(Compagnion owner)
    {
        unit = owner;
        inFightActivated = false;
        permanent = trigger != GeneralUtils.SUBJECT_TRIGGER.START_OF_COMBAT && effects.Count > 0;
        showUi = permanent;
        if (!permanent) { duration = 1; }
        target = unit as Unit;
        noValue = true;
    }

    public string GetOnUnlockDescription(UNIT_MODIFIER modifier, int amount)
    {
        string str = amount >= 0 ? "Gain " : "Lose ";
        str += Mathf.Abs(amount).ToString();
        switch (modifier)
        {
            case UNIT_MODIFIER.ACTIONPOINT:
                str += " action point placeholder";
                break;
            case UNIT_MODIFIER.HEALTH:
                str += " maximum health points";
                break;
            case UNIT_MODIFIER.MANA:
                str += " maximum mana";
                break;
            case UNIT_MODIFIER.MANAREGEN:
                str += " mana regeneration";
                break;
        }
        str += ".";
        return str;
    }

    public bool Unlockable {
        get {
            bool tnumber = unit.talentTree.talentPoint > 0;
            bool manaCost = true;
            bool healthCost = true;
            for (int i = 0; i < onUnlockEffect.Count; i++)
            {
                UNIT_MODIFIER type = onUnlockEffect[i];
                int amount = amounts[i];
                switch (type)
                {
                    case UNIT_MODIFIER.HEALTH:
                        if(unit.maxHealth + amount <= 0) { healthCost = false; }
                        break;
                    case UNIT_MODIFIER.MANA:
                        if (unit.maxMana + amount < 0) { manaCost = false; }

                        break;
                }
            }
            return tnumber && manaCost && healthCost;

        } }

    private string GetIssues()
    {
        List<string> issueList = new List<string>();
        string issueDescription = "";
        if (state == STATE.AVAILABLE || state == STATE.UNAVAILABLE)
        {
            bool tnumber = unit.talentTree.talentPoint > 0;
            bool manaCost = true;
            bool healthCost = true;
            for (int i = 0; i < onUnlockEffect.Count; i++)
            {
                UNIT_MODIFIER type = onUnlockEffect[i];
                int amount = amounts[i];
                switch (type)
                {
                    case UNIT_MODIFIER.HEALTH:
                        if (unit.maxHealth + amount<=0) { healthCost = false; }
                        break;
                    case UNIT_MODIFIER.MANA:
                        if (unit.maxMana + amount < 0) { manaCost = false; }

                        break;
                }
            }
            if (!tnumber) { issueList.Add("talent"); }
            if (!manaCost) { issueList.Add("maximum mana"); }
            if (!healthCost) { issueList.Add("maximum health"); }
            if(issueList.Count > 0)
            {
                issueDescription ="\nYou don't have enough ";
                for(int i = 0; i < issueList.Count; i++)
                {
                    issueDescription += issueList[i];
                    if(i == issueList.Count - 2)
                    {
                        issueDescription+= "and ";
                    }else if( i < issueList.Count - 2)
                    {
                        issueDescription += ", ";
                    }
                }issueDescription += " points to pay for this talent.";
            }
            if(state == STATE.UNAVAILABLE)
            {
                issueDescription += "\nYou need to unlock the previous talent to be able to unlock this one.";
            }
        }
        return issueDescription;
    }
    public List<string> GetDescriptions()
    {
        List<string> data = new List<string>();
        if(state == STATE.LOCKED)
        {
            data.Add("Talent lock, get the previous talent in order to discover this one.");
            data.Add("");
            data.Add("<i>Talents are randomized each play.</i>");
        }
        else
        {
            data.Add("<b>"+name+"</b>");
            data.Add("");
            for (int i = 0; i < onUnlockEffect.Count; i++)
            {
                data.Add(GetOnUnlockDescription(onUnlockEffect[i], amounts[i]));
            }
            if (GetTriggerDescr() != "")
            {
                data.Add(GetTriggerDescr());
            }
            foreach(CombatEffect e in effects)
            {
                data.Add(e.GetDescription());
            }
            data.Add(GetIssues());
        }
        return data;
    }
    private string GetTriggerDescr()
    {
        switch (trigger)
        {
            case GeneralUtils.SUBJECT_TRIGGER.START_OF_COMBAT:
                return "At the start of each combat: ";
            case GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN:
                return "At the start of each turn: ";
        }
        return "";
    }
    public void StartCombat()
    {
        if (state == STATE.UNLOCKED)
        {
            TurnManager.NotifyAll += Notified;
            inFightActivated = true;
        }
    }

    public void EndCombat()
    {
        if (state == STATE.UNLOCKED)
        {
            TurnManager.NotifyAll -= Notified;
            inFightActivated = false;
        }

    }

    public override void Notified(GeneralUtils.SUBJECT_TRIGGER currentTrigger)
    {
        if (trigger == currentTrigger)
        {
            if (ui != null)
            {
                ui.UpdateData();
                ui.setDestroy = false;
                ui.Trigger(TurnManager.Instance.currentDuration, currentTrigger != GeneralUtils.SUBJECT_TRIGGER.START_OF_TURN);
            }
            foreach (CombatEffect e in effects)
            {
                CombatStatusData data = new CombatStatusData(name+"\t\t\t Talent", CombatStatus.STATUS.STATUS_APPLY, icon, icon);
                e.Perform(new List<Unit> { unit }, unit, null, 0f, 0f, data);
            }
        }
    }

    public void OnUnlock()
    {
        for(int i = 0; i < onUnlockEffect.Count; i++)
        {
            UNIT_MODIFIER type = onUnlockEffect[i];
            int amount = amounts[i];
            switch (type)
            {
                case UNIT_MODIFIER.HEALTH:
                    unit.maxHealth += amount;
                    unit.CurrentHealth += amount;
                    break;
                case UNIT_MODIFIER.MANA:
                    unit.maxMana += amount;
                    unit.CurrentMana += amount;
                    break;
                case UNIT_MODIFIER.ACTIONPOINT:
                    TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.ACTIONPOINT);
                    unit.maxAction += amount;
                    break;
                case UNIT_MODIFIER.MANAREGEN:
                    unit.baseManaRegen += amount;
                    break;
            }
        }
        if(unit.talentTree.branch1.Contains(this)){
            unit.talentTree.branch1Value += 1;
        }
        else
        {
            unit.talentTree.branch2Value += 1;
        }
    }

    public override string GetAnimationName()
    {
        return "";
    }

    public override string GetDescription()
    {
        return string.Join("\n", GetDescriptions());
    }
}
