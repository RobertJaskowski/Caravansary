using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IModuleController
{
    string[] CoreModulesKeys { get; }

    void StartCoreModule(string name);
    void StopCoreModule(string name);

    void SendMessage(string ModuleName, string message);
}
