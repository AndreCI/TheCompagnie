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
    public DeckDisplayUI levelUpDisplay;

    public CardUI cardHolder;

    public void Start()
    {
        
    }

    public void SetInfos(IEnumerable<Unit> units = null)
    {
        if(units == null || units.Count() == 0) { units = PlayerInfos.Instance.compagnions; }

        //Portrait Setup
        unit = (new List<Unit>(units))[0];
        if (portrait != null)
        {
            portrait.Setup(unit);
        }
        //Level up setup
        if(levelUp != null)
        {
            levelUp.interactable = unit.level.talentPoints > 0;
        }
        if(image != null) { image.sprite = unit.combatSprite; }
        //Deck display setup
        
        deckDisplay.Setup(PlayerInfos.Instance.persistentPartyDeck.GetCards(units), 0);// PlayerInfos.Instance.persistentPartyDeck.GetCardSlots(units));

    }
    private void OnDestroy()
    {
        //unit.NotifyUpdate -= UpdateInfo;
    }

    

    private void OnEnable()
    {
        SetInfos(UnitSelector.Instance.GetSelectedUnit(UnitSelector.SELECTION_MODE.SELECT));
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
        levelUpDisplay.Setup(PlayerInfos.Instance.cardDatabase.GetRandomCards(3, PlayerInfos.Instance.cardDatabase.GetCardsFromClass(unit.availableCards)));
    }
}