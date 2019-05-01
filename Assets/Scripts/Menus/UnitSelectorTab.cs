using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectorTab : MonoBehaviour
{
    public List<DuloGames.UI.UITab> tabs;
    public List<Unit> units;
    

    private void OnEnable()
    {
        tabs = new List<DuloGames.UI.UITab>(GetComponentsInChildren<DuloGames.UI.UITab>(true));

        units = new List<Unit>(PlayerInfos.Instance.compagnions);
        for(int i=0; i < tabs.Count; i++)
        {
            tabs[i].gameObject.SetActive(i < units.Count);
            if(i< units.Count)
            {
                tabs[i].realTarget = units[i];
                SetupImage(tabs[i], units[i]);

            }
        }
    }

    private void SetupImage(DuloGames.UI.UITab tab, Unit u)
    {
        foreach (Image icon in tab.GetComponentsInChildren<Image>())
        {
            if (icon.name == "Icon")
            {
                icon.sprite = u.portraitSprite;
            }
        }
    }

}