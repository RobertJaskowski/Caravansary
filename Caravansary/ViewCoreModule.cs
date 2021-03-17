using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

public class ViewCoreModule
{

    private ICoreModule coreModule;
    public ICoreModule CoreModule { get => coreModule; set => coreModule = value; }


    private UserControl view;
    public UserControl View { get => view; set => view = value; }
}
