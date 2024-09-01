using UnityEngine;

namespace Sapiscow.Framework.Signal
{
    /// <summary>
    /// Basic subscription of signal.
    /// </summary>
    public class SignalSubscription<T> : ISubscription where T : ISignal
    {
        protected readonly object _owner;

        public System.Action<T> Listener { get; private set; }
        public bool IsUnsubscribed { get; private set; }

        public System.Type SignalType => typeof(T);
        public object Owner
        {
            get
            {
                if (_owner is MonoBehaviour monoBehaviour)
                {
                    try { return monoBehaviour.gameObject; }
                    catch (System.Exception) { return null; }
                }

                return _owner;
            }
        }

        public SignalSubscription(System.Action<T> listener)
        {
            _owner = listener.Target;
            Listener = listener;
        }

        public virtual void Invoke(ISignal signal)
        {
            if (Owner == null) Unsubscribe();
            else Listener?.Invoke((T)signal);
        }

        public void Unsubscribe()
        {
            IsUnsubscribed = true;
            SignalManager.Unsubscribe(this);
        }
    }
}