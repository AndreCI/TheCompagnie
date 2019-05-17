using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ITooltipActivator
{
    void OnToolTip(bool show);
    bool ToolTipShow { get; set; }
}