using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinWindow : MonoBehaviour
{
    public List<DuloGames.UI.UIStatsAdd> xpButtons;
    [HideInInspector] public List<UnitPortrait> portraits;
    public GameObject xpTextHolder;
    [HideInInspector] public List<Text> xpTexts;
    public int remainingXP;
    private List<Unit> units;
    public Button closeButton;

    public void Setup(int xpGain)
    {
        remainingXP = xpGain;
        xpTexts = new List<Text>(xpTextHolder.GetComponentsInChildren<Text>());
        foreach (Text xpText in xpTexts)
        {
            xpText.text = "XP : " + remainingXP.ToString();
        }
        units = new List<Unit>(PlayerInfos.Instance.compagnions);
        portraits = new List<UnitPortrait>(GetComponentsInChildren<UnitPortrait>());

        for (int i = 0; i < portraits.Count; i++)
        {
            if (i < units.Count)
            {
                portraits[i].transform.parent.parent.gameObject.SetActive(true);
                xpButtons[i].gameObject.SetActive(true);
                xpButtons[i].Setup(units[i], this);
                portraits[i].Setup(units[i]);
            }
            else
            {
                xpButtons[i].transform.parent.parent.gameObject.SetActive(false);
                portraits[i].gameObject.SetActive(false);
            }
        }
        closeButton.interactable = false;
        TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.COMBATWIN);

    }

    public void OnXPAdds()
    {
        portraits = new List<UnitPortrait>(GetComponentsInChildren<UnitPortrait>());

        for (int i = 0; i < units.Count; i++)
        {
            portraits[i].Setup(units[i], false);
        }
        remainingXP -= 1;
        foreach (Text xpText in xpTexts)
        {
            xpText.text = "XP : " + remainingXP.ToString();
        }
        if(remainingXP <= 0)
        {
            foreach(DuloGames.UI.UIStatsAdd b in xpButtons)
            {
                if(b.isActiveAndEnabled)
                    b.Toggle(false);
            }
            closeButton.interactable = true;
        }
    }




    public void Close()
    {
        SceneManager.LoadScene(1);
    }
}
