using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UnitSelector
{
    private static UnitSelector _instance;
    public static UnitSelector Instance { get { if (_instance == null) { _instance = new UnitSelector(); } return _instance; } }
    private List<Unit> compagnionSelected;

    public delegate void AddToListener(List<Unit> selected);
    public static event AddToListener Notify;

    private UnitSelector()//void Start()
    {
        _instance = this;
        compagnionSelected = new List<Unit>();
    }

    public void AddCompagnionToSelection(Unit c)
    {
        Debug.Log(c.ToString());
        Debug.Log(compagnionSelected.Count);
        if (!compagnionSelected.Contains(c)){
            compagnionSelected.Add(c);
        }
        Notify?.Invoke(compagnionSelected);
    }

    public void Unselect()
    {
        compagnionSelected = new List<Unit>();
    }

    public IEnumerable<Unit> GetSelectedCompagnions()
    {
        return compagnionSelected;
    }
}
