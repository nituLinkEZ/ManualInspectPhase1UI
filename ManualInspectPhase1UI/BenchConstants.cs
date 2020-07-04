using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManualInspectPhase1UI
{
    class BenchConstants
    {
        public List<string> CHANNEL_LIST = new List<string>();

        public BenchConstants()
        {
            UpdateDefaultChannelNames();
        }


        private void UpdateDefaultChannelNames()
        {
            CHANNEL_LIST.Add("VERNIER");
            CHANNEL_LIST.Add("MICROMETER");
            CHANNEL_LIST.Add("Ver_300mm");
        }
    }
}
