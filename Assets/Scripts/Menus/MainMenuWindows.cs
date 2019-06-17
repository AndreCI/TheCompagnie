using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuWindows : MonoBehaviour
{
    public StarterDeckDisplay starterInfos;

    public void StartGame()
    {
        PlayerPrefs.SetInt("Starter",(int) starterInfos.currentClass);
        PlayerPrefs.SetInt("StarterDeck",(int) starterInfos.currentDeck);
        SceneManager.LoadScene(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void OpenFeedback()
    {
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSdT7eLixwCpVmH7yRivF5uhUOrrlG5AMcv9gIkEjU8gDCVWFg/viewform");

    }
}