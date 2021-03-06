﻿using System;
using System.Collections.Generic;

namespace Cottle
{
    public interface IMap : IComparable<IMap>, IEnumerable<KeyValuePair<Value, Value>>, IEquatable<IMap>
    {
        /// <summary>
        /// Get value associated to given key if it exists, or undefined value otherwise.
        /// </summary>
        /// <param name="key">Requested key</param>
        Value this[Value key] { get; }

        /// <summary>
        /// Get number of elements in this map.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Check whether this map contains a pair with given key or not.
        /// </summary>
        /// <param name="key">Requested key</param>
        /// <returns>True if key was found, false otherwise</returns>
        bool Contains(Value key);

        /// <summary>
        /// Retrieve value from pair associated to given key.
        /// </summary>
        /// <param name="key">Requested key</param>
        /// <param name="value">Output value if found</param>
        /// <returns>True if value was found, false otherwise</returns>
        bool TryGet(Value key, out Value value);
    }
}