
namespace MyBase.Observer
{
    public class Observer : IObserver
    {
        protected void attach(string mask = "0", string funcName = null)
        {
            EventManager.Attach(this, mask, funcName);
        }

        protected void dettach(string mask = "0")
        {
            EventManager.Detach(this, mask);
        }

        protected void notify(string type, object data = null)
        {
            EventManager.Notify(type, data, this);
        }

        public virtual void OnEvent(string type, object source, object data = null)
        {

        }
    }
}
