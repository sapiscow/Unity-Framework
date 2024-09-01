namespace Sapiscow.Framework.Signal
{
    /// <summary>
    /// One time subscription for a signal.
    /// </summary>
    public class SignalOnceSubscription<T> : SignalSubscription<T> where T : ISignal
    {
        public SignalOnceSubscription(System.Action<T> listener) : base(listener) { }

        public override void Invoke(ISignal signal)
        {
            base.Invoke(signal);
            Unsubscribe();
        }
    }
}