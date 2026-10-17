using System.Collections.Concurrent;
using System.Net.ServerSentEvents;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace TestSSE;

public sealed class SseBroadcaster
{
    private readonly ConcurrentDictionary<Guid, Channel<string>> _subscribers = new();

    public async IAsyncEnumerable<SseItem<string>> Subscribe(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();
        var channel = Channel.CreateUnbounded<string>();
        _subscribers[id] = channel;

        try
        {
            yield return new SseItem<string>("connected", eventType: "connected");

            await foreach (var message in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return new SseItem<string>(message, eventType: "message");
            }
        }
        finally
        {
            _subscribers.TryRemove(id, out _);
        }
    }

    public void Broadcast(string message)
    {
        foreach (var subscriber in _subscribers.Values)
        {
            subscriber.Writer.TryWrite(message);
        }
    }
}
