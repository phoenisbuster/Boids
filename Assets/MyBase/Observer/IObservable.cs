namespace MyBase.Observer
{
    interface IObservable
    {
        void Attach(IObserver observer, string mask = "0", string funcName = null);

        void Dettach(IObserver observer, string mask = "0");

        void Notify(string type, object source, object data = null);
    }
}
