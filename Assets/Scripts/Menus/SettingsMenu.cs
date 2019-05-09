using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider timeSpeed;
    public Slider eventSpeed;
    public Slider volume;
    public Toggle resetTuto;
    public Toggle deactivateTUto;
    public Toggle deactivateTutorialScroll;

    PlayerSettings settings;
    private void OnEnable()
    {
        settings = PlayerSettings.Instance;
        SetWindows();
    }

    private void SetWindows()
    {
        float ts = settings.timeSpeed;
        Debug.Log(ts);
        ts = Mathf.Sqrt(ts);
        Debug.Log(ts);
        ts -= 0.5f;
        Debug.Log(ts);
        timeSpeed.value = ts;
        Debug.Log(ts);

        ts = settings.eventSpeed;
        Debug.Log(ts);
        ts = Mathf.Sqrt(ts);
        Debug.Log(ts);
        ts -= 0.5f;
        Debug.Log(ts);
        eventSpeed.value = ts;
        Debug.Log(ts);

        ts = settings.themeVolume;
        Debug.Log(ts);
        volume.value= ts;

        
        if (TutorialManager.Instance == null || TutorialManager.Instance.deactivateTuto)
        {
            deactivateTUto.SetIsOnWithoutNotify(true);
        }
        if (TutorialManager.Instance != null && TutorialManager.Instance.deactivateTutorialScroll)
        {
            deactivateTutorialScroll.SetIsOnWithoutNotify(true);
        }
    }

    public void SetSettings()
    {
        float ts = timeSpeed.value;
        ts += 0.5f;
        ts = Mathf.Pow(ts, 2);
        settings.timeSpeed = ts;
        ts = eventSpeed.value;
        ts += 0.5f;
        ts = Mathf.Pow(ts, 2);
        settings.eventSpeed = ts;

        ts = volume.value;
        settings.themeVolume = ts;
        AudioManager.Instance.volume = ts;

        if (resetTuto.isOn)
        {
            TutorialManager.Instance?.ResetTuto();
        }
        if (deactivateTUto.isOn)
        {
            TutorialManager.Instance?.DeactivateTuto();
        }
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance?.DeactivateTutorialScroll(deactivateTutorialScroll.isOn);
        }
    }
}