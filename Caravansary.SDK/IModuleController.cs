using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

public interface IModuleController
{
    string[] CoreModulesKeys { get; }

    void StartCoreModule(string name);

    void StopCoreModule(string name);

    [Obsolete]
    void SendMessage(string ModuleName, string message);

    void SendEvent(string source, string eventName, string data);

    bool SaveModuleInformation(string ModuleName, string saveFileName, object objectToSave);

    T LoadModuleInformation<T>(string ModuleName, string saveFileName);

    void SubscribeToEvent(string eventName, Action<object> action);

    void SubscribeToEvent(string ModuleName, string eventName, Action<object> action);

    void UnsubscribeToEvent(string ModuleName, string eventName, Action<object> action);

    void OnEventTriggered(string ModuleName, string eventName, object data);

    #region events

    public void HookWindowSwitchEvent(Action<WindowSwitchedArgs> method);

    public void UnHookWindowSwitchEvent(Action<WindowSwitchedArgs> method);

    public void HookKeyboardPressedEvent(Action<KeyPressedArgs> method);

    public void UnHookKeyboardPressedEvent(Action<KeyPressedArgs> method);

    public void HookKeyboardReleaseEvent(Action<KeyReleasedArgs> method);

    public void UnHookKeyboardReleasedEvent(Action<KeyReleasedArgs> method);

    #endregion events
}