using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides extension methods for <see cref="PropertyDescriptor"/>.
    /// </summary>
    public static class PropertyDescriptorExtensions
    {
        /// <summary>
        /// Returns an observable sequence of property changed notifications from the 
        /// specified <paramref name="property"/> descriptor.
        /// </summary>
        /// <param name="property">The descriptor from which to create an observable sequence of changed notifications.</param>
        /// <param name="source">The object to which the <paramref name="property"/> belongs.</param>
        /// <returns>An observable sequence of property changed notifications.</returns>
        /// <exception cref="ArgumentException">The specified property does not support change events.</exception>
        public static IObservable<IEvent<PropertyChangedEventArgs>> PropertyChanged(
            this PropertyDescriptor property,
            object source)
        {
            Contract.Requires(property != null);
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<IObservable<IEvent<PropertyChangedEventArgs>>>() != null);

            if (!property.SupportsChangeEvents)
                throw new ArgumentException("PropertyDoesNotSupportChangeEvents", "property");

            var observable =
                from e in Observable.FromEvent<EventHandler, EventArgs>(
                    handler => handler.Invoke,
                    handler => property.AddValueChanged(source, handler),
                    handler => property.RemoveValueChanged(source, handler))
                select Event.Create(
                    e.Sender,
                    e.EventArgs as PropertyChangedEventArgs ?? new PropertyChangedEventArgs(property.Name));

            Contract.Assume(observable != null);

            return observable;
        }
    }
}
