using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class ToggleTipWindow : MonoBehaviour
{
    private static ToggleTipWindow instance;
    public static ToggleTipWindow Instance { get => instance; }

    public Text ToggleText;
    public bool trackMouse;

    public void Start()
    {
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        transform.position = Input.mousePosition;
        transform.SetAsLastSibling();
    }

    private void Update()
    {
        if (trackMouse)
        {
            transform.position = Input.mousePosition;
        }
    }
}