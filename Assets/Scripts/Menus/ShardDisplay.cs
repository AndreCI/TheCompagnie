using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class ShardDisplay : MonoBehaviour, ITooltipActivator
{
    private static ShardDisplay instance;
    public static ShardDisplay Instance { get => instance; }

    public Image glowingIndicator;
    public GameObject holder;
    public int amount = 0;
    private float currentTime = 0f;
    private float animationTime = 1.5f;
    public float minScale = 0.9f;
    public float maxScale = 1.2f;
    public float rotationSpeed = -1f;
    public float additionalRotation = 0f;
    public bool trigger = false;
    private List<Text> texts;

    private void Awake()
    {
        if(instance != null) { Destroy(gameObject); return; }
        instance = this;
        amount = 0;
        texts = new List<Text>(GetComponentsInChildren<Text>());
        foreach (Text t in texts)
            t.text = "0";
    }

    private void Update()
    {
        if (trigger)
        {
            float progression = 0f;
            currentTime += Time.deltaTime;
            if (currentTime > animationTime * 2 * PlayerInfos.Instance.settings.eventSpeed)
            {
                currentTime = 0f;
                trigger = false;
            }
            else if (currentTime > animationTime * PlayerInfos.Instance.settings.eventSpeed)
            {
                progression = 2 - (currentTime / (animationTime * PlayerInfos.Instance.settings.eventSpeed));
            }
            else
            {
                progression = currentTime / (animationTime * PlayerInfos.Instance.settings.eventSpeed);
            }
            glowingIndicator.transform.localScale = new Vector3((maxScale + (float)amount/100f) * progression + minScale * (1 - progression),
                (maxScale + (float)amount / 100f) * progression + minScale * (1 - progression),
                1f);
        }
        holder.transform.Rotate(new Vector3(0, 0, rotationSpeed - additionalRotation));
        if (additionalRotation > 0.1f)
        {
            additionalRotation -= additionalRotation / 8f * Time.deltaTime;
        }
    }

    public void AddShards(int x)
    {
        amount += x;
        foreach(Text t in texts)
            t.text = amount.ToString();
        additionalRotation += (float)x/100f;
        trigger = true;
    }

    public bool ToolTipShow { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void OnToolTip(bool show)
    {
        throw new NotImplementedException();
    }
}
