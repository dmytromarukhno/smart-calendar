namespace SmartCalendar.Application.Interfaces;

public interface ISceneScheduler
{
    void ScheduleTrigger(Guid sceneId, DateTime triggerAt);
    void Cancel(Guid sceneId);
}
