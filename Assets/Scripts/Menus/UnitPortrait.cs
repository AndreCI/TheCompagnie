using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Tweens;
using UnityEngine.EventSystems;

using static DuloGames.UI.Test_UIProgressBar;

public class UnitPortrait : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public DuloGames.UI.Test_UIProgressBar healthBar;
    public DuloGames.UI.Test_UIProgressBar blockBar;
    public GameObject shield;
    public Image blockImage;
    public DuloGames.UI.Test_UIProgressBar manaBar;
    public Image manaBlinkingMask;
    public DuloGames.UI.Test_UIProgressBar xpBar;
    public List<ActionPoint> actionsPoints;
    public GameObject actionPointsHolder;
    public Image portraitImage;
    public Text levelText;
    public Image levelImage;
    public Text talentPoints;
    public List<Image> glowingIndicator;
    public Text unitName;
    public Unit linkedUnit;
    public GridLayoutGroup status;
    public Image selectionImage;

    public NotificationDisplay notificationDisplay;
    private float blinkingCurrentTime = 0f;
    private bool activateNotification = false;
    private bool deactivateNotification = false;

    private float currentTime = 0f;
    private float animationTime = 1.5f;
    private float minScale = 0.9f;
    private float maxScale = 1.2f;
    private List<bool> internalReadyToGo;

    
    public void glowingIndicatorOn(int index, bool v)
    {
        glowingIndicator[index].gameObject.SetActive(v);
        internalReadyToGo[index] = v;
        
    }
    void Awake()
    {
        if (internalReadyToGo == null) { internalReadyToGo = new List<bool>(); }

        foreach (Image glow in glowingIndicator)
        {
            internalReadyToGo.Add(false);
        }
    }

    private float phantomHealth = 0f;
    public float phantomHealthLost
    {
        get
        {
            return phantomHealth;
        }
        set
        {
            phantomHealth = value;
            UpdatePortrait();
        }
    }
    private float phantomMana = 0f;
    public float phantomManaCost { get
        {
            return phantomMana;
        }
        set
        {
            phantomMana = value;
            activateBlinking = phantomMana > linkedUnit.CurrentMana;
            UpdatePortrait();
        }
    }
    private bool activateB = false;
    public bool activateBlinking { get { return activateB; }
        set { if (manaBlinkingMask != null) { manaBlinkingMask.gameObject.SetActive(value); } activateB = value; }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (selectionImage != null)
        {
            UnitSelector.Instance.ToggleSelection(linkedUnit, UnitSelector.SELECTION_MODE.SELECT);
            selectionImage.gameObject.SetActive(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT).Contains(linkedUnit));
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(xpBar == null) {
            if(linkedUnit == null) { gameObject.SetActive(false); return; }
            unitName.text = linkedUnit.unitName;
            levelImage.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (xpBar == null)
        {
            unitName.text = "";//linkedUnit.unitName;
            levelImage.gameObject.SetActive(false);
        }
    }

    public void Setup(Unit unit, bool setBarTo0=true)
    {
        linkedUnit = unit;
        levelText.text = unit.level.currentLevel.ToString();
        unitName.text = unit.unitName;
        healthBar.m_TextValue = unit.maxHealth;
        manaBar.m_TextValue = unit.maxMana;
        if(xpBar != null) { xpBar.m_TextValue = unit.level.nextLevelThreshold; }
        if(blockBar != null) { blockBar.m_TextValue = 0; }
        portraitImage.sprite = unit.portraitSprite;

        if (setBarTo0)
        {
            healthBar.SetFillAmount(0);
            manaBar.additionalText = "";
            manaBar.SetFillAmount(0);
            if (xpBar != null) { xpBar.SetFillAmount(0); }
            if (blockBar != null) { blockBar.SetFillAmount(0); }
        }
        if(levelImage!=null) { unitName.text = ""; levelImage.gameObject.SetActive(false); }
        actionsPoints = new List<ActionPoint>();
        if(actionPointsHolder != null) {
            actionsPoints = new List<ActionPoint>();
            foreach(ActionPoint ap in actionPointsHolder.GetComponentsInChildren<ActionPoint>())
            {
                ap.gameObject.SetActive(true);
                ActionPoint Nap = ap.Setup(unit);
                if(Nap != null) { actionsPoints.Add(Nap); }
            }
        }
        if(talentPoints != null) { talentPoints.text = unit.CurrentTalentPoints.ToString(); }

        unit.SpecificUpdate += Unit_SpecificUpdate;
        if (selectionImage != null)
        {
            selectionImage.color = linkedUnit.GetCurrentColor();
            UnitSelector.Notify += UpdateSelection;
        }
        
        UpdatePortrait(1f);
    }

    private void Unit_SpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER trigger, Unit source)
    {
        if (trigger == Unit.UNIT_SPECIFIC_TRIGGER.DAMAGE_INSTANCE_END && shield != null)
        {
            if (blockBar != null)
            {
                SetBlockBar(0.5f);
            }
            if (linkedUnit.CurrentBlock > 0 || linkedUnit.CurrentStatus.Exists(x => x.status == CombatStatus.STATUS.PARRY))
            {
                shield.gameObject.SetActive(true);
                if (linkedUnit.CurrentBlock > 0)
                {
                    blockImage.gameObject.SetActive(true);
                    blockImage.GetComponentInChildren<Text>().text = linkedUnit.CurrentBlock.ToString();
                    
                }
                else
                {
                    blockImage.gameObject.SetActive(false);
                }
                if (linkedUnit.CurrentStatus.Exists(x => x.status == CombatStatus.STATUS.PARRY))
                {
                    glowingIndicatorOn(0, true);
                }
                else
                {
                    glowingIndicatorOn(0, false);
                }
            }
            else
            {
                shield.gameObject.SetActive(false);
            }

        }
    }


    private void OnDestroy()
    {
        if(linkedUnit!=null)
            linkedUnit.SpecificUpdate -= Unit_SpecificUpdate;
        if (selectionImage != null)
        {
            UnitSelector.Notify -= UpdateSelection;

        }
    }

    private void SetBlockBar(float duration)
    {
        healthBar.forcedText = (linkedUnit.CurrentHealth - phantomHealthLost).ToString() + "/" + linkedUnit.maxHealth.ToString();

        float blockValue = linkedUnit.CurrentBlock > 0 ? linkedUnit.CurrentHealth - phantomHealthLost + linkedUnit.CurrentBlock : 0;
        float addToMaxHealth = linkedUnit.CurrentHealth + linkedUnit.CurrentBlock - phantomHealthLost > linkedUnit.maxHealth ? linkedUnit.CurrentBlock : 0;
        healthBar.StartTween(linkedUnit.CurrentHealth - phantomHealthLost, duration, linkedUnit.maxHealth + addToMaxHealth);
        blockBar.StartTween(blockValue, duration * 2f,
               linkedUnit.maxHealth + addToMaxHealth);
    }
    public void UpdatePortrait(float duration=0.3f)
    {
        if (blockBar != null)
        {
            SetBlockBar(duration);
        }
        else
        {
            healthBar.StartTween(linkedUnit.CurrentHealth - phantomHealthLost, duration, linkedUnit.maxHealth);
        }
        Unit_SpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER.DAMAGE_INSTANCE_END, null);
        healthBar.m_TextValue = linkedUnit.maxHealth;
        manaBar.m_TextValue = linkedUnit.maxMana;
        if (linkedUnit.maxMana > 0) {
            manaBar.StartTween(linkedUnit.CurrentMana - phantomManaCost, duration, linkedUnit.maxMana,
                 "\t" + (linkedUnit.CurrentManaRegen < 0 ? linkedUnit.CurrentManaRegen.ToString() : "+" + linkedUnit.CurrentManaRegen.ToString()));
           
        }
        if (xpBar != null) { xpBar.StartTween(linkedUnit.level.currentXP, duration, linkedUnit.level.nextLevelThreshold); }
        levelText.text = linkedUnit.level.currentLevel.ToString();
        if (talentPoints != null) { talentPoints.text = linkedUnit.CurrentTalentPoints.ToString(); }


        if (status != null)
        {
            foreach(CombatStatus cs in linkedUnit.CurrentStatus)
            {
                if(cs.ui == null && cs.showUi)
                {
                    AddNewCombatStatusUI(cs);
                }
            }if (linkedUnit.GetType() == typeof(Compagnion)) {
                foreach (Talent t in (linkedUnit as Compagnion).talentTree.GetActivatedTalents())
                {
                    if(t.ui == null && t.showUi)
                    {
                        AddNewTalentStatuisUI(t);
                    }
                }
            }
        }
        UpdateSelection(new List<Unit>(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT)), 
            UnitSelector.SELECTION_MODE.SELECT);
    }

    private void UpdateSelection(List<Unit> selected, UnitSelector.SELECTION_MODE mode)
    {
        if (selectionImage != null && mode == UnitSelector.SELECTION_MODE.SELECT)
        {

            selectionImage.gameObject.SetActive(selected.Contains(linkedUnit));
        }
    }
    private void AddNewTalentStatuisUI(Talent t)
    {
        CombatStatusUI ui = Instantiate(CombatManager.Instance.combaStatusUI);
        ui.transform.SetParent(status.transform);
        ui.transform.localScale = new Vector3(1, 1, 1);
        ui.Setup(t);
        t.ui = ui;
    }
    private void AddNewCombatStatusUI(CombatStatus cs)
    {
        CombatStatusUI ui = Instantiate(CombatManager.Instance.combaStatusUI);
        ui.transform.SetParent(status.transform);
        ui.transform.localScale = new Vector3(1, 1, 1);
        ui.Setup(cs);
        cs.ui = ui;
    }
    public void EnqueueNotification(string text, Color color)
    {
        if (notificationDisplay != null)
        {

            notificationDisplay.Activate(text, color, 
                Mathf.Min(TurnManager.Instance.currentEventTimeDuration,
                TurnManager.Instance.currentDuration));
        }
    }

    void Update()
    {
        if (manaBlinkingMask != null)
        {
            if (activateBlinking)
            {
                blinkingCurrentTime += Time.deltaTime;
                float blinkingVal = blinkingCurrentTime / 1.2f;//manaBlinkingMask.color.a;
                if (blinkingCurrentTime > 2.4f)
                {
                    blinkingCurrentTime = 0f;
                    blinkingVal = 0f;
                }
                else if (blinkingCurrentTime > 1.2f)
                {
                    blinkingVal = 2 - blinkingVal;
                }
                manaBlinkingMask.color = new Color(manaBlinkingMask.color.r, manaBlinkingMask.color.g, manaBlinkingMask.color.b, blinkingVal);

            }
        }
        for (int i = 0; i < glowingIndicator.Count; i++)
        {
            if (internalReadyToGo[i] && glowingIndicator[i] != null)
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
                glowingIndicator[i].transform.localScale = new Vector3(maxScale * progression + minScale * (1 - progression),
                    maxScale * progression + minScale * (1 - progression),
                    1f);
            }
        }
    }

    //Win Portrait methods


    public void Medidate()
    {
        WinWindow window = GetComponentInParent<WinWindow>();
        window.remainingRestPoints -= 1;
        linkedUnit.CurrentMana = linkedUnit.maxMana;//+= Mathf.FloorToInt((float)linkedUnit.maxMana / 10f);
        UpdatePortrait();

    }
    public void Train()
    {

        WinWindow window = GetComponentInParent<WinWindow>();
        window.remainingRestPoints -= 1;
        glowingIndicatorOn(0, true);
        linkedUnit.CurrentTalentPoints += 1;
        UpdatePortrait();
    }

    public void Rest()
    {
        WinWindow window = GetComponentInParent<WinWindow>();
        window.remainingRestPoints -= 1;
        linkedUnit.CurrentHealth += Mathf.FloorToInt((float)linkedUnit.maxHealth / 4f);
        linkedUnit.CurrentMana += Mathf.FloorToInt((float)linkedUnit.maxMana / 10f);
        UpdatePortrait();
    }

}
