using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelector : MonoBehaviour
{
    private static UnitSelector _instance;
    public static UnitSelector Instance { get { return _instance; } }
    private List<Unit> unitSelected;

    public delegate void AddToListener(List<Unit> selected);
    public static event AddToListener Notify;

    void Start()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        unitSelected = new List<Unit>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Unselect();
        }
    }

    public void ToggleSelection(Unit c)
    {
        if (!unitSelected.Contains(c))
        {
            unitSelected.Add(c);
        }
        else
        {
            unitSelected.Remove(c);
        }
        Notify?.Invoke(unitSelected);
    }

    public void Unselect()
    {
        unitSelected = new List<Unit>();
        Notify?.Invoke(unitSelected);
    }

    public IEnumerable<Unit> GetSelectedUnit()
    {
        return unitSelected;
    }

}
