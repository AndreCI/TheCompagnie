using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapNode : MonoBehaviour, IPointerDownHandler
{
    public List<MapNode> nextNodes;
    public CombatNodeEvent nodeEvent;
    public Image icon;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (nextNodes.Contains(PlayerInfos.Instance.currentPosition))
        {
            PlayerInfos.Instance.currentPosition = this;
            nodeEvent.Perform();
            icon.color = Color.red;
        }
    }
}