public interface ICoreModule
{
    void Init(IModuleController host);
    void Start();
    void Stop();
}
