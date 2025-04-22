using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using _Game.Scripts.Core.Interfaces;

namespace _Game.Scripts.Application
{
    /// <summary>
    /// Implementation of the Event Aggregator pattern, allowing components to communicate
    /// through events without direct dependencies.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
        private readonly ReaderWriterLockSlim _lock = new();
        private readonly Dictionary<Type, List<Delegate>> _cachedSubscribers = new();
        private bool _isCacheDirty = true;

        /// <inheritdoc />
        public void Subscribe<T>(Action<T> callback) where T : class
        {
            if (callback == null)
            {
                Debug.LogWarning("Attempted to subscribe with null callback");
                return;
            }

            var type = typeof(T);
            _lock.EnterWriteLock();
            try
            {
                if (!_subscribers.ContainsKey(type))
                    _subscribers[type] = new List<Delegate>();

                _subscribers[type].Add(callback);
                _isCacheDirty = true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public void Unsubscribe<T>(Action<T> callback) where T : class
        {
            if (callback == null)
            {
                Debug.LogWarning("Attempted to unsubscribe with null callback");
                return;
            }

            var type = typeof(T);
            _lock.EnterWriteLock();
            try
            {
                if (!_subscribers.ContainsKey(type)) return;

                _subscribers[type].Remove(callback);
                _isCacheDirty = true;

                // Clean up empty lists
                if (!_subscribers[type].Any())
                    _subscribers.Remove(type);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <inheritdoc />
        public void Publish<T>(T @event) where T : class
        {
            if (@event == null)
            {
                Debug.LogWarning("Attempted to publish null event");
                return;
            }

            var type = @event.GetType();
            _lock.EnterReadLock();
            try
            {
                if (!_subscribers.ContainsKey(type)) return;

                var callbacks = GetCachedSubscribers<T>(type);
                foreach (var callback in callbacks)
                {
                    try
                    {
                        callback(@event);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error invoking callback for event type {type.Name}: {ex.Message}\n{ex.StackTrace}");
                    }
                }
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Unsubscribes all callbacks for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        public void UnsubscribeAll<T>() where T : class
        {
            var type = typeof(T);
            _lock.EnterWriteLock();
            try
            {
                if (_subscribers.ContainsKey(type))
                {
                    _subscribers.Remove(type);
                    _isCacheDirty = true;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Unsubscribes all callbacks for all event types.
        /// </summary>
        public void UnsubscribeAll()
        {
            _lock.EnterWriteLock();
            try
            {
                _subscribers.Clear();
                _cachedSubscribers.Clear();
                _isCacheDirty = true;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Checks if there are any subscribers for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <returns>True if there are subscribers, false otherwise.</returns>
        public bool HasSubscribers<T>() where T : class
        {
            var type = typeof(T);
            _lock.EnterReadLock();
            try
            {
                return _subscribers.ContainsKey(type) && _subscribers[type].Any();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets the number of subscribers for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <returns>The number of subscribers.</returns>
        public int GetSubscriberCount<T>() where T : class
        {
            var type = typeof(T);
            _lock.EnterReadLock();
            try
            {
                return _subscribers.ContainsKey(type) ? _subscribers[type].Count : 0;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private List<Action<T>> GetCachedSubscribers<T>(Type type) where T : class
        {
            if (!_isCacheDirty && _cachedSubscribers.ContainsKey(type))
            {
                return _cachedSubscribers[type].OfType<Action<T>>().ToList();
            }

            _lock.EnterWriteLock();
            try
            {
                if (!_isCacheDirty && _cachedSubscribers.ContainsKey(type))
                {
                    return _cachedSubscribers[type].OfType<Action<T>>().ToList();
                }

                _cachedSubscribers.Clear();
                foreach (var kvp in _subscribers)
                {
                    _cachedSubscribers[kvp.Key] = new List<Delegate>(kvp.Value);
                }
                _isCacheDirty = false;

                return _cachedSubscribers[type].OfType<Action<T>>().ToList();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}