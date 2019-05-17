using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class PartyMenu : MonoBehaviour
{
    public Unit unit;
    public UnitPortrait portrait;
    public GameObject tab1;
    public Button levelUp;
    public Image image;
    public DeckDisplayUI deckDisplay;
    public GettableCardsDisplay levelUpDisplay;
    public Image skillPointsImageIndicator;

    public CardUI cardHolder;


    private float currentTime = 0f;
    private float animationTime = 1f;
    public float minScale = 0.9f;
    public float maxScale = 1.3f;
    private bool activeAndAnimated = false;
    private bool lastTick = false;

    public void Start()
    {
        
    }

    public void SetInfos(IEnumerable<Unit> units = null, IDeck deck=null)
    {
        if(units == null || units.Count() == 0) { units = PlayerInfos.Instance.compagnions; }

        //Portrait Setup
        unit = (new List<Unit>(units))[0];
        if (portrait != null)
        {
            portrait.Setup(unit);
            if (portrait.talentPoints != null) { portrait.talentPoints.text = unit.CurrentTalentPoints.ToString(); }
        }
        //Level up setup
        if (levelUp != null)
        {
            levelUp.interactable = unit.CurrentTalentPoints > 0;
            activeAndAnimated = unit.CurrentTalentPoints > 0;
            if (!activeAndAnimated)
            {
                skillPointsImageIndicator.transform.localScale = new Vector3(0.2f, 0.2f, 1f);
            }
        }
        if(image != null) { image.sprite = unit.combatSprite; }
        //Deck display setup
        units = new List<Unit>(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT));
        deckDisplay.Setup(deck ?? PlayerInfos.Instance.persistentPartyDeck.GetDeck(unit));
        TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.PARTYMENU);


    }
    private void OnDestroy()
    {
        //unit.NotifyUpdate -= UpdateInfo;
    }

    

    private void OnEnable()
    {
        IEnumerable<Unit> selected = UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT);
        if(!selected.All(x=>x.GetType() == typeof(Compagnion)))
        {
            selected = null;
        }
        SetInfos(selected);
    }
    public void ShowCardHolder(Card card)
    {
        cardHolder.gameObject.SetActive(true);
        cardHolder.Setup(card);
    }
    public void HideCardHolder()
    {
        cardHolder.gameObject.SetActive(false);
    }

    public void ShowLevelUp()
    {
        levelUpDisplay.gameObject.SetActive(true);
        levelUpDisplay.Setup((unit as Compagnion).DiscoverCard());
        unit.CurrentTalentPoints -= 1;
        if(portrait != null && portrait.talentPoints != null) { portrait.talentPoints.text = unit.CurrentTalentPoints.ToString(); }

        if (levelUp != null)
        {
            levelUp.interactable = unit.CurrentTalentPoints > 0;
            activeAndAnimated = unit.CurrentTalentPoints > 0;
        }
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
            lastTick = true;
            if (currentTime > animationTime * 2)
            {
                currentTime -= animationTime * 2;
                lastTick = false;
                skillPointsImageIndicator.CrossFadeAlpha(0f, 0.2f, true);
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
}