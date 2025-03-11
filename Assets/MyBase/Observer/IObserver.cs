namespace MyBase.Observer
{
    interface IObserver
    {
        void OnEvent(string type, object source, object data = null);
    }
}
