namespace ElevatorService.Mappings.Interfaces
{
    public interface IUnitOfWorkMapping : IDisposable
    {
        MissionMapping Missions { get; }
        PositionMapping Positions { get; }
        MapMapping Maps { get; }
        ElevatorMapping Elevators { get; }
        CommandMapping Commands { get; }
    }
}