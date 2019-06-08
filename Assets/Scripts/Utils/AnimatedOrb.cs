using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedOrb : MonoBehaviour
{
    public GameObject holder;
    public Image glowingIndicator;
    public GameObject textHolder;

    private float currentTime = 0f;
    public float animationTime = 1.5f;
    private float minScale = 0.9f;
    public float maxScale = 1.2f;
    public float rotationSpeed = -1f;
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
            glowingIndicator.color = new Color(glowingIndicator.color.r, glowingIndicator.color.g, glowingIndicator.color.b, 1 - progression/5f);

        }
        holder.transform.Rotate(new Vector3(0, 0, rotationSpeed));
        
    }

    public void SetText(string text)
    {
        foreach(Text t in textHolder.GetComponentsInChildren<Text>())
        {
            readyToGoIndicator = true;
            t.text = text;
        }
    }
}