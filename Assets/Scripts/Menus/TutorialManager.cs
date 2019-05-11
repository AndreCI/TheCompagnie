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
    public enum TUTOTRIGGER { OVERWORLD, COMBAT, PARTYMENU, COMBATENDTURN, COMBATPLAY, COMBATATTACKED,COMBATWIN, COMBATMANA, WELCOME, ACTIONPOINT};
    public GameObject windows;
    public Text currentText;
    public Dictionary<TUTOTRIGGER, string> texts;
    public Dictionary<TUTOTRIGGER, bool> status;

    public TUTOTRIGGER current;

    private bool activated;
    public float timePerWord = 0.05f;
    public float timePerLetter = 0.0f;
    public float currentTime;
    public Queue<String> tokenizedText;
    private string currentWord;
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
            if ((currentTime) > timePerWord + currentWord.Count() * timePerLetter)
            {
                currentTime -= timePerWord + currentWord.Count() * timePerLetter;
                currentText.text += " " + currentWord;
                currentWord = tokenizedText.Dequeue();
            }
        }else if (activated)
        {
            currentText.text += " " + currentWord;
            activated = false;
        }
    }

    public void Activate(TUTOTRIGGER trigger, bool forceNonScrolling=false)
    {
        if (!status[trigger] && !PlayerSettings.Instance.disableTutorial){
            windows.gameObject.SetActive(true);
            current = trigger;
            if (!deactivateTutorialScroll && !forceNonScrolling)
            {
                activated = true;
                tokenizedText = new Queue<string>(texts[trigger].Split(' '));
                currentText.text = tokenizedText.Dequeue();
                currentWord = tokenizedText.Dequeue();
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

    private string GetText(TUTOTRIGGER trigger)
    {
        switch (trigger)
        {
            case TUTOTRIGGER.WELCOME:
                return "Welcome to The Compagnie. This is currently an alpha version, build v0.1.2. \n" +
                    "Please note that this is far from a final build and that everything that you see here can be subject to change.\n"+
                    "It would help me a lot if you provide some feedback. In the meantime, I hope you enjoy this!";
            case TUTOTRIGGER.OVERWORLD:
                return "This is the Overworld. Right now, you are located on the bottom right of the map. \n" +
                    "You can click on the next node on the Overworld to start your first combat. \n" +
                    "Otherwise, you can see your team and learn more about it with the button Party and adjust the settings with the button Menu on the top right.";
                    
            case TUTOTRIGGER.PARTYMENU:
                return "Here, you can see your compagnions, their health, mana and current experience points. Their cards are also displayed. \n" +
                    "If you hover over them, you can see what they do.\n Finally, you can also discover new cards when you level up. Be sure to have " +
                    "enough room!";
            case TUTOTRIGGER.COMBAT:
                return "You are under attack! You can defend yourself and defeat the enemies by playing cards each turn. The ennemies will also " +
                    "attack you!";
            case TUTOTRIGGER.COMBATENDTURN:
                return "You are out of action! Each turn, you can play a limited number of cards. You gain 1 action point per turn!\n" +
                    "It's time to end the turn. Press end turn and see the action that you and the ennemies planned happend! Pay attention to the order in which the intents will go! \n" +
                   "You can also explore the intent by hovering over them.";

            case TUTOTRIGGER.COMBATPLAY:
                return "You can play a card by dragging it over a potential target (see the green sign). \n" +
                    "You can see the delay of each card on it (bottom number). The bigger it is, the latter it will happens.";
                    
            case TUTOTRIGGER.COMBATWIN:
                return "Congratulation! You just won a fight! \n" +
                    "Now you can spend your experience points. If you have enough, will you gain a level. \n" +
                    "Each time you gain a level, you can acquire more cards and sometimes over bonuses. Go to Party (top right) and click discover skill. You can drag the card(s) " +
                    "that you like into your deck.";
            case TUTOTRIGGER.COMBATATTACKED:
                return "You just got attacked!!" + "\n"
                    + "You can defend yourself by playing block cards before the enemy attacks you, or take the hit. If you health go down to 0, well you die.";
            case TUTOTRIGGER.COMBATMANA:
                return "You just used mana! \n" +
                    "Mana is used to perform powerful attack. It is displayed as the top number on a card. Each turn, you gain 2 mana. Your current mana is represented as a blue bar.\n" +
                    "If you don't have enough mana, you won't be able to play the card.";
            case TUTOTRIGGER.ACTIONPOINT:
                return "You gained another action point placeholder! During combat, they are represented as blue square on the top left.\n Now you can bank " +
                    "action point as you still gain 1 per turn if you decide to pass your turn.";
        }
        return "Tutorial text not found :( ";
    }
}