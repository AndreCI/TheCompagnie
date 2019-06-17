using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class CustomToggleGroup : MonoBehaviour
{
    public Dictionary<Toggle, bool> toggles;
    private void Awake()
    {
        List<Toggle> ttoggles = GetComponentsInChildren<Toggle>().ToList();
        toggles = new Dictionary<Toggle, bool>();
        foreach(Toggle t in ttoggles)
        {
            t.SetIsOnWithoutNotify(false);
            t.onValueChanged.AddListener(delegate { OnToggleChange(t); });
            toggles.Add(t, false);
        }
        toggles.First().Key.SetIsOnWithoutNotify(true);
        toggles[toggles.First().Key] = true;
    }


    public void OnToggleChange(Toggle current)
    {
        foreach (Toggle t in new List<Toggle>(toggles.Keys))
        {
            t.SetIsOnWithoutNotify(t == current);
            toggles[t] = t == current;


            
        }
    }

    public void SetInteractable(List<int> index, bool v)
    {
        int i = 0;
        foreach (Toggle t in toggles.Keys)
        {
            if (index.Contains(i)) { t.interactable = v; }
            i++;
        }
    }
    public int GetCurrentIndex()
    {
        int i = 0;
        foreach (Toggle t in toggles.Keys)
        {
            if (toggles[t]) { return i; }
            i++;
        }
        return -1;
    }
    public void SetBaseToggle()
    {
        toggles.Keys.First().isOn = true;
    }

    public Toggle GetCurrentToggle()
    {
        foreach(Toggle t in toggles.Keys)
        {
            if (toggles[t]) { return t; }
        }
        return null;
    }
}
