using System;

namespace AgentFramework.Core.Contracts
{
    /// <summary>
    /// Event Aggregator.
    /// </summary>
    /// Modified from https://github.com/shiftkey/Reactive.EventAggregator
    public interface IEventAggregator
    {
        /// <summary>
        /// Gets an observable for an event type.
        /// </summary>
        /// <typeparam name="TEvent">Type of the event to fetch the observable for.</typeparam>
        /// <returns></returns>
        IObservable<TEvent> GetEventByType<TEvent>();

        /// <summary>
        /// Publishes a particular event.
        /// </summary>
        /// <typeparam name="TEvent">Event to publish.</typeparam>
        /// <param name="eventToPublish"></param>
        void Publish<TEvent>(TEvent eventToPublish);
    }
}
