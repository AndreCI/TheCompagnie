using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public Button endTurnButton;

    private static TurnManager instance;
    public static TurnManager Instance { get => instance; }

    void Start()
    {
        instance = this;
    }

    /*Turn slicing:
     * 1) Effect Phase
     * 2) Draw Phase
     * 3) Play Phase
     * 4) blabla TODO
     */
    public void StartTurn()
    {
        List<Card> drawnCards = CombatManager.Instance.compagnionDeck.Draw();
        Hand.Instance.AddToHand(drawnCards);
    }

    public void EndTurn()
    {
        StartTurn();
    }

}
