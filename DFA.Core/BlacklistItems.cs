using System.Collections.Generic;
using System.Configuration;

namespace DFA
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class BlacklistItems : List<BlacklistItem>
    {

        public BlacklistItems()
        { }
    }



}
