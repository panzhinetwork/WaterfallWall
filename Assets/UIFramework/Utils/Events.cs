// ========================================================================================
// A typesafe, lightweight event lib for Unity.
// ========================================================================================
// Based on https://github.com/yankooliveira/signals
//
// Usage:
//    1) Define your class, eg:
//          ScoreEvent : Event<int> {}
//    2) Add listeners on portions that should react, eg on Awake():
//          Events.Get<ScoreEvent>().AddListener(OnScore);
//    3) Dispatch, eg:
//          Events.Get<ScoreEvent>().Dispatch(userScore);
//    4) Don't forget to remove the listeners upon destruction! Eg on OnDestroy():
//          Events.Get<ScoreEvent>().RemoveListener(OnScore);
//    5) If you don't want to use global EventManager, you can have your very own EventHub
//       instance in your class
//
// Code Example:
//    public class GameOverEvent : BaseEvent {}
//    public class ScoreEvent : BaseEvent<string, int> { }
//
//    Events.Get<ScoreEvent>().AddListener(OnScore);
//    Events.Get<ScoreEvent>().Raise(playerName, playerScore);
//    Events.Get<ScoreEvent>().RemoveListener(OnScore);
// ========================================================================================


using System;
using System.Collections.Generic;

namespace UIFramework {

    /// <summary>
    /// 事件接口。
    /// </summary>
    public interface IEvent {
        string Name { get; }
    }

    /// <summary>
    /// 全局使用的所有事件的主API。
    /// </summary>
    public static class Events {

        private static readonly EventHub hub = new EventHub();

        /// <summary>
        /// 获取对应类型的事件。
        /// </summary>
        /// <returns>The get.</returns>
        /// <typeparam name="EType">The 1st type parameter.</typeparam>
        public static EType Get<EType>() where EType : IEvent, new() {
            return hub.Get<EType>();
        }

        public static void AddListenerWithName(string eventName, Action handler) {
            hub.AddListenerWithName(eventName, handler);
        }

        public static void RemoveListenerWithName(string eventName, Action handler) {
            hub.RemoveListenerWithName(eventName, handler);
        }
    }

    /// <summary>
    /// 保存事件的类。
    /// </summary>
    public class EventHub {

        private Dictionary<Type, IEvent> events = new Dictionary<Type, IEvent>();

        /// <summary>
        /// 获取对应类型的事件。
        /// </summary>
        /// <returns>The get.</returns>
        /// <typeparam name="EType">The 1st type parameter.</typeparam>
        public EType Get<EType>() where EType : IEvent, new() {
            Type eventType = typeof(EType);
            IEvent e;

            if (events.TryGetValue(eventType, out e)) {
                return (EType)e;
            }

            return (EType)Bind(eventType);
        }

        /// <summary>
        /// 手动提供唯一的事件名称，并且绑定 Listener。
        /// </summary>
        /// <param name="eventName">Unique event name.</param>
        /// <param name="handler">Handler.</param>
        public void AddListenerWithName(string eventName, Action handler) {
            IEvent e = GetEventByName(eventName);
            if (e != null && e is BaseEvent) {
                (e as BaseEvent).AddListener(handler);
            }
        }

        /// <summary>
        /// 手动提供唯一的事件名称，并且解绑 Listenr。
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="handler">Handler.</param>
        public void RemoveListenerWithName(string eventName, Action handler) {
            IEvent e = GetEventByName(eventName);
            if (e != null && e is BaseEvent) {
                (e as BaseEvent).RemoveListener(handler);
            }
        }

        private IEvent Bind(Type eventType) {
            IEvent e;
            if (events.TryGetValue(eventType, out e)) {
                UnityEngine.Debug.LogError(string.Format("Signal already registered for type {0}", eventType.ToString()));
                return e;
            }

            e = (IEvent)Activator.CreateInstance(eventType);
            events.Add(eventType, e);
            return e;
        }

        private IEvent GetEventByName(string eventName) {
            foreach(IEvent e in events.Values) {
                if(e.Name == eventName) {
                    return e;
                }
            }

            return null;
        }
    }


