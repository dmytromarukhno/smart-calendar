using SmartCalendar.Domain.ValueObjects;

namespace SmartCalendar.Application.Interfaces;

public interface IMqttPublisher
{
    Task PublishAsync(string topic, MqttCommandPayload payload, CancellationToken ct = default);
}
