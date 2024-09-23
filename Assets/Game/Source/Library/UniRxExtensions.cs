using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UniRx;

namespace WerewolfBearer {
    public static class UniRxExtensions {
        public static TaskAwaiter GetAwaiter(this AsyncReactiveCommand command)  {
            return AwaitForCommandToExecute(command).GetAwaiter();
        }

        public static IObservable<Unit> ObserveAll<T>(this ReactiveCollection<T> collection) {
            return Observable.Merge(
                collection.ObserveAdd().AsUnitObservable(),
                collection.ObserveMove().AsUnitObservable(),
                collection.ObserveRemove().AsUnitObservable(),
                collection.ObserveReplace().AsUnitObservable(),
                collection.ObserveReset()
            );
        }

        public static IObservable<Unit> ObserveAll<TKey, TValue>(this ReactiveDictionary<TKey, TValue> dictionary) {
            return Observable.Merge(
                dictionary.ObserveAdd().AsUnitObservable(),
                dictionary.ObserveRemove().AsUnitObservable(),
                dictionary.ObserveReplace().AsUnitObservable(),
                dictionary.ObserveReset()
            );
        }

        private static async Task AwaitForCommandToExecute(AsyncReactiveCommand command) {
            while (!command.CanExecute.Value) {
                await Task.Yield();
            }
        }
    }
}
