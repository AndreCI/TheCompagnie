using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class ControlOverworldEffect : OverworldEffect
{
   // public List<ControlOverworldEffect> controlChild;
    public List<LeafOverworldEffect> leafChild;

    public override void Perform()
    {
  //      if(controlChild.Count > 0 && leafChild.Count > 0) { return; }
   /*     if (controlChild.Count > 0)
        {
            foreach(ControlOverworldEffect e in controlChild)
            {
                e.Perform();
            }
        }else*/ if (leafChild.Count > 0)
        {
            foreach(LeafOverworldEffect e in leafChild)
            {
                e.Perform();
            }
        }
    }

    public override void Setup(OverworldEvent parentEvent, int child)
    {
        if (leafChild.Count > 0)
        {
            foreach(LeafOverworldEffect e in leafChild)
            {
                e.Setup(parentEvent, child);
            }
        }
    }
}