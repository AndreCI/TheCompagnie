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
    public enum NODETYPE { UNKNOWN, EVENT, COMBAT};
    public List<MapNode> nextNodes;
    public NODETYPE type;
    public Image icon;
    public bool visited;
    public Button button;
    public bool discovered = false;

    public void Setup(List<MapNode> nodes)
    {
        nextNodes = new List<MapNode>();
        List<float> distances = nodes.Select(x => Vector3.Distance(transform.position, x.transform.position)).ToList();
        distances.Remove(distances.Min());
        float minDist = distances.Min();
        distances.Remove(distances.Min());
        float sndMinDist = distances.Min();
        distances.Remove(distances.Min());
        float trdDist = distances.Min();

        nextNodes.Add(nodes.Find(x => Vector3.Distance(transform.position, x.transform.position) == minDist));
        if(sndMinDist > minDist * 1.2) { return; }
        nextNodes.Add(nodes.Find(x => Vector3.Distance(transform.position, x.transform.position) == sndMinDist));

        if (trdDist > minDist * 1.5) { return; }
        nextNodes.Add(nodes.Find(x => Vector3.Distance(transform.position, x.transform.position) == trdDist));

        nextNodes = Shuffle(nextNodes);
        
        if(type != NODETYPE.EVENT && !visited && !discovered)
        {
            if (new System.Random().Next(100) > 66) 
            {
                type = NODETYPE.UNKNOWN;
            }
            else
            {
                type = NODETYPE.COMBAT;
            }
        }
        

        

    }


    public static List<MapNode> Shuffle(List<MapNode> iterable)
    {
        System.Random rng = new System.Random();
        int n = iterable.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            MapNode value = iterable[k];
            iterable[k] = iterable[n];
            iterable[n] = value;
        }
        return iterable;
    }

    public void UpdateNode()
    {
        if (nextNodes.Contains(PlayerInfos.Instance.currentPosition))
        {
            if(type == NODETYPE.COMBAT)
            {
                icon.sprite = OverworldMap.Instance.combat;
            }else if(type == NODETYPE.EVENT)
            {
                icon.sprite = OverworldMap.Instance.eventIcon;
            }else if(type == NODETYPE.UNKNOWN)
            {
                icon.sprite = OverworldMap.Instance.unknown;
            }
            
            if (visited)
            {
            icon.sprite = OverworldMap.Instance.visited;
            }
            button.interactable = true;
            discovered = true;
        }
        else
        {
            if (visited)
            {
                icon.sprite = OverworldMap.Instance.visited;
            }
            else if(!discovered)
            {
                icon.sprite = OverworldMap.Instance.locked;
            }
            button.interactable = false;
        }
        if(PlayerInfos.Instance.currentPosition == this)
        {
            icon.sprite = OverworldMap.Instance.current;
        }
    }

    public void OnClick()
    {
        if (button.interactable)
        {
            PlayerInfos.Instance.currentPosition = this;
            OverworldMap.Instance.UpdateNodes();
            if (visited)
            {
                return;
            }
            visited = true;
            if(type == NODETYPE.COMBAT)
            {
                OverworldMap.Instance.StartCombat();
                OverworldMap.Instance.noTavern = false;
            }else if(type == NODETYPE.EVENT)
            {
                OverworldMap.Instance.StartEvent();
                OverworldMap.Instance.noTavern = true;
            }else if(type == NODETYPE.UNKNOWN)
            {
                if(new System.Random().Next(100) > 50 || OverworldMap.Instance.noTavern)
                {
                    OverworldMap.Instance.StartCombat();
                    OverworldMap.Instance.noTavern = false;
                }
                else
                {
                    OverworldMap.Instance.StartTown();
                    OverworldMap.Instance.noTavern = true;
                }
            }
        }
    }
}