using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinWindow : MonoBehaviour
{
    [HideInInspector] public List<UnitPortrait> portraits;
    public GameObject xpTextHolder;
    public GameObject restHolder;
    public GameObject goldHolder;
    [HideInInspector] public List<Text> xpTexts;
    private int xp;
    private int restp;

    public int remainingRestPoints { get => restp; set { restRotation += 2; restp = value; } }
    private List<Unit> units;
    public Button closeButton;
    public Image glowingIndicator;

    private float currentTime = 0f;
    private float animationTime = 1.5f;
    private float minScale = 0.9f;
    private float maxScale = 1.2f;
    public float rotationSpeed = -1f;
    public float xpRotation = 0f;
    public float restRotation = 0f;
    private bool internalReadyToGo = false;
    public bool readyToGoIndicator
    {
        get => internalReadyToGo; set
        {
            glowingIndicator.gameObject.SetActive(value);
            internalReadyToGo = value;
        }
    }
    private void Update()
    {
        if (readyToGoIndicator)
        {
            float progression = 0f;
            currentTime += Time.deltaTime;
            if (currentTime > animationTime * 2)
            {
                currentTime = 0f;
            }
            else if (currentTime > animationTime)
            {
                progression = 2 - (currentTime / animationTime);
            }
            else
            {
                progression = currentTime / animationTime;
            }
            glowingIndicator.transform.localScale = new Vector3(maxScale * progression + minScale * (1 - progression),
                maxScale * progression + minScale * (1 - progression),
                1f);
        }
        xpTextHolder.transform.Rotate(new Vector3(0, 0, rotationSpeed - xpRotation));
        goldHolder.transform.Rotate(new Vector3(0, 0, rotationSpeed));
        restHolder.transform.Rotate(new Vector3(0, 0, rotationSpeed - restRotation));
        if(xpRotation > 0.1f)
        {
            xpRotation -= xpRotation/8f * Time.deltaTime;
        }
        if (restRotation > 0.1f)
        {
            restRotation -= restRotation/8f * Time.deltaTime;
        }
    }

    private void SetupHolder(GameObject holder, int value) {
        if(value <= 0)
        {
            holder.GetComponent<Image>().CrossFadeAlpha(0f, 1f, true);
            foreach(Image i in holder.GetComponentsInChildren<Image>())
                i.CrossFadeAlpha(0f, 1f, true);
        }
        List<Text> texts = new List<Text>(holder.transform.parent.GetComponentsInChildren<Text>());
        foreach(Text t in texts.FindAll(x=>!x.gameObject.isStatic))
        {
            t.text = value.ToString();
        }
    }

    public void Setup(int xpGain)
    {
        remainingRestPoints = Mathf.Max(Mathf.Max(7 - Mathf.FloorToInt(TurnManager.Instance.turnNumber/2f), 
                                                13 - TurnManager.Instance.turnNumber * 2), 
                                      3);
        xpRotation = xpGain;

        double u1 = 1.0 - Utils.rdx.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - Utils.rdx.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                     Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        double randNormal =
                     xpGain * 3 + xpGain * randStdNormal; //random normal(mean,stdDev^2)
        int goldGain = Mathf.Max((int)(randNormal), 3);
        SetupHolder(xpTextHolder, xpGain);
        SetupHolder(goldHolder, goldGain);
        WinWindow_NotifyUpdate();
        units = new List<Unit>(PlayerInfos.Instance.compagnions);
        portraits = new List<UnitPortrait>(GetComponentsInChildren<UnitPortrait>());

        for (int i = 0; i < portraits.Count; i++)
        {
            if (i < units.Count)
            {
                portraits[i].transform.parent.parent.gameObject.SetActive(true);
                portraits[i].Setup(units[i]);
                units[i].NotifyUpdate += WinWindow_NotifyUpdate;
            }
            else
            {
                portraits[i].gameObject.SetActive(false);
            }
        }
        closeButton.interactable = false;
        TutorialManager.Instance?.Activate(TutorialManager.TUTOTRIGGER.COMBATWIN);

        glowingIndicator.transform.localScale = new Vector3(minScale, minScale, 1f);
        StartCoroutine(addXpDelayed(xpGain, goldGain));
    }

    private IEnumerator addXpDelayed(int xpGain, int goldGain)
    {
        yield return new WaitForSeconds(1.5f * PlayerInfos.Instance.settings.eventSpeed);
        foreach (Unit u in units)
        {
            int cLevel = u.level.currentLevel;
            u.GainXp(xpGain);
            if(u.level.currentLevel != cLevel)
            {
                portraits.Find(x => x.linkedUnit == u).glowingIndicatorOn(1, true);
            }
            portraits.Find(x => x.linkedUnit == u).Setup(u, false);
        }

        PlayerInfos.Instance.CurrentShards += goldGain;
        yield return new WaitForSeconds(1f * PlayerInfos.Instance.settings.eventSpeed);

        closeButton.interactable = true;

    }

    private void WinWindow_NotifyUpdate()
    {
        if(this == null || gameObject == null) { return; }
        SetupHolder(restHolder, remainingRestPoints);
        if(remainingRestPoints <= 0)
        {
            DeactivateRewards();
        }
    }

    private void DeactivateRewards()
    {
        foreach(Button b in GetComponentsInChildren<Button>())
        {
            b.interactable = false;
        }
        closeButton.interactable = true;
        readyToGoIndicator = true;

 
    }
    private void OnDisable()
    {
        if (units != null && units.Count > 0)
        {
            foreach (Unit unit in units)
            {
                unit.NotifyUpdate -= WinWindow_NotifyUpdate;

            }
        }
    }
    private void OnDestroy()
    {
        foreach (Unit unit in units)
        {
            unit.NotifyUpdate -= WinWindow_NotifyUpdate;

        }
    }

    public void Close()
    {
        SceneManager.LoadScene(1);
        Destroy(gameObject);
    }
}
