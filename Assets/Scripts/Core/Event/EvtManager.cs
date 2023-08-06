using System;
using System.Collections.Generic;
using LFrame.Core.Tools;

namespace LFrame.Core.Event
{
    /// <summary>
    /// 事件实体
    /// </summary>
    public class Evt
    {
        public int ID;

        public object Param;

        public static Evt mEvt = new Evt();


        /// <summary>
        /// 共享对象，无GC
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static Evt Shared(int id, params object[] param)
        {
            mEvt.ID = id;
            if (param != null)
            {
                if (param.Length == 1)
                {
                    mEvt.Param = param[0];
                }
                else
                {
                    mEvt.Param = param;
                }
            }

            return mEvt;
        }

        /// <summary>
        /// 使用后清除引用
        /// </summary>
        public static void Reset()
        {
            mEvt.ID = 0;
            mEvt.Param = null;
        }
    }

    public class EvtManager
    {
        private static EvtManager _Instance;

        private static EvtManager Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new EvtManager();
                }

                return _Instance;
            }
        }

        EvtManager()
        {
            _Handlers = new Dictionary<int, List<EventHandlerDelegate>>();
            _Onces = new Dictionary<EventHandlerDelegate, bool>();
        }

        /// <summary>
        /// 委托事件
        /// </summary>
        public delegate void EventHandlerDelegate(Evt evt);

        /// <summary>
        /// 委托字典
        /// </summary>
        private Dictionary<int, List<EventHandlerDelegate>> _Handlers;

        /// <summary>
        /// 执行一次类型委托字典
        /// </summary>
        private Dictionary<EventHandlerDelegate, bool> _Onces;


        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        /// <param name="once"></param>
        public void Register(int id, EventHandlerDelegate handler, bool once = false)
        {
            if (handler != null)
            {
                if (_Handlers.ContainsKey(id) == false)
                {
                    _Handlers.Add(id, new List<EventHandlerDelegate>());
                }

                AddHandler(id, handler, once);
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        public void UnRegister(int id, EventHandlerDelegate handler)
        {
            if (handler != null && _Handlers.ContainsKey(id))
            {
                RemoveHandler(id, handler);
            }
        }

        /// <summary>
        /// 清除所有事件
        /// </summary>
        public void UnRegisterAll()
        {
            if (!(_Onces is null))
                _Onces.Clear();
            if (!(_Handlers is null))
                _Handlers.Clear();
        }

        public void AddHandler(int id, EventHandlerDelegate handler, bool once)
        {
            if (_Handlers.TryGetValue(id, out List<EventHandlerDelegate> handlers))
            {
                handlers.Add(handler);
                if (once)
                {
                    if (_Onces.TryGetValue(handler, out _) == false)
                    {
                        _Onces.Add(handler, true);
                    }
                }
            }
        }

        public void RemoveHandler(int id, EventHandlerDelegate handler)
        {
            if (_Handlers.TryGetValue(id, out List<EventHandlerDelegate> handlers))
            {
                handlers.Remove(handler);
                if (_Onces.TryGetValue(handler, out _))
                {
                    _Onces.Remove(handler);
                }
            }
        }

        /// <summary>
        /// 发布通知
        /// </summary>
        /// <param name="id"></param>
        /// <param name="evt"></param>
        public void Dispatch(int id, Evt evt)
        {
            if (evt == null) return;
            if (_Handlers.TryGetValue(id, out List<EventHandlerDelegate> handlers))
            {
                for (int i = 0; i < handlers.Count; i++)
                {
                    handlers[i](evt);
                    if (_Onces.TryGetValue(handlers[i], out _))
                    {
                        _Onces.Remove(handlers[i]);
                        handlers.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        public static void Notify(int id, params object[] param)
        {
            Instance.Dispatch(id, Evt.Shared(id, param));
            Evt.Reset();
        }

        public static void RegEvt(int id, EventHandlerDelegate handler, bool once = false)
        {
            Instance.Register(id, handler, once);
        }

        public static void UnRegEvt(int id, EventHandlerDelegate handler) { Instance.UnRegister(id, handler); }

        public static void Notify(Enum id, params object[] param) { Notify(id.GetHashCode(), param); }

        public static void RegEvt(Enum id, EventHandlerDelegate handler, bool once = false)
        {
            RegEvt(id.GetHashCode(), handler, once);
        }

        public static void UnRegEvt(Enum id, EventHandlerDelegate handler) { UnRegEvt(id.GetHashCode(), handler); }

        public static void UnRegAllEvt() { Instance.UnRegisterAll(); }

        public static T DecodeEvt<T>(Evt evt)
        {
            try
            {
                return (T)evt.Param;
            }
            catch (Exception e)
            {
                Helper.Log("Error:{0} EvtID: {1} paramType: {2}", e.Message, evt.ID, evt.Param.GetType().FullName);
                throw;
            }
        }
    }
}
