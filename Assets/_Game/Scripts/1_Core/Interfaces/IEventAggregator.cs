using System;

namespace _Game.Scripts.Core.Interfaces
{
    /// <summary>
    /// Interface for an Event Aggregator, which facilitates communication between components
    /// by allowing subscription, unsubscription, and publishing of events.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Subscribes to an event of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the event to subscribe to.</typeparam>
        /// <param name="callback">The callback to invoke when the event is published.</param>
        void Subscribe<T>(Action<T> callback) where T : class;

        /// <summary>
        /// Unsubscribes from an event of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the event to unsubscribe from.</typeparam>
        /// <param name="callback">The callback to remove from the subscription list.</param>
        void Unsubscribe<T>(Action<T> callback) where T : class;

        /// <summary>
        /// Publishes an event of type <typeparamref name="T"/> to all subscribers.
        /// </summary>
        /// <typeparam name="T">The type of the event to publish.</typeparam>
        /// <param name="event">The event instance to publish.</param>
        void Publish<T>(T @event) where T : class;

        /// <summary>
        /// Unsubscribes all callbacks for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        void UnsubscribeAll<T>() where T : class;

        /// <summary>
        /// Unsubscribes all callbacks for all event types.
        /// </summary>
        void UnsubscribeAll();

        /// <summary>
        /// Checks if there are any subscribers for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <returns>True if there are subscribers, false otherwise.</returns>
        bool HasSubscribers<T>() where T : class;

        /// <summary>
        /// Gets the number of subscribers for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of the event.</typeparam>
        /// <returns>The number of subscribers.</returns>
        int GetSubscriberCount<T>() where T : class;
    }
}