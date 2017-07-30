using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeMaster.Logic.Shared
{
    public class BreakSettings
    {
        int BreakSequenceId { get; set; }
        int BreakWalkTime { get; set; }
        int BreakIdleTime { get; set; }
        int BreakEnabled { get; set; }
    }
}
