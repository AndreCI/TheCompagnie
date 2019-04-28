using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Tweens;
using static DuloGames.UI.Test_UIProgressBar;

public class UnitPortrait : MonoBehaviour
{
    public DuloGames.UI.Test_UIProgressBar healthBar;
    public DuloGames.UI.Test_UIProgressBar manaBar;
    public DuloGames.UI.Test_UIProgressBar xpBar;
    public List<ActionPoint> actionsPoints;
    public GameObject actionPointsHolder;
    public Image portraitImage;
    public Text levelText;
    public Text unitName;
    public Unit linkedUnit;

    public void Setup(Unit unit)
    {
        linkedUnit = unit;
        levelText.text = unit.level.currentLevel.ToString();
        unitName.text = unit.unitName;
        healthBar.m_TextValue = unit.maxHealth;
        manaBar.m_TextValue = unit.maxMana;
        if(xpBar != null) { xpBar.m_TextValue = unit.level.nextLevelThreshold; }
        portraitImage.sprite = unit.portraitSprite;

        healthBar.SetFillAmount(0);
        manaBar.SetFillAmount(0);
        if (xpBar != null) { xpBar.SetFillAmount(0); }
        actionsPoints = new List<ActionPoint>();
        if(actionPointsHolder != null) {
            actionsPoints = new List<ActionPoint>();
            foreach(ActionPoint ap in actionPointsHolder.GetComponentsInChildren<ActionPoint>())
            {
                ActionPoint Nap = ap.Setup(unit);
                if(Nap != null) { actionsPoints.Add(Nap); }
            }
        }
        UpdatePortrait(1f);
    }

    public void UpdatePortrait(float duration=0.3f)
    {
        healthBar.StartTween(linkedUnit.CurrentHealth, duration);
        manaBar.StartTween(linkedUnit.CurrentMana, duration);
        if (xpBar != null) { xpBar.StartTween(linkedUnit.level.currentXP, duration); }
    }
}
