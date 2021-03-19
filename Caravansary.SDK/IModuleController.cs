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

    bool SaveModuleInformation(string ModuleName, string saveFileName, object objectToSave);

    T LoadModuleInformation<T>(string ModuleName, string saveFileName);

    #region events
    public void HookWindowSwitchEvent(Action<WindowSwitchedArgs> method);

    public void UnHookWindowSwitchEvent(Action<WindowSwitchedArgs> method);

    public void HookKeyboardPressedEvent(Action<KeyPressedArgs> method);

    public void UnHookKeyboardPressedEvent(Action<KeyPressedArgs> method);

    public void HookKeyboardReleaseEvent(Action<KeyPressedArgs> method);

    public void UnHookKeyboardReleasedEvent(Action<KeyPressedArgs> method);

    #endregion
}
