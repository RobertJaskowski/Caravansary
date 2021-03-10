using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caravansary.CoreModules.Roadmap
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class RoadmapItems : List<RoadmapItem>
    {

        public RoadmapItems()
        { }

        public RoadmapItems(List<RoadmapItem> roadmapItems)
        {
            Clear();
            foreach (var item in roadmapItems)
            {
                this.Add(item);
            }
        }
    }
}
