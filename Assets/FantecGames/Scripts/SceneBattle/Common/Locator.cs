using System.Collections.Generic;
using UnityEngine;
using System;

namespace fantec.Battle
{
    /// <summary>
    /// サービスロケーター
    /// </summary>
    public static class Locator
    {
        /// <summary>
        /// 単一インスタンス用ディクショナリー
        /// </summary>
        private static Dictionary<Type, object> _instanceDict = new Dictionary<Type, object>();

        /// <summary>
        /// 単一インスタンスを登録する
        /// 呼び出すと上書き登録する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">インスタンス</param>
        public static void Register<T>(T instance)where T : ILocatable
        {
            if(_instanceDict.ContainsKey(typeof(T)))
            {
                Debug.Log($"{typeof(T)} への上書き登録が行われました。");
            }
            else
            {
                _instanceDict[typeof(T)]= instance;
            }
        }

        /// <summary>
        /// 型を指定して登録されているインスタンスを取得する
        /// </summary>
        /// <typeparam name="T">型</typeparam>
        /// <returns>インスタンス</returns>
        public static T Resolve<T>() where T : class
        {
            Type type= typeof(T);

            T instance;
            if(_instanceDict.ContainsKey(type))
            {
                // 事前に生成された単一インスタンスを返す
                instance = _instanceDict[type] as T;
                return instance;
            }
            else
            {
                throw new KeyNotFoundException($"Locator: {typeof(T).Name} not found.");
            }
        }

        /// <summary>
        /// 型を指定して登録されているインスタンスを取り除く
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Remove<T>() where T: ILocatable
        {
            _instanceDict.Remove(typeof(T));
        }

        /// <summary>
        /// 登録中のものをすべてDispose
        /// </summary>
        public static void Dispose()
        {
            foreach(var locator in _instanceDict.Values)
            {
                if (locator is IDisposable)
                    (locator as IDisposable).Dispose();
            }
        }

        /// <summary>
        /// 登録中のものをすべて取り除く
        /// </summary>
        public static void Clear()
        {
            _instanceDict.Clear();
        }
    }
}