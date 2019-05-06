using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapNode : MonoBehaviour
{
    public enum NODETYPE { COMBAT, EVENT};
    public List<MapNode> nextNodes;
    public NODETYPE type;
    public Image icon;
    public bool visited;
    public Button button;

    public void Setup()
    {
        if (nextNodes.Contains(PlayerInfos.Instance.currentPosition) || visited)
        {
            if(type == NODETYPE.COMBAT)
            {
                icon.sprite = OverworldMap.Instance.combat;
            }else if(type == NODETYPE.EVENT)
            {
                icon.sprite = OverworldMap.Instance.unknown;
            }
            button.interactable = true;
        }
        else
        {
            icon.sprite = OverworldMap.Instance.locked;
            button.interactable = false;
        }
        if (visited)
        {
            button.interactable = false;
            icon.sprite = OverworldMap.Instance.visited;
        }
    }

    public void OnClick()
    {
        if (button.interactable)
        {
            visited = true;
            PlayerInfos.Instance.currentPosition = this;
            if(type == NODETYPE.COMBAT)
            {
                OverworldMap.Instance.StartCombat();
            }else if(type == NODETYPE.EVENT)
            {
                OverworldMap.Instance.StartEvent();
            }
        }
    }
}