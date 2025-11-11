using Data.Repositorys;
using Data.Repositorys.Historys;
using Data.Repositorys.Maps;
using Data.Repositorys.Positions;
using System.Data;

namespace Data.Interfaces
{
    public class ConnectionStrings
    {
        public static readonly string DB1 = @"Data SOURCE=.\SQLEXPRESS;Initial Catalog=AmkorK5; User ID = sa;TrustServerCertificate=true; Password=acsserver;Connect Timeout=30;";
        //public static readonly string DB1 = @"Data Source=192.168.8.215,1433; Initial Catalog=JobScheduler; User ID = sa; Password=acsserver; Connect Timeout=30; TrustServerCertificate=true"; // STI
    }

    public class UnitOfWorkRepository : IUnitOfWorkRepository
    {
        private IDbConnection _db;

        private static readonly string connectionString = ConnectionStrings.DB1;

        #region Base

        public MapRepository Maps { get; private set; }
        public PositionRepository Positions { get; private set; }
        public DeviceRepository Devices { get; private set; }

        #endregion Base

        public MissionRepository Missions { get; private set; }
        public CommandRepository Commands { get; private set; }

        public MissionHistoryRepository MissionHistorys { get; private set; }

        public MissionFinishedHistoryRepository MissionFinishedHistorys { get; private set; }

        public ServiceApiRepository ServiceApis { get; private set; }

        public UnitOfWorkRepository()
        {
            repository();
        }

        private void repository()
        {
            #region Base

            Maps = new MapRepository(connectionString);
            Positions = new PositionRepository(connectionString);
            Devices = new DeviceRepository(connectionString);

            #endregion Base

            MissionHistorys = new MissionHistoryRepository(connectionString);
            MissionFinishedHistorys = new MissionFinishedHistoryRepository(connectionString);

            Missions = new MissionRepository(connectionString);
            Commands = new CommandRepository(connectionString);

            ServiceApis = new ServiceApiRepository(connectionString);
        }

        public void SaveChanges()
        {
        }

        public void Dispose()
        {
        }
    }
}