using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeMaster.Logic.Shared
{
    public class BreakSettings
    {
        public int BreakSequenceId { get; set; }
        public int BreakWalkTime { get; set; }
        public int BreakIdleTime { get; set; }
        public bool BreakEnabled { get; set; }
    }
}
