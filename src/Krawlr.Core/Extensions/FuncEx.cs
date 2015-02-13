using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Krawlr.Core.Extensions
{
    public static class FuncEx
    {
        [DebuggerStepThrough]
        public static Func<TRes> Create<TRes>(Func<TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TRes> Create<TArg1, TRes>(Func<TArg1, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TRes> Create<TArg1, TArg2, TRes>(Func<TArg1, TArg2, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TRes> Create<TArg1, TArg2, TArg3, TRes>(Func<TArg1, TArg2, TArg3, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TArg4, TRes> Create<TArg1, TArg2, TArg3, TArg4, TRes>(Func<TArg1, TArg2, TArg3, TArg4, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TRes> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TRes>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TRes> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TRes>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TRes> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TRes>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TRes> f) { return f; }
        [DebuggerStepThrough]
        public static Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TRes> Create<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TRes>(Func<TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, TRes> f) { return f; }

        [DebuggerStepThrough]
        public static TRes Invoke<TRes>(Func<TRes> f) { return f(); }

        internal static bool dontMemoize = false;

        /// <summary>
        /// (IS) implements the memoization pattern: the given function is called once only, its result gets cached.
        /// </summary>
        /// <typeparam name="T">result type</typeparam>
        /// <param name="f">function to memoize</param>
        /// <returns>memoized result</returns>
        public static MemoizedFunc<T> Memoize<T>(this Func<T> f, bool threadSafe = false)
        {
            return new MemoizedFunc<T>(threadSafe, f);
        }
        /// <summary>
        /// (IS) implements the memoization pattern: the given function is called once only, its result gets cached.
        /// </summary>
        /// <typeparam name="TArg1">first argument type</typeparam>
        /// <typeparam name="TRes">result type</typeparam>
        /// <param name="f">function to memoize</param>
        /// <param name="threadSafe">true if it needs to be thread-safe (incurs performance penalty), false otherwise; default is false (not thread-safe)</param>
        /// <returns>memoized result</returns>
        public static MemoizedFunc<TArg1, TRes> Memoize<TArg1, TRes>(this Func<TArg1, TRes> f, bool threadSafe = false)
        {
            return new MemoizedFunc<TArg1, TRes>(threadSafe, f);
        }

        /// <summary>
        /// (IS) implements the memoization pattern: the given function is called once only, its result gets cached.
        /// </summary>
        /// <typeparam name="TArg1">first argument type</typeparam>
        /// <typeparam name="TArg2">2nd argument type</typeparam>
        /// <typeparam name="TRes">result type</typeparam>
        /// <param name="f">function to memoize</param>
        /// <param name="threadSafe">true if it needs to be thread-safe (incurs performance penalty), false otherwise; default is false (not thread-safe)</param>
        /// <returns>memoized result</returns>
        public static MemoizedFunc<TArg1, TArg2, TRes> Memoize<TArg1, TArg2, TRes>(this Func<TArg1, TArg2, TRes> f, bool threadSafe = false)
        {
            return new MemoizedFunc<TArg1, TArg2, TRes>(threadSafe, f);
        }
        public static MemoizedFunc<TArg1, TArg2, TArg3, TRes> Memoize<TArg1, TArg2, TArg3, TRes>(this Func<TArg1, TArg2, TArg3, TRes> f, bool threadSafe = false)
        {
            return new MemoizedFunc<TArg1, TArg2, TArg3, TRes>(threadSafe, f);
        }

        //public static Func<Void> Void(Action f)
        //{
        //    return f.AsFunc();
        //}

        /// <summary>
        /// (IS) creates async Func[T] factory
        /// </summary>
        /// <typeparam name="T">function return type</typeparam>
        /// <param name="f">function which gets executed asynchronously</param>
        /// <returns>async Func[T] factory</returns>
        public static Func<Func<T>> AsAsyncFactory<T>(this Func<T> f)
        {
            return FuncEx.Create(() =>
            {
#if !disable_async
                var task = System.Threading.Tasks.Task.Factory.StartNew(f);
                return FuncEx.Create(() => task.Result);
#else
				var res = f();
				return FuncEx.Create(() => res);
#endif
            });
        }
    }

    public class Void
    {
        private Void() { }
        public static Void Value = new Void();
        public static Tuple<Void> TupleValue = Tuple.Create(Value);
    }

    public abstract class MemoizedFunc
    {
        static readonly long MinTime = DateTime.MinValue.ToBinary();
        long _timeClearedOn = MinTime;
        public MemoizedFunc(bool threadSafe)
        {
            //this.EventsToClearOn = Enumerable.Empty<ApplicationEvent>();
            this.ThreadSafe = threadSafe;
        }
        protected void ClearCache()
        {
            this.TimeClearedOn = DateTime.UtcNow.ToBinary();
        }
        public bool ThreadSafe { get; private set; }
        //protected bool _AnyEvents;
        //IEnumerable<ApplicationEvent> _EventsToClearOn;
        //public IEnumerable<ApplicationEvent> EventsToClearOn
        //{
        //    get { return _EventsToClearOn; }
        //    internal set
        //    {
        //        _EventsToClearOn = value.EmptyIfNull();
        //        _AnyEvents = _EventsToClearOn.Any();
        //    }
        //}
        public long TimeClearedOn
        {
            get
            {
                return System.Threading.Interlocked.Read(ref _timeClearedOn);
            }
            set
            {
                System.Threading.Interlocked.Exchange(ref _timeClearedOn, value);
            }
        }
        public string Name { get; internal set; }
    }

    public class MemoizedTupleKeyFunc<TKey, TValue> : MemoizedFunc
    {
        Action _clearCache;
        Func<TKey, TValue> _valueGetter;
        protected MemoizedTupleKeyFunc(bool threadSafe, Func<TKey, TValue> valueFactory)
            : base(threadSafe)
        {
            if (FuncEx.dontMemoize)
            {
                _valueGetter = valueFactory;
                _clearCache = () => { };
            }
            else if (threadSafe)
            {
                var cache = new System.Collections.Concurrent.ConcurrentDictionary<TKey, TValue>();
                _valueGetter = key =>
                {
                    //Helpers.Helpers.Ignore(this.Name);
                    var res = cache.GetOrAdd(key, valueFactory);
                    return res;
                };
                _clearCache = () =>
                {
                    //Helpers.Helpers.Ignore(this.Name);
                    cache.Clear();
                };
            }
            else
            {
                var cache = new Dictionary<TKey, TValue>();
                _valueGetter = key => cache.GetValueOrCreate(key, () => valueFactory(key));
                _clearCache = () => cache.Clear();
            }
        }
        protected TValue Get(TKey key)
        {
            //if (_AnyEvents)
            //{
            //    var clearedOn = this.TimeClearedOn;
            //    if (this.EventsToClearOn.Where(ev => ev.IsFiredSince(clearedOn)).Any())
            //        this.ClearCache();
            //}

            return _valueGetter(key);
        }
        new public void ClearCache()
        {
            base.ClearCache();
            _clearCache();
        }
    }

    public class MemoizedFunc<TValue> : MemoizedTupleKeyFunc<Tuple<Void>, TValue>
    {
        public MemoizedFunc(bool threadSafe, Func<TValue> valueFactory) : base(threadSafe, _ => valueFactory()) { }
        public TValue Get()
        {
            return base.Get(Void.TupleValue);
        }
        public static implicit operator Func<TValue>(MemoizedFunc<TValue> mf)
        {
            return mf.Get;
        }
        public Func<TValue> ToFunc()
        {
            return this;
        }
        public Func<TValue> AsFunc()
        {
            return this;
        }
    }

    public class MemoizedFunc<TArg1, TValue> : MemoizedTupleKeyFunc<Tuple<TArg1>, TValue>
    {
        public MemoizedFunc(bool threadSafe, Func<TArg1, TValue> valueFactory) : base(threadSafe, t => valueFactory(t.Item1)) { }
        public TValue Get(TArg1 arg1)
        {
            return base.Get(Tuple.Create(arg1));
        }
        public static implicit operator Func<TArg1, TValue>(MemoizedFunc<TArg1, TValue> mf)
        {
            return mf.Get;
        }
        public Func<TArg1, TValue> AsFunc()
        {
            return this;
        }
        public Func<TArg1, TValue> ToFunc()
        {
            return this;
        }
    }

    public class MemoizedFunc<TArg1, TArg2, TValue> : MemoizedTupleKeyFunc<Tuple<TArg1, TArg2>, TValue>
    {
        public MemoizedFunc(bool threadSafe, Func<TArg1, TArg2, TValue> valueFactory) : base(threadSafe, t => valueFactory(t.Item1, t.Item2)) { }
        public TValue Get(TArg1 arg1, TArg2 arg2)
        {
            return base.Get(Tuple.Create(arg1, arg2));
        }
        public static implicit operator Func<TArg1, TArg2, TValue>(MemoizedFunc<TArg1, TArg2, TValue> mf)
        {
            return mf.Get;
        }
        public Func<TArg1, TArg2, TValue> AsFunc()
        {
            return this;
        }
        public Func<TArg1, TArg2, TValue> ToFunc()
        {
            return this;
        }
    }

    public class MemoizedFunc<TArg1, TArg2, TArg3, TValue> : MemoizedTupleKeyFunc<Tuple<TArg1, TArg2, TArg3>, TValue>
    {
        public MemoizedFunc(bool threadSafe, Func<TArg1, TArg2, TArg3, TValue> valueFactory) : base(threadSafe, t => valueFactory(t.Item1, t.Item2, t.Item3)) { }
        public TValue Get(TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            return base.Get(Tuple.Create(arg1, arg2, arg3));
        }
        public static implicit operator Func<TArg1, TArg2, TArg3, TValue>(MemoizedFunc<TArg1, TArg2, TArg3, TValue> mf)
        {
            return mf.Get;
        }
        public Func<TArg1, TArg2, TArg3, TValue> AsFunc()
        {
            return this;
        }
        public Func<TArg1, TArg2, TArg3, TValue> ToFunc()
        {
            return this;
        }
    }

    //public static class MemoizedFuncEx
    //{
    //    public static TMemoizedFunc ResetOnEvents<TMemoizedFunc>(this TMemoizedFunc mf, IEnumerable<ApplicationEvent> events) where TMemoizedFunc : MemoizedFunc
    //    {
    //        mf.EventsToClearOn = events.EmptyIfNull();
    //        return mf;
    //    }
    //    public static TMemoizedFunc Named<TMemoizedFunc>(this TMemoizedFunc mf, string name) where TMemoizedFunc : MemoizedFunc
    //    {
    //        mf.Name = name;
    //        return mf;
    //    }

    //}

    //public interface IEventTimeStorage
    //{
    //    int Priority { get; }
    //    long GetLastTimeTriggered(ApplicationEvent applicationEvent);
    //    void SetLastTimeTriggered(ApplicationEvent applicationEvent, long value);
    //}

    //public class NonDistributedApplicationEventTimeStorage : IEventTimeStorage
    //{
    //    public long GetLastTimeTriggered(ApplicationEvent applicationEvent)
    //    {
    //        return applicationEvent.LastTimeTriggered;
    //    }
    //    public void SetLastTimeTriggered(ApplicationEvent applicationEvent, long value)
    //    {
    //        applicationEvent.SetLastTimeTriggered(value);
    //    }
    //    public int Priority
    //    {
    //        get
    //        {
    //            //(IS) to make sure that this storage is used when no other available
    //            return 1000;
    //        }
    //    }
    //}

    //namespace Internal
    //{
    //    public abstract class EventTimeStorageStrategyBase : IEventTimeStorage
    //    {
    //        public virtual int Priority { get { return 0; } }
    //        public abstract long GetLastTimeTriggered(ApplicationEvent applicationEvent);
    //        public abstract void SetLastTimeTriggered(ApplicationEvent applicationEvent, long value);
    //    }

    //    public class FailoverEventTimeStorage : EventTimeStorageStrategyBase
    //    {

    //        public class EventTimeStorageStrategyNoCache : EventTimeStorageStrategyBase
    //        {
    //            public EventTimeStorageStrategyNoCache(Exception reason)
    //            {
    //                this.Reason = reason;
    //            }
    //            public Exception Reason { get; private set; }
    //            public override long GetLastTimeTriggered(ApplicationEvent applicationEvent)
    //            {
    //                return DateTime.UtcNow.ToBinary();
    //            }
    //            public override void SetLastTimeTriggered(ApplicationEvent applicationEvent, long value)
    //            {
    //            }
    //        }

    //        public FailoverEventTimeStorage(IEventTimeStorage inner)
    //        {
    //            this.Inner = inner;
    //            this.CurrentStorage = inner;
    //        }
    //        public IEventTimeStorage Inner { get; private set; }
    //        public IEventTimeStorage CurrentStorage { get; protected set; }

    //        private DateTime? LastFailedAt { get; set; }
    //        private const int TimeToNextAttemptMin = 5;

    //        public override int Priority
    //        {
    //            get
    //            {
    //                return this.Inner.Priority;
    //            }
    //        }
    //        public override long GetLastTimeTriggered(ApplicationEvent applicationEvent)
    //        {
    //            applicationEvent.CheckForcedRefresh();
    //            return this.CallStorage(storage => storage.GetLastTimeTriggered(applicationEvent));
    //        }
    //        public override void SetLastTimeTriggered(ApplicationEvent applicationEvent, long value)
    //        {
    //            this.CallStorage(storage =>
    //            {
    //                storage.SetLastTimeTriggered(applicationEvent, value);
    //                return true;
    //            });
    //        }
    //        T CallStorage<T>(Func<IEventTimeStorage, T> f)
    //        {
    //            try
    //            {
    //                if (LastFailedAt.HasValue && (DateTime.UtcNow - LastFailedAt.Value).TotalMinutes >= TimeToNextAttemptMin)
    //                {
    //                    this.LastFailedAt = null;
    //                    this.CurrentStorage = this.Inner;
    //                }
    //                return f(this.CurrentStorage);
    //            }
    //            catch (Exception e)
    //            {
    //                this.LastFailedAt = DateTime.UtcNow;
    //                this.CurrentStorage = new EventTimeStorageStrategyNoCache(e);
    //                //e.LogException();

    //                return f(this.CurrentStorage);
    //            }
    //        }
    //        public bool HasFailed
    //        {
    //            get
    //            {
    //                return this.LastFailedAt != null;
    //            }
    //        }
    //    }
    //}

    //public static class ApplicationEventEx
    //{
    //    public static bool CacheNotUsedDueToCacheError
    //    {
    //        get
    //        {
    //            var storage = (Internal.FailoverEventTimeStorage)EventTimeStorage();
    //            return storage.HasFailed;
    //        }
    //    }

    //    public static Func<IEventTimeStorage> EventTimeStorage =
    //        FuncEx.Create(() => new Internal.FailoverEventTimeStorage((from st in Janison.Utils.IoC.Helpers.GetServices<IEventTimeStorage>() orderby st.Priority select st).First()))
    //        .Memoize(threadSafe: true);

    //    public static TApplicationEvent TriggerOn<TApplicationEvent>(this TApplicationEvent applicationEvent, IEnumerable<ApplicationEvent> events) where TApplicationEvent : ApplicationEvent
    //    {
    //        events.EmptyIfNull().Iter(ev => ev.AddChildEvent(applicationEvent));
    //        return applicationEvent;
    //    }
    //}

    //public class ApplicationEventSubscription
    //{
    //    internal ApplicationEventSubscription(Action<ApplicationEvent, ApplicationEvent> onTrigger)
    //    {
    //        this.OnTriggerAction = onTrigger;
    //    }
    //    public Action<ApplicationEvent, ApplicationEvent> OnTriggerAction { get; private set; }
    //}

    //public class ApplicationEvent
    //{
    //    static readonly long MinTime = DateTime.MinValue.ToBinary();

    //    public class RequestEventTimeRecord
    //    {
    //        public ApplicationEvent Event { get; set; }
    //        public long LastTimeTriggered { get; set; }
    //    }

    //    static Func<ApplicationEvent, RequestEventTimeRecord> requestEventTimeRecord =
    //        FuncEx.Create((ApplicationEvent applicationEvent) => new RequestEventTimeRecord() { Event = applicationEvent, LastTimeTriggered = ApplicationEventEx.EventTimeStorage().GetLastTimeTriggered(applicationEvent) })
    //        .MemoizePerRequest();

    //    static long GetRequestLastTimeTriggered(ApplicationEvent applicationEvent)
    //    {
    //        return requestEventTimeRecord(applicationEvent).LastTimeTriggered;
    //    }
    //    static void SetRequestLastTimeTriggered(ApplicationEvent applicationEvent, long value)
    //    {
    //        if (!applicationEvent.SetLastTimeTriggered(value))
    //            return;

    //        var er = requestEventTimeRecord(applicationEvent);
    //        //(IS) update the local event time, so that this event can be seen locally right away
    //        er.LastTimeTriggered = value;
    //        //(IS) update the global event time
    //        ApplicationEventEx.EventTimeStorage().SetLastTimeTriggered(applicationEvent, value);
    //    }

    //    protected internal ApplicationEvent()
    //    {
    //        this.ChildEvents = new ConcurrentBag<ApplicationEvent>();
    //    }
    //    public IEnumerable<ApplicationEvent> ChildEvents { get; private set; }
    //    internal void AddChildEvent(ApplicationEvent ev)
    //    {
    //        ((ConcurrentBag<ApplicationEvent>)this.ChildEvents).Add(ev);
    //    }
    //    public string Name { get; internal set; }
    //    //(IS) this is to say that the event hasnt occured yet
    //    long _LastTimeTriggered = MinTime;
    //    /// <summary>
    //    /// (IS) time of the last event, in the global context
    //    /// </summary>
    //    public long LastTimeTriggered
    //    {
    //        get
    //        {
    //            return System.Threading.Interlocked.Read(ref _LastTimeTriggered);
    //        }
    //    }

    //    /// <summary>
    //    /// Trigger event if last forced refresh happened earlier 
    //    /// </summary>
    //    /// <returns></returns>
    //    internal bool CheckForcedRefresh()
    //    {
    //        if (!ForcedRefreshIntervalMin.HasValue)
    //            return false;

    //        var now = DateTime.UtcNow;

    //        if (DateTime.FromBinary(LastTimeTriggered) < now.AddMinutes(-ForcedRefreshIntervalMin.Value))
    //        {
    //            this.OnTriggeredRemotely();
    //            return true;
    //        }

    //        return false;
    //    }

    //    /// <summary>
    //    /// (AT) If specified then cache will be updated after certain time without external interventions
    //    /// </summary>
    //    public int? ForcedRefreshIntervalMin { get; internal set; }
    //    /// <summary>
    //    /// (IS) sets the time when the event was triggered last time, in global context
    //    /// </summary>
    //    /// <param name="value">time of the event</param>
    //    /// <returns>true if successful - when the given time is greater than one of the previous event</returns>
    //    public bool SetLastTimeTriggered(long value)
    //    {
    //        long current;
    //        do
    //        {
    //            current = this.LastTimeTriggered;
    //            //(IS) do not let the LastTimeTriggered going back in time, only forward
    //            if ((ulong)current > (ulong)value)
    //                return false;

    //        } while (current != System.Threading.Interlocked.CompareExchange(ref _LastTimeTriggered, value, current));

    //        return true;
    //    }
    //    /// <summary>
    //    /// (IS) similar to the Trigger(), except it is a response to an event which came from the remote app instance, therefore we should react on it but not send it to the other apps again
    //    /// </summary>
    //    internal void OnTriggeredRemotely()
    //    {
    //        this.SetLastTimeTriggered(DateTime.UtcNow.ToBinary());
    //        this.ChildEvents.Iter(ev => ev.OnTriggeredRemotely());
    //        this._Subscriptions.Iter(a => a.OnTriggerAction(this, this));
    //    }
    //    /// <summary>
    //    /// (IS) fires the event in the current request context; the event is also propagated to the global context and to all remote app instances
    //    /// </summary>
    //    public void Trigger()
    //    {
    //        SetRequestLastTimeTriggered(this, DateTime.UtcNow.ToBinary());
    //        this.ChildEvents.Iter(ev => ev.Trigger());
    //        this._Subscriptions.Iter(a => a.OnTriggerAction(this, this));
    //    }
    //    /// <summary>
    //    /// (IS) returns true if the event was fired since the given time, in current request context
    //    /// </summary>
    //    /// <param name="time">point of time</param>
    //    /// <returns>true if the event was fired since the given time, in current request context</returns>
    //    public bool IsFiredSince(long time)
    //    {
    //        var lastTimeTriggered = GetRequestLastTimeTriggered(this);
    //        return (ulong)lastTimeTriggered > (ulong)time;
    //    }
    //    ConcurrentBag<ApplicationEventSubscription> _Subscriptions = new ConcurrentBag<ApplicationEventSubscription>();
    //    public ApplicationEventSubscription Subscribe(Action<ApplicationEvent, ApplicationEvent> onTrigger)
    //    {
    //        var res = new ApplicationEventSubscription(onTrigger);
    //        _Subscriptions.Add(res);
    //        return res;
    //    }
    //}

    //public class ApplicationEventFactory
    //{
    //    public static ApplicationEventFactory Instance = new ApplicationEventFactory();
    //    ConcurrentBag<ApplicationEvent> Events = new ConcurrentBag<ApplicationEvent>();
    //    protected ApplicationEventFactory()
    //    {
    //    }
    //    public TEvent Create<TEvent>(string name, int? forcedRefreshIntervalMin = null) where TEvent : ApplicationEvent
    //    {
    //        var res = (TEvent)Activator.CreateInstance(typeof(TEvent), true);
    //        res.Name = name;
    //        res.ForcedRefreshIntervalMin = forcedRefreshIntervalMin;
    //        this.Events.Add(res);
    //        return res;
    //    }
    //    public void TriggerAllEvents()
    //    {
    //        this.Events.Iter(e => e.Trigger());
    //    }
    //}
}

