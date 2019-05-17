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
        ts -= 0.5f;
        timeSpeed.value = ts;

        ts = settings.eventSpeed;
        ts = Mathf.Sqrt(ts);
        ts -= 0.5f;
        eventSpeed.value = ts;

        ts = settings.themeVolume;
        volume.value= ts;

        resetTuto.isOn = false;
        
        if (TutorialManager.Instance == null || PlayerSettings.Instance.disableTutorial)
        {
            deactivateTUto.isOn = true;
        }
        else
        {
            deactivateTUto.isOn = false;

        }
        if (TutorialManager.Instance != null && TutorialManager.Instance.deactivateTutorialScroll)
        {
            deactivateTutorialScroll.isOn = true;
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
        AudioManager.Instance.SetVolume(ts);

        if (deactivateTUto.isOn)
        {
            TutorialManager.Instance?.DeactivateTuto();
        }
        else
        {
            PlayerSettings.Instance.disableTutorial = false;
        }
        if (resetTuto.isOn)
        {
            TutorialManager.Instance?.ResetTuto();
        }
        if (TutorialManager.Instance != null)
        {
            TutorialManager.Instance?.DeactivateTutorialScroll(deactivateTutorialScroll.isOn);
        }
        settings.WriteToDisk();
    }
}