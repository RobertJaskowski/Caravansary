using System.Collections.Generic;
using System.Configuration;


[SettingsSerializeAs(SettingsSerializeAs.Xml)]
public class BlacklistItems : List<BlacklistItem>
{

    public BlacklistItems()
    { }
}



