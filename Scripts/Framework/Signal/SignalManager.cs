using Sapiscow.Framework.Singleton;
using System.Collections.Generic;

namespace Sapiscow.Framework.Signal
{
    public static class SignalManager
    {
        /// <summary>
        /// Publish a signal to be broadcasted.
        /// </summary>
        public static void Publish(ISignal signal)
            => SignalUpdater.Instance.Publish(signal);

        /// <summary>
        /// Subscribe for a signal to be listened.
        /// </summary>
        /// <param name="isOnce">One time or all time subscription</param>
        /// <returns></returns>
        public static ISubscription Subscribe<T>(System.Action<T> listener, bool isOnce = false) where T : ISignal
            => SignalUpdater.Instance.Subscribe(listener, isOnce);

        /// <summary>
        /// Unsubscribe from a listened signal.
        /// </summary>
        public static void Unsubscribe(ISubscription subscription)
            => SignalUpdater.Instance.Unsubscribe(subscription);

        internal class SignalUpdater : SingletonMono<SignalUpdater>
        {
            private readonly Dictionary<System.Type, LinkedList<ISubscription>> _signalDictionary = new();

            private readonly Queue<ISignal> _publishQueue = new();
            private readonly Queue<ISubscription> _subscribeQueue = new();
            private readonly Queue<LinkedListNode<ISubscription>> _unsubscribeQueue = new();

            private bool _isBusy;

            private void LateUpdate()
            {
                while (!_isBusy && _subscribeQueue.Count > 0)
                    SubscribeProcess(_subscribeQueue.Dequeue());

                while (!_isBusy && _unsubscribeQueue.Count > 0)
                    UnsubscribeProcess(_unsubscribeQueue.Dequeue());

                while (!_isBusy && _publishQueue.Count > 0)
                    PublishProcess(_publishQueue.Dequeue());
            }

            #region Publish
            public void Publish(ISignal signal)
            {
                if (_isBusy || _subscribeQueue.Count > 0 || _unsubscribeQueue.Count > 0)
                    _publishQueue.Enqueue(signal);
                else PublishProcess(signal);
            }

            private void PublishProcess(ISignal signal)
            {
                _isBusy = true;
                System.Type signalType = signal.GetType();

                foreach (var kvp in _signalDictionary)
                {
                    System.Type type = kvp.Key;
                    if (type.IsAssignableFrom(signalType))
                    {
                        var list = kvp.Value;
                        foreach (ISubscription sub in list)
                            if (!sub.IsUnsubscribed) sub.Invoke(signal);
                    }
                }

                _isBusy = false;
            }
            #endregion

            #region Subscribe
            public ISubscription Subscribe<T>(System.Action<T> listener, bool isOnce = false) where T : ISignal
            {
                ISubscription subscription = !isOnce ? new SignalSubscription<T>(listener) : new SignalOnceSubscription<T>(listener);

                if (_isBusy) _subscribeQueue.Enqueue(subscription);
                else SubscribeProcess(subscription);

                return subscription;
            }

            private void SubscribeProcess(ISubscription subscription)
            {
                _isBusy = true;
                System.Type signalType = subscription.SignalType;

                if (!_signalDictionary.TryGetValue(signalType, out var list))
                {
                    list = new();
                    _signalDictionary.Add(signalType, list);
                }

                list.AddLast(subscription);
                _isBusy = false;
            }
            #endregion

            #region Unsubscribe
            public void Unsubscribe(ISubscription subscription)
            {
                if (subscription == null) return;

                if (_signalDictionary.TryGetValue(subscription.SignalType, out var list))
                {
                    var toBeUnsubscribed = list.Find(subscription);
                    if (toBeUnsubscribed != null)
                    {
                        if (_isBusy) _unsubscribeQueue.Enqueue(toBeUnsubscribed);
                        else UnsubscribeProcess(toBeUnsubscribed);
                    }
                }
            }

            private void UnsubscribeProcess(LinkedListNode<ISubscription> subscription)
            {
                _isBusy = true;
                subscription?.List?.Remove(subscription);
                _isBusy = false;
            }
            #endregion
        }
    }
}