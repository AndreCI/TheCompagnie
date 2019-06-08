using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class PartyMenu : MonoBehaviour
{
    public enum SLOT_TRIGGER { SETUP, SKILL, ON_DRAG, ON_END_DRAG}
    public Unit unit;
    public UnitPortrait portrait;
    public GameObject tab1;
    public Button levelUp;
    public Image image;
    public DeckDisplayUI deckDisplay;
    public GettableCardsDisplay levelUpDisplay;
    public RemovableCardDisplay pitDisplay;
    public TalentTreeDisplay talentTreeDisplay;
    public Image skillPointsImageIndicator;
    public Color skillPointColor;
    public Color voidPointColor;
    public Color talentPointColor;

    public CardUI cardHolder;

    public delegate void SkillUpdate(SLOT_TRIGGER trigger);
    public event SkillUpdate PartyMenuNotify;


    private float currentTime = 0f;
    private float animationTime = 1f;
    public float minScale = 0.9f;
    public float maxScale = 1.3f;
    private bool activeAndAnimated = false;
    private bool lastTick = false;

    public void Start()
    {
        levelUpDisplay.gameObject.SetActive(true);
        levelUpDisplay.gameObject.SetActive(false);

    }

    public void UnitySetInfos()
    {
        SetInfos(new List<Unit> { unit });
    }

    public void SetInfos(IEnumerable<Unit> units = null, IDeck deck=null)
    {
        if(units == null || units.Count() == 0 || units.Count(x=>x.GetType() == typeof(Enemy)) >0) {
            units = PlayerInfos.Instance.compagnions;
            UnitSelector.Instance.Unselect();
            UnitSelector.Instance.ToggleSelection(units.First(), UnitSelector.SELECTION_MODE.SELECT);

        }

        //Portrait Setup
        unit =units.First();
        if (portrait != null)
        {
            portrait.Setup(unit);
        }
        //Level up setup
        SetLearnForgetButton();
        if(image != null) { image.sprite = unit.combatSprite;
            image.transform.localRotation = Quaternion.Euler(0, unit.switchSprite ? 180 : 0, 0);
            image.transform.parent.GetComponentsInChildren<Image>(true).ToList().Find(x => x.name == "color").color = unit.GetCurrentColor();
        }

        if(talentTreeDisplay != null)
        {
            talentTreeDisplay.gameObject.SetActive(false);
        }
        //Deck display setup
        //units = new List<Unit>(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT));

        deckDisplay.Setup(deck ?? PlayerInfos.Instance.persistentPartyDeck.GetDeck(unit));
        TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.PARTYMENU);
       // PartyMenuNotify?.Invoke(SLOT_TRIGGER.SETUP);


    }
    private void OnDestroy()
    {
        //unit.NotifyUpdate -= UpdateInfo;
    }

    

    private void OnEnable()
    {
        IEnumerable<Unit> selected = UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT);
        SetInfos(selected);
    }
    public void ShowCardHolder(Card card, bool alternativeColor = false)
    {
        cardHolder.gameObject.SetActive(true);
        Color fixedColor = unit.GetCurrentColor();
        if (alternativeColor)
        {
            switch (card.branch)
            {
                case CardDatabase.BRANCH.B1:
                    fixedColor = (unit as Compagnion).branch1Color;
                    break;
                case CardDatabase.BRANCH.B2:
                    fixedColor = (unit as Compagnion).branch2Color;
                    break;
            }
        }
        cardHolder.Setup(card, fixedColor, card.manaCost > unit.maxMana ? Color.red : Color.white);
    }
    public void HideCardHolder()
    {
        cardHolder.gameObject.SetActive(false);
    }


    public void ShowLevelUp()
    {
        talentTreeDisplay.gameObject.SetActive(false);
        if (unit.CurrentVoidPoints > 0)
        {
            TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.FORGET);
            levelUpDisplay.gameObject.SetActive(false);
            SetInfos(new List<Unit> { unit });
            pitDisplay.gameObject.SetActive(true);
            pitDisplay.Setup(unit.CurrentVoidPoints > 5? 5 : unit.CurrentVoidPoints);
            unit.CurrentVoidPoints -= Mathf.Min(5, unit.CurrentVoidPoints);
            SetLearnForgetButton();
            lastTick = unit.CurrentTalentPoints <= 0 && unit.CurrentVoidPoints <= 0;
        }
        else
        {
            TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.LEARN);
            pitDisplay.gameObject.SetActive(false);
            SetInfos(new List<Unit> { unit });
            levelUpDisplay.gameObject.SetActive(true);
            levelUpDisplay.Setup((unit as Compagnion).DiscoverCard());
            unit.CurrentTalentPoints -= 1;
            if (portrait != null && portrait.talentPoints != null) { portrait.talentPoints.text = unit.CurrentTalentPoints.ToString(); }

            if (levelUp != null)
            {
                levelUp.interactable = unit.CurrentTalentPoints > 0;
                lastTick = unit.CurrentTalentPoints <= 0;
            }
            StartCoroutine(NotifyAllSlotsDelayed(SLOT_TRIGGER.SKILL, 0f));
        }

    }

    public IEnumerator NotifyAllSlotsDelayed(SLOT_TRIGGER trigger, float delayed=0f)
    {
        yield return new WaitForSeconds(delayed);
        PartyMenuNotify?.Invoke(trigger);
    }



    private void Update()
    {
        if (activeAndAnimated || lastTick)
        {
            if (!lastTick)
            {
                skillPointsImageIndicator.CrossFadeAlpha(1f, 0.2f, true);
            }
            float progression = 0f;
            currentTime += Time.deltaTime;
            if (currentTime > animationTime * 2)
            {
                currentTime -= animationTime * 2;
                if (lastTick)
                {
                    lastTick = false;
                    activeAndAnimated = false;
                    skillPointsImageIndicator.CrossFadeAlpha(0f, 0.2f, true);
                }
            }
            else if (currentTime > animationTime)
            {
                progression = 2 - (currentTime / animationTime);
            }
            else
            {
                progression = currentTime / animationTime;
            }
            skillPointsImageIndicator.gameObject.transform.localScale = new Vector3(maxScale * progression + minScale * (1 - progression),
                maxScale * progression + minScale * (1 - progression),
                1f);
        }
    }

    private void SetLearnForgetButton()
    {
        if (levelUp != null)
        {
            levelUp.interactable = unit.CurrentTalentPoints > 0 || unit.CurrentVoidPoints > 0;
            activeAndAnimated = unit.CurrentTalentPoints > 0 || unit.CurrentVoidPoints > 0 || (unit as Compagnion).talentTree.talentPoint > 0;
            if (!activeAndAnimated)
            {
                skillPointsImageIndicator.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
            }
            if (unit.CurrentVoidPoints > 0 || unit.CurrentTalentPoints > 0 || (unit as Compagnion).talentTree.talentPoint > 0)
            {
                skillPointsImageIndicator.color = unit.CurrentVoidPoints > 0 ? voidPointColor : ((unit as Compagnion).talentTree.talentPoint > 0 ? talentPointColor : skillPointColor);
            }
            levelUp.GetComponentInChildren<Text>().text = unit.CurrentVoidPoints > 0 ? "Forget" : "Learn";
            if (portrait.talentPoints != null) {
                portrait.talentPoints.text = unit.CurrentVoidPoints > 0? unit.CurrentVoidPoints.ToString() : unit.CurrentTalentPoints.ToString();
            }

        }
    }

    public Color GetGlowingCardColor(Card card)
    {
        Color fixedColor = Color.white;
        switch (card.branch)
        {
            case CardDatabase.BRANCH.B1:
                fixedColor = (unit as Compagnion).branch1Color;
                break;
            case CardDatabase.BRANCH.B2:
                fixedColor = (unit as Compagnion).branch2Color;
                break;
            case CardDatabase.BRANCH.BASIC:
                fixedColor = Compagnion.MixColor((unit as Compagnion).branch1Color,
                    (unit as Compagnion).branch2Color,
                    0.5f, 0.5f);
                break;
            case CardDatabase.BRANCH.NONE:
                fixedColor = unit.GetCurrentColor();
                break;
        }
        switch (card.rarity)
        {
            case CardDatabase.RARITY.NONE:
                fixedColor.a = 1f;
                break;
            case CardDatabase.RARITY.STARTER:
                fixedColor.a = 0.4f;
                break;
            case CardDatabase.RARITY.COMMON:
                fixedColor.a = 0.4f;
                break;
            case CardDatabase.RARITY.RARE:
                fixedColor.a = 0.7f;
                break;
            case CardDatabase.RARITY.EPIC:
                fixedColor.a = 1f;
                break;
        }
        return fixedColor;
    }
}