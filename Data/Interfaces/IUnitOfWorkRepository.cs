using Data.Repositorys;
using Data.Repositorys.Historys;
using Data.Repositorys.Maps;
using Data.Repositorys.Positions;

namespace Data.Interfaces
{
    public interface IUnitOfWorkRepository : IDisposable
    {
        #region Base

        MapRepository Maps { get; }
        PositionRepository Positions { get; }
        DeviceRepository Devices { get; }

        #endregion Base

        CommandRepository Commands { get; }
        MissionRepository Missions { get; }

        MissionHistoryRepository MissionHistorys { get; }

        MissionFinishedHistoryRepository MissionFinishedHistorys { get; }

        ServiceApiRepository ServiceApis { get; }

        void SaveChanges();
    }
}