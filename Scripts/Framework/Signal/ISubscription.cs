namespace Sapiscow.Framework.Signal
{
    /// <summary>
    /// Subscription of any signal.
    /// </summary>
    public interface ISubscription
    {
        System.Type SignalType { get; }
        bool IsUnsubscribed { get; }

        void Invoke(ISignal signal);
        void Unsubscribe();
    }
}