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

    public CardUI cardHolder;

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
            if (portrait.talentPoints != null) { portrait.talentPoints.text = unit.level.talentPoints.ToString(); }
        }
        //Level up setup
        if (levelUp != null)
        {
            levelUp.interactable = unit.level.talentPoints > 0;
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
        unit.level.talentPoints -= 1;
        if(portrait != null && portrait.talentPoints != null) { portrait.talentPoints.text = unit.level.talentPoints.ToString(); }

        if (levelUp != null)
        {
            levelUp.interactable = unit.level.talentPoints > 0;
        }
    }
}