using System;
using System.Windows.Controls;

public interface ICoreModule
{

    string ModuleName
    {
        get;
    }
    string GetModuleName();
    void Init(IModuleController host);
    void Start();

    void ReceiveMessage(string message);

    void Stop();

    void OnInteractableEntered();
    void OnInteractableExited();

    void OnMinViewEntered();
    void OnFullViewEntered();

    ModulePosition GetModulePosition();

    UserControl GetModuleUserControlView();
    UserControl GetSettingsUserControlView();

}
