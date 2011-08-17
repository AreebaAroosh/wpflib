using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Linq;
using System.Windows.Threading;
using System.Concurrency;
using WPFLib.Misc;
using System.Disposables;
using System.Reflection;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using WPFLib.Reactive;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Specialized;

namespace System
{
    public static class ReactiveExtensions
    {
        public static IObservable<T> MergeLive<T>(this IEnumerable<IObservable<T>> source)
        {
            var collChanged = source as INotifyCollectionChanged;
            if (collChanged == null)
            {
                throw new Exception("source must be INotifyCollectionChanged");
            }
            return Observable.Create<T>((observer) =>
            {
                var subscriptions = new Dictionary<IObservable<T>, IDisposable>();
                foreach (var el in source)
                    subscriptions[el] = el.Subscribe(observer.OnNext);
                collChanged.CollectionChanged += (sender, args) =>
                {
                    switch (args.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            foreach (IObservable<T> item in args.NewItems)
                            {
                                if (!subscriptions.ContainsKey(item))
                                {
                                    subscriptions[item] = item.Subscribe(observer.OnNext);
                                }
                            }
                            break;
                        case NotifyCollectionChangedAction.Move:
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            foreach (IObservable<T> item in args.OldItems)
                            {
                                IDisposable subs;
                                if (subscriptions.TryGetValue(item, out subs))
                                {
                                    subs.Dispose();
                                    subscriptions.Remove(item);
                                }
                            }
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            foreach (var sb in subscriptions.Values)
                                sb.Dispose();
                            subscriptions.Clear();
                            break;
                        default:
                            break;
                    }
                };
                return () =>
                {
                    foreach (var sb in subscriptions.Values)
                        sb.Dispose();
                    subscriptions.Clear();
                };
            }
            );
        }


        public static IObservable<Unit> ToUnit<T>(this IObservable<T> source)
        {
            return source.Select(v => new Unit());
        }

        public static IObservable<IEvent<RoutedEventArgs>> ToObservableEvent(this RoutedEvent e, UIElement target)
        {
            return Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
                (h) => new RoutedEventHandler(h),
                (h) => target.AddHandler(e, h),
                (h) => target.RemoveHandler(e, h));
        }

        public static Task<TResult> ToTaskFirst<TResult>(this IObservable<TResult> observable, CancellationToken cancellationToken, object state)
        {
            if (observable == null)
            {
                throw new ArgumentNullException("observable");
            }
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>(state);
            MutableDisposable disposable = new MutableDisposable();
            cancellationToken.Register(delegate
            {
                disposable.Dispose();
                tcs.TrySetCanceled();
            });
            disposable.Disposable = observable.Subscribe<TResult>(delegate(TResult value)
            {
                tcs.TrySetResult(value);
                disposable.Dispose();
            }, delegate(Exception ex)
            {
                tcs.TrySetException(ex);
                disposable.Dispose();
            }, delegate
            {
                tcs.TrySetResult(default(TResult));
                disposable.Dispose();
            });
            return tcs.Task;
        }

        public static Task<TResult> ToTaskFirst<TResult>(this IObservable<TResult> observable, CancellationToken cancellationToken)
        {
            return observable.ToTaskFirst<TResult>(cancellationToken, null);
        }

        public static Task<TResult> ToTaskFirst<TResult>(this IObservable<TResult> observable)
        {
            return observable.ToTaskFirst<TResult>(new CancellationToken(), null);
        }

        /// <summary>
        /// Заменяет конструкцию Select<TSource,IObservable<TValue>>().Switch()
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IObservable<TValue> Switch<TValue, TSource>(this IObservable<TSource> source, Func<TSource, IObservable<TValue>> selector)
        {
            return source.Select(selector).Switch();
        }

        public static IObservable<TValue> ToObservableStream<T, TValue>(this T source, Expression<Func<T, TValue>> property)
        {
            return Observable2.FromPropertyChangedPattern(source, property, true);
        }

        public static IObservable<TValue> ToObservable<T, TValue>(this T source, Expression<Func<T, TValue>> property)
        {
            return Observable2.FromPropertyChangedPattern(source, property);
        }

        /// <summary>
        /// Аналогичен BufferWithTime, но начинает буферизацию только когда появляется значение - т.е. никогда не дает пустого буфера
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static IObservable<IList<TSource>> BufferWithTimeAfterValue<TSource>(this IObservable<TSource> source, TimeSpan timeSpan)
        {
            return source.BufferWithTimeAfterValue(timeSpan, Scheduler.TaskPool);
        }

        /// <summary>
        /// Аналогичен BufferWithTime, но начинает буферизацию только когда появляется значение - т.е. никогда не дает пустого буфера
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="timeSpan"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static IObservable<IList<TSource>> BufferWithTimeAfterValue<TSource>(this IObservable<TSource> source, TimeSpan timeSpan, IScheduler scheduler)
        {
            var closing = source.Delay(timeSpan);
            return source.Window(() => closing)
                .SelectMany<IObservable<TSource>, IList<TSource>>(new Func<IObservable<TSource>, IObservable<IList<TSource>>>(Observable.ToList<TSource>));
        }

        /// <summary>
        /// То же что и TimeOut, но только возвращает значение после которого в указанный промежуток небыло другого значения
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static IObservable<TSource> TimeOutAfterValue<TSource>(this IObservable<TSource> source, TimeSpan timeOut)
        {
            return source.TimeOutAfterValue(timeOut, Scheduler.TaskPool);
        }
        /// <summary>
        /// То же что и TimeOut, но только возвращает значение после которого в указанный промежуток небыло другого значения
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="timeOut"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static IObservable<TSource> TimeOutAfterValue<TSource>(this IObservable<TSource> source, TimeSpan timeOut, IScheduler scheduler)
        {
            return Observable.CreateWithDisposable<TSource>((observer) =>
            {
                CompositeDisposable compositeDisposable = new CompositeDisposable();
                MutableDisposable timerDisposable = new MutableDisposable();
                compositeDisposable.Add(timerDisposable);
                Action<TSource> timer = null;

                timer = (v) =>
                {
                    observer.OnNext(v);
                };

                IDisposable prev = null;
                var subscription = source.Subscribe((v) =>
                {
                    if (prev != null)
                        prev.Dispose();
                    prev =
                        scheduler.Schedule(() => { timer(v); }, timeOut);
                    timerDisposable.Disposable = prev;
                });
                compositeDisposable.Add(subscription);
                return compositeDisposable;
            });
        }


        public static IObservable<T> Pump<T>(this T val)
        {
            return Observable.Return<T>(val);
        }

        public static IDisposable Subscribe<TSource>(this IObservable<TSource> source, Action onNext)
        {
            return source.Subscribe((e) => { onNext(); });
        }

        public static IObservable<object> DataContextObservable(this FrameworkElement element)
        {
            var initialContext = Observable.Create<object>((observer) => 
            {
                if (element.DataContext != null)
                {
                    observer.OnNext(element.DataContext);
                }
                observer.OnCompleted();
                return () => { };
            });
            return initialContext.Concat(element.DataContextChangedObservable().Select(e => element.DataContext));
        }
    }
}
