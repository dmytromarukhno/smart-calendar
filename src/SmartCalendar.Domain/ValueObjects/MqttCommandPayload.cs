namespace SmartCalendar.Domain.ValueObjects;

public sealed record MqttCommandPayload(string Action, string Value);
