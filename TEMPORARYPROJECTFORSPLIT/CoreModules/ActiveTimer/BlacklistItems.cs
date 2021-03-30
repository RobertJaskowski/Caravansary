using System.Collections.Generic;
using System.Configuration;

namespace Caravansary
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class BlacklistItems : List<BlacklistItem>
    {

        public BlacklistItems()
        { }
    }



}
