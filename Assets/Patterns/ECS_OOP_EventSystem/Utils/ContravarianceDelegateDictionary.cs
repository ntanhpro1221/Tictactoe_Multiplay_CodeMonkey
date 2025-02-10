﻿using System;
using System.Collections.Generic;

namespace ECS_OOP_EventSystem {
    /// <summary>
    /// This class Acts as wrapper for <see cref="Dictionary{TKey, TValue}"/>.<br/>
    /// You can combine <see cref="Delegate"/> (with explicit cast delegate) without losing of <see cref="Delegate"/> references ==> You can remove delegate in future.
    /// </summary>
    /// <typeparam name="TKey">Key type of dictionary</typeparam>
    /// <typeparam name="TValue">Delegate Value type of dictionary</typeparam>
    internal class ContravarianceDelegateDictionary<TKey, TValue> where TValue : Delegate {
        public Dictionary<TKey, TValue> Dict { get; } = new();

        private readonly Dictionary<object, TValue> wrapperDict = new();

        #region MAIN FUNCTION
        /// <summary>
        /// Acts as Dict += value;
        /// </summary>
        /// <param name="wrapFunction">Convert from Delegate to TValue to store</param>
        /// <returns></returns>
        public TValue AddToKey(TKey key, Delegate value, Func<Delegate, TValue> wrapFunction) {
            if (wrapFunction == null) throw new InvalidWrapFunction();

            // store wrapper value for removing "value" in future
            wrapperDict.Add(value, wrapFunction.Invoke(value));

            // add wrapper to dict
            Ensure_Dict_KeyExist(key);
            Dict[key] = (TValue)Delegate.Combine(Dict[key], wrapperDict[value]);

            // return value after add delegate like "+=" operator's behaviour
            return Dict[key];
        }

        /// <summary>
        /// Acts as Dict -= value;
        /// </summary>
        /// <returns></returns>
        public TValue RemoveFromKey(TKey key, Delegate value) {
            // remove stored delegate value
            Ensure_Dict_KeyExist(key);
            Dict[key] = (TValue)Delegate.Remove(Dict[key], wrapperDict[value]);

            // remove key in wrapperDict because it will never be use again
            wrapperDict.Remove(value);

            // return value after remove delegate like "-=" operator's behaviour
            return Dict[key];
        }
        #endregion

        #region CONVINIENT FUNCTION
        private void Ensure_Dict_KeyExist(TKey key) {
            if (!Dict.ContainsKey(key))
                Dict.Add(key, null);
        }

        public TValue this[TKey key]
            => Dict[key];

        public bool ContainsKey(TKey key)
            => Dict.ContainsKey(key);

        public void Add(TKey key, TValue value)
            => Dict.Add(key, value);

        public void Remove(TKey key)
            => Dict.Remove(key);

        public static implicit operator Dictionary<TKey, TValue>(ContravarianceDelegateDictionary<TKey, TValue> value)
            => value.Dict;
        #endregion

        public class InvalidWrapFunction : Exception {
            public InvalidWrapFunction() : base("Invalid wrap function") { }
        }
    }
}
