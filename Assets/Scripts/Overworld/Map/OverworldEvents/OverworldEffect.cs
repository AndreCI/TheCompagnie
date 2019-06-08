using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public abstract class OverworldEffect
{
    public abstract void Setup(OverworldEvent parentEvent, int child);
    public abstract void Perform();
    
}
