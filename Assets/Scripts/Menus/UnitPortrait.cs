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
    public Image shield;
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

    public Text NotificationText;
    public float FixedNotificationDuration = 0.4f;
    public float notificationDuration = 0.4f;
    private float currentTime = 0f;
    private float blinkingCurrentTime = 0f;
    private bool activateNotification = false;
    private bool deactivateNotification = false;

    private float currentTime2 = 0f;
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
        portraitImage.sprite = unit.portraitSprite;

        if (setBarTo0)
        {
            healthBar.SetFillAmount(0);
            manaBar.SetFillAmount(0);
            if (xpBar != null) { xpBar.SetFillAmount(0); }
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
            UnitSelector.Notify += UpdateSelection;
        }
        UpdatePortrait(1f);
    }

    private void Unit_SpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER trigger)
    {
        if (trigger == Unit.UNIT_SPECIFIC_TRIGGER.DAMAGE_INSTANCE_END && shield != null)
        {
            if (shield != null)
            {
                if (linkedUnit.currentBlock > 0 || linkedUnit.CurrentStatus.Exists(x => x.status == CombatStatus.STATUS.PARRY))
                {
                    shield.gameObject.SetActive(true);
                    if (linkedUnit.currentBlock > 0)
                    {
                        blockImage.gameObject.SetActive(true);
                        blockImage.GetComponentInChildren<Text>().text = linkedUnit.currentBlock.ToString();
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

    public void UpdatePortrait(float duration=0.3f)
    {
        healthBar.StartTween(linkedUnit.CurrentHealth - phantomHealthLost, duration, linkedUnit.maxHealth);
        Unit_SpecificUpdate(Unit.UNIT_SPECIFIC_TRIGGER.DAMAGE_INSTANCE_END);
        if (linkedUnit.maxMana > 0) { manaBar.StartTween(linkedUnit.CurrentMana - phantomManaCost, duration, linkedUnit.maxMana); }
        if (xpBar != null) { xpBar.StartTween(linkedUnit.level.currentXP, duration, linkedUnit.level.nextLevelThreshold); }
        levelText.text = linkedUnit.level.currentLevel.ToString();
        if (talentPoints != null) { talentPoints.text = linkedUnit.CurrentTalentPoints.ToString(); }


        if (status != null)
        {
            foreach(CombatStatus cs in linkedUnit.CurrentStatus)
            {
                if(cs.ui == null)
                {
                    AddNewCombatStatusUI(cs);
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

    private void AddNewCombatStatusUI(CombatStatus cs)
    {
        CombatStatusUI ui = Instantiate(CombatManager.Instance.combaStatusUI);
        ui.transform.SetParent(status.transform);
        ui.transform.localScale = new Vector3(1, 1, 1);
        ui.Setup(cs);
        cs.ui = ui;
    }

    public void PlayNotificationText(string text, Color color)
    {
        if (NotificationText != null)
        {
            notificationDuration = FixedNotificationDuration * PlayerInfos.Instance.settings.eventSpeed;
            NotificationText.text = text;
            NotificationText.color = color;
            activateNotification = true;
            currentTime = 0f;
            NotificationText.CrossFadeAlpha(1f, notificationDuration, false);
        }
    }

    void Update()
    {
        if (NotificationText != null)
        {
            if (activateNotification)
            {
                currentTime += Time.deltaTime;
                if (currentTime > notificationDuration)
                {
                    currentTime = 0f;
                    activateNotification = false;
                    NotificationText.CrossFadeAlpha(0f, notificationDuration, false);
                }
            }
        }
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
                currentTime2 += Time.deltaTime;
                if (currentTime2 > animationTime * 2)
                {
                    currentTime2 = 0f;
                }
                else if (currentTime2 > animationTime)
                {
                    progression = 2 - (currentTime2 / animationTime);
                }
                else
                {
                    progression = currentTime2 / animationTime;
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
        linkedUnit.CurrentMana += Mathf.FloorToInt((float)linkedUnit.maxMana / 10f);
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
        linkedUnit.CurrentHealth += Mathf.FloorToInt((float)linkedUnit.maxHealth / 10f);
        linkedUnit.CurrentMana += Mathf.FloorToInt((float)linkedUnit.maxMana / 10f);
        UpdatePortrait();
    }


    public void RestToFull()
    {
        WinWindow window = GetComponentInParent<WinWindow>();

        while (linkedUnit.CurrentHealth != linkedUnit.maxHealth && window.remainingRestPoints > 0)
        {
            Rest();
        }
    }
    public void MedidateToFull()
    {
        WinWindow window = GetComponentInParent<WinWindow>();

        while (linkedUnit.CurrentMana != linkedUnit.maxMana && window.remainingRestPoints > 0)
        {
            Medidate();
        }
    }
    public void TrainToFull()
    {
        WinWindow window = GetComponentInParent<WinWindow>();

        while (window.remainingRestPoints > 0)
        {
            Train();
        }
    }
}
