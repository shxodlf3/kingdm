using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;

namespace KingsDamageMeter
{
    public class SafeNotifiedCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Be nice - use BlockReentrancy like MSDN said
            using (BlockReentrancy())
            {
                NotifyCollectionChangedEventHandler eventHandler = CollectionChanged;
                if (eventHandler == null)
                    return;

                Delegate[] delegates = eventHandler.GetInvocationList();
                // Walk thru invocation list
                //NotifyCollectionChangedEventHandler handler = (NotifyCollectionChangedEventHandler)delegates[0];
                System.Diagnostics.Debug.WriteLine("Delegates: " + delegates.Length);
                foreach (NotifyCollectionChangedEventHandler handler in delegates)
                {
                    var dispatcherObject = handler.Target as DispatcherObject;
                    // If the subscriber is a DispatcherObject and different thread
                    if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                    {
                        // Invoke handler in the target dispatcher's thread
                        //dispatcherObject.Dispatcher.Invoke(DispatcherPriority.Render, handler, this, e);
                        System.Diagnostics.Debug.WriteLine("Need to be invoked in dispatcher: " + dispatcherObject);
                    }
                    else // Execute handler as is
                    {
                        handler(this, e);
                    }
                }
            }
        }
    }
}