    /// <summary>
    /// 只提供名字功能的事件抽象类。
    /// </summary>
    public abstract class AbstractEvent : IEvent {

        protected string _name;

        /// <summary>
        /// 事件的唯一名称。
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get {
                if (string.IsNullOrEmpty(_name)) {
                    _name = this.GetType().ToString();
                }
                return _name;
            }
        }
    }

    /// <summary>
    /// 无参数的强类型事件基类。
    /// </summary>
    public abstract class BaseEvent : AbstractEvent {

        private Action _callback;

        /// <summary>
        /// 给事件添加 Listener。
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void AddListener(Action handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
                "不支持匿名 delegate 的绑定。因为后面无法移除这些 delegate。");
#endif
            _callback += handler;
        }

        /// <summary>
        /// 给事件移除 Listener.
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void RemoveListener(Action handler) {
#pragma warning disable RECS0020 // Delegate subtraction has unpredictable result
            _callback -= handler;
#pragma warning restore RECS0020 // Delegate subtraction has unpredictable result
        }

        /// <summary>
        /// 触发事件。
        /// </summary>
        public void Raise() {
            if (_callback != null) {
                _callback();
            }
        }
    }

    /// <summary>
    /// 一个参数的强类型事件基类。
    /// </summary>
    public abstract class BaseEvent<T> : AbstractEvent {

        private Action<T> _callback;

        /// <summary>
        /// 给事件添加 Listener。
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void AddListener(Action<T> handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
                "不支持匿名 delegate 的绑定。因为后面无法移除这些 delegate。");
#endif
            _callback += handler;
        }

        /// <summary>
        /// 给事件移除 Listener.
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void RemoveListener(Action<T> handler) {
#pragma warning disable RECS0020 // Delegate subtraction has unpredictable result
            _callback -= handler;
#pragma warning restore RECS0020 // Delegate subtraction has unpredictable result
        }

        /// <summary>
        /// 触发事件。
        /// </summary>
        public void Raise(T arg1) {
            if (_callback != null) {
                _callback(arg1);
            }
        }
    }

    /// <summary>
    /// 两个参数的强类型事件基类。
    /// </summary>
    public abstract class BaseEvent<T, U> : AbstractEvent {

        private Action<T, U> _callback;

        /// <summary>
        /// 给事件添加 Listener。
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void AddListener(Action<T, U> handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
                "不支持匿名 delegate 的绑定。因为后面无法移除这些 delegate。");
#endif
            _callback += handler;
        }

        /// <summary>
        /// 给事件移除 Listener.
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void RemoveListener(Action<T, U> handler) {
#pragma warning disable RECS0020 // Delegate subtraction has unpredictable result
            _callback -= handler;
#pragma warning restore RECS0020 // Delegate subtraction has unpredictable result
        }

        /// <summary>
        /// 触发事件。
        /// </summary>
        public void Raise(T arg1, U arg2) {
            if (_callback != null) {
                _callback(arg1, arg2);
            }
        }
    }

    /// <summary>
    /// 三个参数的强类型事件基类。
    /// </summary>
    public abstract class BaseEvent<T, U, V> : AbstractEvent {

        private Action<T, U, V> _callback;

        /// <summary>
        /// 给事件添加 Listener。
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void AddListener(Action<T, U, V> handler) {
#if UNITY_EDITOR
            UnityEngine.Debug.Assert(handler.Method.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), inherit: false).Length == 0,
                "不支持匿名 delegate 的绑定。因为后面无法移除这些 delegate。");
#endif
            _callback += handler;
        }

        /// <summary>
        /// 给事件移除 Listener.
        /// </summary>
        /// <param name="handler">Handler.</param>
        public void RemoveListener(Action<T, U, V> handler) {
#pragma warning disable RECS0020 // Delegate subtraction has unpredictable result
            _callback -= handler;
#pragma warning restore RECS0020 // Delegate subtraction has unpredictable result
        }

        /// <summary>
        /// 触发事件。
        /// </summary>
        public void Raise(T arg1, U arg2, V arg3) {
            if (_callback != null) {
                _callback(arg1, arg2, arg3);
            }
        }
    }
}