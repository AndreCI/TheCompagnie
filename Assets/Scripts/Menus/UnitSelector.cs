using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelector : MonoBehaviour
{
    public enum SELECTION_MODE { SELECT, TPOTENTIAL, TCURRENT}
    private static UnitSelector _instance;
    public static UnitSelector Instance { get { return _instance; } }
    //private List<Unit> unitSelected;
    private List<Unit> potentialTarget;
    private List<Unit> currentTarget;

    private Dictionary<SELECTION_MODE, List<Unit>> unitSelected;

    public delegate void AddToListener(List<Unit> selected, SELECTION_MODE mode);
    public static event AddToListener Notify;

    void Start()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        unitSelected = new Dictionary<SELECTION_MODE, List<Unit>>();//new List<Unit>();
        foreach(SELECTION_MODE mode in Enum.GetValues(typeof(SELECTION_MODE)))
        {
            unitSelected.Add(mode, new List<Unit>());
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Unselect();
        }
    }


    public void ToggleSelection(Unit c, SELECTION_MODE mode, bool forceAdd=false, bool forceRemove=false)
    {
        if (!unitSelected[mode].Contains(c) && !forceRemove)
        {
            unitSelected[mode].Add(c);
        }
        else if(unitSelected[mode].Contains(c) && !forceAdd)
        {
            unitSelected[mode].Remove(c);
        }
        Notify?.Invoke(unitSelected[mode], mode);
    }

    public void Unselect()
    {
        unitSelected = new Dictionary<SELECTION_MODE, List<Unit>>();//new List<Unit>();
        foreach (SELECTION_MODE mode in Enum.GetValues(typeof(SELECTION_MODE)))
        {
            unitSelected.Add(mode, new List<Unit>());
            Notify?.Invoke(unitSelected[mode], mode);
        }
    }

    public IEnumerable<Unit> GetSelectedUnit(SELECTION_MODE mode)
    {
        return unitSelected[mode];
    }

}
