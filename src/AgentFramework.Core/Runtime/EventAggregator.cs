using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AgentFramework.Core.Contracts;

namespace AgentFramework.Core.Runtime
{
    /// <inheritdoc />
    //Modified from https://github.com/shiftkey/Reactive.EventAggregator
    public class EventAggregator : IEventAggregator
    {
        private readonly Subject<object> _subject = new Subject<object>();

        /// <inheritdoc />
        public IObservable<TEvent> GetEventByType<TEvent>() => _subject.OfType<TEvent>().AsObservable();

        /// <inheritdoc />
        public void Publish<TEvent>(TEvent eventToPublish) => _subject.OnNext(eventToPublish);
    }
}
