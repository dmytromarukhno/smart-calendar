using SmartCalendar.Application.Interfaces;
using SmartCalendar.Domain.ValueObjects;

namespace SmartCalendar.Infrastructure.Mqtt;

public sealed class ConsoleMqttPublisher : IMqttPublisher
{
    public Task PublishAsync(string topic, MqttCommandPayload payload, CancellationToken ct = default)
    {
        Console.WriteLine($"[MQTT-mock] {topic} → {{\"action\":\"{payload.Action}\",\"value\":\"{payload.Value}\"}}");
        return Task.CompletedTask;
    }
}
