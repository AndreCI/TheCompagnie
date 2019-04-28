using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class CombatStatusUI : MonoBehaviour
{
    public Image icon;
    public Text infos;

    public CombatStatus combatStatus;

    public void Setup(CombatStatus s)
    {
        combatStatus = s;
    }

    public void UpdateData()
    {
        infos.text = combatStatus.value.ToString() + "/" + combatStatus.duration.ToString();
    }

}