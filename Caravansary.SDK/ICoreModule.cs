using System;

public interface ICoreModule
{
    void Init(IModuleController host);
    void Start();

    void ReceiveMessage(string message);

    void Stop();

    void OnMinViewEntered();
    void OnFullViewEntered();

    ModulePosition GetModulePosition();

    

}
