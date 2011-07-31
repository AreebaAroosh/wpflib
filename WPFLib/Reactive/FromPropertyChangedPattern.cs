using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using WPFLib.Reactive;

namespace System.Linq
{
    public static partial class Observable2
    {
        /// <summary>
        /// Creates an observable sequence of property changed notifications for the specified 
        /// <paramref name="property"/> on the specified <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TSource">Type of the object that defines the specified <paramref name="property"/>.</typeparam>
        /// <typeparam name="TValue">Type of the property's value.</typeparam>
        /// <param name="source">The object that defines the specified <paramref name="property"/>.</param>
        /// <param name="property">The property on the specified <paramref name="source"/> from which to generate property changed notifications.</param>
        /// <remarks>
        /// The following property changed notification patterns are supported: 
        /// <list type="bullet">
        /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
        /// <item>[Property]Changed event pattern.</item>
        /// <item>WPF dependency properties.</item>
        /// </list>
        /// </remarks>
        /// <returns>An observable sequence of property changed notifications.</returns>
        public static IObservable<TValue> FromPropertyChangedPattern<TSource, TValue>(
            TSource source,
            Expression<Func<TSource, TValue>> property, bool pumpInitial = false)
        {
            Contract.Requires(source != null);
            Contract.Requires(property != null);
            Contract.Ensures(Contract.Result<IObservable<TValue>>() != null);

            var compile = new Lazy<Func<TSource, TValue>>(
                property.Compile,
                isThreadSafe: false);		// Rx ensures that it's thread-safe

            var observable =
                from e in FromPropertyChangedPattern(source, property.GetPropertyInfo)
                let getter = compile.Value
                select getter(source);

            Contract.Assume(observable != null);

            if (pumpInitial)
            {
                return Observable.Return(compile.Value(source)).Merge(observable);
            }
            return observable;
        }

        /// <summary>
        /// Creates an observable sequence of property changed notifications for the specified <paramref name="property"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the property's value.</typeparam>
        /// <param name="property">The property from which to generate property changed notifications.</param>
        /// <remarks>
        /// The following property changed notification patterns are supported: 
        /// <list type="bullet">
        /// <item><see cref="INotifyPropertyChanged"/> implementations.</item>
        /// <item>[Property]Changed event pattern.</item>
        /// <item>WPF dependency properties.</item>
        /// </list>
        /// </remarks>
        /// <returns>An observable sequence of property changed notifications.</returns>
        public static IObservable<TValue> FromPropertyChangedPattern<TValue>(
            Expression<Func<TValue>> property)
        {
            Contract.Requires(property != null);
            Contract.Ensures(Contract.Result<IObservable<TValue>>() != null);

            var compile = new Lazy<Func<TValue>>(
                property.Compile,
                isThreadSafe: false);		// Rx ensures that it's thread-safe

            object source;
            PropertyInfo propertyInfo = property.GetPropertyInfo(out source);

            var observable =
                from e in FromPropertyChangedPattern(source, () => propertyInfo)
                let getter = compile.Value
                select getter();

            Contract.Assume(observable != null);

            return observable;
        }

        private static IObservable<IEvent<PropertyChangedEventArgs>> FromPropertyChangedPattern(
            object source,
            Func<PropertyInfo> getPropertyInfo)
        {
            Contract.Requires(source != null);
            Contract.Requires(getPropertyInfo != null);
            Contract.Ensures(Contract.Result<IObservable<IEvent<PropertyChangedEventArgs>>>() != null);

            /* TypeDescriptor supports INotifyPropertyChanged; however, it doesn't acknowledge that changed
             * events can also be raised for inherited properties, thus we should use INotifyPropertyChanged
             * directly when it's available.
             * 
             * Original discussion: 
             * http://social.msdn.microsoft.com/Forums/en/rx/thread/2fc8ab3c-28ed-45a9-a96f-59133a3d103c
             */
            var notifies = source as INotifyPropertyChanged;

            if (notifies != null)
            {
                var observable =
                    Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        eh => eh.Invoke,
                        eh => notifies.PropertyChanged += eh,
                        eh => notifies.PropertyChanged -= eh);

                Contract.Assume(observable != null);

                return observable;
            }
            else
            {
                PropertyInfo propertyInfo = getPropertyInfo();

                Contract.Assume(propertyInfo != null);

                string propertyName = propertyInfo.Name;

                var propertyDescriptor = ComponentReflection.GetProperty(source, propertyName, StringComparison.Ordinal);

                Contract.Assume(propertyDescriptor != null);

                return propertyDescriptor.PropertyChanged(source);
            }
        }
    }
}
