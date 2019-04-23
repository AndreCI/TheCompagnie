using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : UICardDropZone
{
    public Unit unit;
    public Slider healthSlider;
    public Slider manaSlider;
    public Slider actionSlider;
    public Image Image;

    public void SetInfos(Unit unit_)
    {
        unit = unit_;
        Image.sprite = unit_.combatSprite;
        Image.preserveAspect = true;
        unit.NotifyUpdate += UpdateInfo;

        UpdateInfo();
    }
    public void UpdateInfo()
    {
        healthSlider.value = 1 - (float)unit.CurrentHealth / (float)unit.maxHealth;
        manaSlider.value = 1 - (float)unit.CurrentMana / (float)unit.maxMana;
        actionSlider.value = 1 - (float)unit.CurrentAction / (float)unit.maxAction;
    }
    private void OnDestroy()
    {
        unit.NotifyUpdate -= UpdateInfo;
    }
}