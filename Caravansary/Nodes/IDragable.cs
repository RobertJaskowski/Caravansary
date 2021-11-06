namespace Caravansary
{
    internal interface IDragable
    {
        string DataType { get; }

        void Remove(object i);
    }
}