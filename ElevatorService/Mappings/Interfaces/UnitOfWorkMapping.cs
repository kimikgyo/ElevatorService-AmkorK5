namespace ElevatorService.Mappings.Interfaces
{
    public class UnitOfWorkMapping : IUnitOfWorkMapping
    {
        public MissionMapping Missions { get; private set; }
        public PositionMapping Positions { get; private set; }
        public MapMapping Maps { get; private set; }
        public ElevatorMapping Elevators { get; private set; }
        public CommandMapping Commands { get; private set; }

        public UnitOfWorkMapping()
        {
            mapping();
        }

        private void mapping()
        {
            Missions = new MissionMapping();
            Positions = new PositionMapping();
            Maps = new MapMapping();
            Elevators = new ElevatorMapping();
            Commands = new CommandMapping();
        }

        public void SaveChanges()
        {
        }

        public void Dispose()
        {
        }
    }
}