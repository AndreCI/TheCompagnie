using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager instance;
    public static TutorialManager Instance { get => instance; }
    public enum TUTOTRIGGER { OVERWORLD, COMBAT, PARTYMENU, COMBATENDTURN, COMBATPLAY, COMBATATTACKED,COMBATWIN, COMBATMANA, WELCOME, ACTIONPOINT, PATCHNOTE,
    CHANNEL, LEARN, FORGET, CANCEL_CHANNEL, TALENTPOINT};
    public GameObject windows;
    public Text currentText;
    public Dictionary<TUTOTRIGGER, string> texts;
    public Dictionary<TUTOTRIGGER, bool> status;

    public delegate void DelegateTrigger(TUTOTRIGGER trigger);
    public event DelegateTrigger StartTrigger;
    public event DelegateTrigger EndTrigger;

    public static string versionNumber = "0.1.5.8";
    public static string patchNote = "Added spirit enemies. Improved enemies behavior, events, and fix some bugs."+
        "\nPrevious:\n"+
        "Added meta progression (variable starter)."+
        "Continuing big update. Tutorial arrows + updated enemies (beast only, undead temp removed"+
        
        "Continue big update. Meet Ember! Solved some bugs, notably infinite loop with status triggering themselves." +
        "Added a bit more logic to cards/effect in order to suit paladin/berserk theme of Ember."+
        
        "Big content update. There is certainly some bugs right now (next step is removing them ^^)" +
        "and the tutorial is not really up to date. " +
        "Now, cards/talent can apply status which can apply status or effect. Ex: inflame: target gains until end of combat: 'whenever this attacks, its target" +
        "gain 5 burns for 2 turns.'" +
        "\n also added a new player, 32 cards which use the new mechanics (frost!), rarity on cards (right now: common/rare/epics) and cards now depends on your" +
        "talents."+"Reworked status UI system!"+
        
        "Added talent system and basic talents!" +
        "Modify event structure and implementation, added void points. Added first events." +
        
        "Finished UI rework. Fixed some bug, improve intents"+
        
        "Reworked cards. Added color to cards in order to indentify owner."+
        
        
        "Starting to add more sounds! also some ui still. Doing basic attacks now :)" +

        "Added glowing effects on shard and other things. Still adding toolitps and other small improvements."+

        "Remade Win window. Added shards! Continue to upgrade UI. Did some cleaning, added fading glowing color to intent activated. Also simple animations and tooltips";

    public TUTOTRIGGER current;

    private bool activated;
    public float timePerWord = 0.05f;
    public float timePerLetter = 0.0f;
    public float currentTime;
    public Queue<char> tokenizedText;
    private char chrrentChar;
    public bool deactivateTutorialScroll = false;
    public void Start()
    {
        status = new Dictionary<TUTOTRIGGER, bool>();
        texts = new Dictionary<TUTOTRIGGER, string>();
        foreach(TUTOTRIGGER trigger in Enum.GetValues(typeof(TUTOTRIGGER)))
        {
            status.Add(trigger, false);
            
            texts.Add(trigger, GetText(trigger));
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Activate(TUTOTRIGGER.WELCOME, forceNonScrolling:true);
    }

    public void Update()
    {
        if (activated && tokenizedText.Count>0)
        {
            currentTime += (Time.deltaTime);// + rdn.Next(-1, 1));
            if ((currentTime) > timePerLetter)
            {
                currentTime -= timePerLetter;
                currentText.text += chrrentChar;
                chrrentChar = tokenizedText.Dequeue();
            }
        }else if (activated)
        {
            currentText.text += chrrentChar;
            activated = false;
        }
    }

    public void Activate(TUTOTRIGGER trigger, bool forceNonScrolling=false)
    {
        if (!status[trigger] && !PlayerSettings.Instance.disableTutorial){
            windows.gameObject.SetActive(true);
            current = trigger;
            StartTrigger?.Invoke(trigger);
            if (!deactivateTutorialScroll && !forceNonScrolling)
            {
                activated = true;
                tokenizedText = new Queue<char>(texts[trigger].ToCharArray());
                currentText.text = "";// tokenizedText.Dequeue();
                chrrentChar = tokenizedText.Dequeue();
            }
            else
            {
                currentText.text = texts[trigger];
            }
        }else if(trigger == TUTOTRIGGER.WELCOME)
        {
            windows.gameObject.SetActive(true);
            current = trigger;
            currentText.text = texts[trigger];
        }
    }
    
    public void Validate()
    {
        status[current] = true;
        EndTrigger?.Invoke(current);
    }
    public void DeactivateTutorialScroll(bool v)
    {
        deactivateTutorialScroll = v;
    }
    public void ResetTuto()
    {
        foreach (TUTOTRIGGER trigger in Enum.GetValues(typeof(TUTOTRIGGER)))
        {
            status[trigger] = false;
        }
        PlayerSettings.Instance.disableTutorial = false;
    }

    public void DeactivateTuto()
    {
        PlayerSettings.Instance.disableTutorial = true;
    }

    public void ShowPatchNotes()
    {
        windows.gameObject.SetActive(true);
        current = TUTOTRIGGER.PATCHNOTE;
        currentText.text = texts[TUTOTRIGGER.PATCHNOTE];
    }
    private string GetText(TUTOTRIGGER trigger)
    {
        switch (trigger)
        {
            case TUTOTRIGGER.PATCHNOTE:
            return "PATCH NOTE v" + versionNumber +".\n" +
                    patchNote;
            case TUTOTRIGGER.WELCOME:
                return "Welcome to The Compagnie. This is currently an alpha version, build v"+versionNumber+". \n" +
                    "Please note that this is far from a final build and that everything that you see here can be subject to change.\n"+
                    "It would help me a lot if you provide some feedback. In the meantime, I hope you enjoy this!";
            case TUTOTRIGGER.OVERWORLD:
                return "This is the Overworld. Right now, you are located on the bottom right of the map. \n" +
                    "You can click on the next node on the Overworld to start your first combat. \n" +
                    "Otherwise, you can see your team and learn more about it with the button Party and adjust the settings with the button Menu on the top right.";
                    
            case TUTOTRIGGER.PARTYMENU:
                return "Here, you can see your compagnions, their health, mana and current experience points. Their cards are also displayed. \n" +
                    "If you hover over them, you can see what they do.\n Finally, you can also improve your units by adding and removing cards to their decks "+
                    "and gaining talents";
            case TUTOTRIGGER.COMBAT:
                return "You are under attack! You can defend yourself and defeat the enemies by playing cards each turn. Each turn takes 10 ticks (green rectangle in the middle of the screen) and each cards has a delay, which determine when it will happens (bottom number on the card).";
            case TUTOTRIGGER.COMBATENDTURN:
                return "You are out of action! Each turn, you can play a limited number of cards. You gain 1 action point per turn!\n" +
                    "It's time to end the turn. Press end turn and see the action that you and the ennemies planned happend! Pay attention to the order in which the intents will go! \n" +
                   "You can also explore the intent by hovering over them.";

            case TUTOTRIGGER.COMBATPLAY:
                return "You can play a card by dragging it over a potential target (see the green sign). \n" +
                    "You can see the delay of each card on it (bottom number). The bigger it is, the latter it will happens.";
                    
            case TUTOTRIGGER.COMBATWIN:
                return "Congratulation! You just won a fight! \n" +
                    "After each fight, you receive experience, shards and the remaining time to spent on your units. "+ 
                    "After training, go to Party (top right) and click 'learn' to spend your skill points";
            case TUTOTRIGGER.COMBATATTACKED:
                return "You just got attacked!!" + "\n"
                    + "You can defend yourself by playing block cards before the enemy attacks you, or take the hit. If you health go down to 0, well you die.";
            case TUTOTRIGGER.COMBATMANA:
                return "You just used mana! \n" +
                    "Mana is used to perform powerful attack. It is displayed as the top number on a card. Each turn, you gain 2 mana. Your current mana is represented as a blue bar.\n" +
                    "If you don't have enough mana, you won't be able to play the card.";
            case TUTOTRIGGER.ACTIONPOINT:
                return "You gained another action point placeholder! During combat, they are represented as blue circle on the top left.\n Now you can bank " +
                    "action point as you still gain 1 per turn if you decide to pass your turn.";
            case TUTOTRIGGER.FORGET :
                return "You can forget any abilities by dragging them into these boxes. This will remove the corresponding card from the deck.";
            case TUTOTRIGGER.LEARN :
                return "You can learn new abilities by dragging any of the discovered cards into your deck. This will add the card permanently to your combat deck.";
            case TUTOTRIGGER.CANCEL_CHANNEL :
                return "A channel card has been cancel! Channel cancel happens when the source of the card takes damage from an attack. Any subsequent events from the card are removed.";
            case TUTOTRIGGER.CHANNEL :
                return "Channeling means that a card takes more than one tick to happen : it will takes the number of timestep corresponding to the value of the channel. \n" +
                    "Moreover, its effect and manacost will be duplicated. Dealing damage from an attack will cancel all instances of the card, so pay attention to who cast what "+
                    "and when!";
            case TUTOTRIGGER.TALENTPOINT:
                return "You have a talent point! Each time a unit gain a level, they gain an action point will can be spent on the talent tree. \n " +
                    "You can access the talent tree by clicking on the talent button.";
        }
        return "Tutorial text not found :( ";
    }
}