namespace MyBase.Observer
{
    internal class EventManager
    {
        private static Observable _instance;

        private static Observable instance
        {
            get
            {
                if (_instance == null) _instance = new Observable();
                return _instance;
            }
        }

        public static void Notify(string type, object data = null, object source = null)
        {
            instance.Notify(type, source, data);
        }

        public static void Attach(IObserver observer, string mask = "0", string funcName = null)
        {
            instance.Attach(observer, mask, funcName);
        }

        public static void Detach(IObserver observer, string mask = "0")
        {
            instance.Dettach(observer, mask);
        }
    }
}
