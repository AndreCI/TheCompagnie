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
        settings = GeneralUtils.Copy<PlayerSettings>(PlayerInfos.Instance.settings);
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
        PlayerInfos.Instance.settings = settings;
    }
}