using System;
using System.Reactive.Linq;
using AgentFramework.Core.Runtime;
using Xunit;

namespace AgentFramework.Core.Tests
{
    //Modified from https://github.com/shiftkey/Reactive.EventAggregator
    public class EventAggregatorTests
    {
        class SampleEvent
        {
            public int Status { get; set; }
        }

        [Fact]
        public void EventAggregatorCanSubscribe()
        {
            // arrange
            var eventWasRaised = false;
            var eventPublisher = new EventAggregator();

            // act
            eventPublisher.GetEventByType<SampleEvent>()
                .Subscribe(se => eventWasRaised = true);

            eventPublisher.Publish(new SampleEvent());

            // assert
            Assert.True(eventWasRaised);
        }

        [Fact]
        public void EventAggregatorCanUnSubscribe()
        {
            // arrange
            var eventWasRaised = false;
            var eventPublisher = new EventAggregator();

            // act
            var subscription = eventPublisher.GetEventByType<SampleEvent>()
                                             .Subscribe(se => eventWasRaised = true);

            subscription.Dispose();
            eventPublisher.Publish(new SampleEvent());

            // assert
            Assert.False(eventWasRaised);
        }

        [Fact]
        public void EventAggregatorCanSelectivelySubscribe()
        {
            // arrange
            var eventWasRaised = false;
            var eventPublisher = new EventAggregator();

            // act
            eventPublisher.GetEventByType<SampleEvent>()
                .Where(se => se.Status == 1)
                .Subscribe(se => eventWasRaised = true);

            eventPublisher.Publish(new SampleEvent { Status = 1 });

            // assert
            Assert.True(eventWasRaised);
        }

        [Fact]
        public void EventAggregatorSelectiveSubscribeIgnored()
        {
            // arrange
            var eventWasRaised = false;
            var eventPublisher = new EventAggregator();

            // act
            eventPublisher.GetEventByType<SampleEvent>()
                .Where(se => se.Status != 1)
                .Subscribe(se => eventWasRaised = true);

            eventPublisher.Publish(new SampleEvent { Status = 1 });

            // assert
            Assert.False(eventWasRaised);
        }
    }
}
