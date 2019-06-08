using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class NodeEvent
{
    public enum TYPE {COMBAT };

    public TYPE type;
    public void Setup()
    {

    }
    public int ID;
   // public abstract void Perform();
}
