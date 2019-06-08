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
    public enum NODETYPE { RANDOM_EVENT, FIXED_EVENT, COMBAT};
    public string fixedEventName;
    public List<MapNode> nextNodes;
    public NODETYPE type;
    public Image icon;
    public bool visited;
    public Button button;
    public Image glowingIndicator;
    public Color currentPositionColor;
    public Color nextPositionColor;
    public Color nextPositionVisitedColor;
    public bool discovered = false;

    [HideInInspector]
    public OverworldEvent fixedEvent;
    private float currentTime = 0f;
    private float animationTime = 1f;
    private float minScale = 1f;
    private float maxScale = 1.3f;
    private bool isCurrentPositionOrNext = false;
    public bool IsCurrentPositionOrNext { get => isCurrentPositionOrNext; set {
            glowingIndicator.gameObject.SetActive(value);
            isCurrentPositionOrNext = value;
        } }

    private void Update()
    {
        if(IsCurrentPositionOrNext)
        {
            float progression = 0f;
            currentTime += Time.deltaTime;
            if(currentTime> animationTime * 2)
            {
                currentTime = 0f;
            }else if(currentTime > animationTime)
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
        }
    }

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

        //Variable nodes must be RANDOM_EVENT. Combat nodes will still remains in combat type (tutorial & start of game)
        if(type == NODETYPE.FIXED_EVENT)
        {
            if (fixedEvent == null || fixedEvent.eventName == "") { fixedEvent = OverworldEventManager.Instance.GetFixedEvent(fixedEventName); }
        }else if (type != NODETYPE.FIXED_EVENT && type != NODETYPE.COMBAT && !visited && !discovered )//&& type != NODETYPE.BOSS) 
        {
            if (Utils.rdx.Next(100) > 80) 
            {
                type = NODETYPE.RANDOM_EVENT;
            }
            else
            {
                type = NODETYPE.COMBAT;
            }
        }
    }


    public static List<MapNode> Shuffle(List<MapNode> iterable)
    {
        int n = iterable.Count;
        while (n > 1)
        {
            n--;
            int k = Utils.rdx.Next(n + 1);
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
            }else if(type == NODETYPE.FIXED_EVENT)
            {
                icon.sprite = OverworldMap.Instance.unknown;
                if(fixedEvent == null || fixedEvent.eventName == "") { fixedEvent = OverworldEventManager.Instance.GetFixedEvent(fixedEventName); }
            }else if(type == NODETYPE.RANDOM_EVENT)
            {
                icon.sprite = OverworldMap.Instance.unknown;
            }

            glowingIndicator.CrossFadeColor(nextPositionColor, 1f, true, true);
            if (visited)
            {
                icon.sprite = OverworldMap.Instance.visited;
                glowingIndicator.CrossFadeColor(nextPositionVisitedColor, 1f, true, true);
            }
            button.interactable = true;
            discovered = true;
            IsCurrentPositionOrNext = true;
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
            IsCurrentPositionOrNext = false;
        }
        if(PlayerInfos.Instance.currentPosition == this)
        {
            icon.sprite = OverworldMap.Instance.current;
            IsCurrentPositionOrNext = true;
            glowingIndicator.CrossFadeColor(currentPositionColor, 1f, true, true);
        }
        currentTime = 0f;
    }

    public void OnClick()
    {
        if (button.interactable)
        {
            if (visited)
            {
                OverworldMap.Instance.UpdateNodes();
                PlayerInfos.Instance.currentPosition = this;
                return;
            }
            visited = true;
            if (type == NODETYPE.COMBAT)
            {
                OverworldMap.Instance.StartCombat();
            } else if(type == NODETYPE.FIXED_EVENT)
            {
                OverworldMap.Instance.StartEvent(fixedEvent);
            }else if(type == NODETYPE.RANDOM_EVENT)
            {
                OverworldMap.Instance.StartEvent();
            }

            PlayerInfos.Instance.currentPosition = this;
            OverworldMap.Instance.UpdateNodes();

        }
    }
}