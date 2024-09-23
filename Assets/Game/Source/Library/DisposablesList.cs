using System;
using System.Collections.Generic;

namespace WerewolfBearer {
    public class DisposablesList : IDisposable {
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        public void Dispose() {
            foreach (var subscription in _subscriptions) {
                subscription.Dispose();
            }

            _subscriptions.Clear();
        }

        public void Add(IDisposable disposable) {
            if (!_subscriptions.Contains(disposable)) {
                _subscriptions.Add(disposable);
            }
        }

        public static DisposablesList operator +(DisposablesList list, IDisposable disposable) {
            list.Add(disposable);
            return list;
        }
    }
}
