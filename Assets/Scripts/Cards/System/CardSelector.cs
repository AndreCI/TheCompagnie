using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelector : MonoBehaviour
{
    private static CardSelector _instance;
    public static CardSelector Instance { get { return _instance; } }
    private List<Card> cardSelected;

    public delegate void AddToListener(List<Card> selected);
    public static event AddToListener Notify;

    void Start()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        cardSelected = new List<Card>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Unselect();
        }
    }

    public void ToggleSelection(Card c)
    {
        if (!cardSelected.Contains(c))
        {
            cardSelected.Add(c);
        }
        else
        {
            cardSelected.Remove(c);
        }
        Notify?.Invoke(cardSelected);
    }

    public void Unselect()
    {
        cardSelected = new List<Card>();
        Notify?.Invoke(cardSelected);
    }

    public IEnumerable<Card> GetSelectedCard()
    {
        return cardSelected;
    }

}
