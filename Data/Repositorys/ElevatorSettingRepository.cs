using Common.Models;
using log4net;

namespace Data.Repositorys
{
    public class ElevatorSettingRepository
    {
        private static readonly ILog logger = LogManager.GetLogger("ElevatorSetting"); //Function 실행관련 Log
        private readonly string connectionString;
        private readonly List<ElevatorSetting> _elevatorSettings = new List<ElevatorSetting>(); // cached data
        private readonly object _lock = new object();

        public ElevatorSettingRepository(string connectionString)
        {
            this.connectionString = connectionString;
            createTable();
            Load();
        }

        private void createTable()
        {
        }

        private void Load()
        {
        }

        public void Add(ElevatorSetting add)
        {
            _elevatorSettings.Add(add);
            logger.Info($"Add: {add}");
        }

        public void Update(ElevatorSetting update)
        {
            logger.Info($"Update: {update}");
        }

        public void Delete()
        {
            logger.Info($"Delete");
        }

        public void Remove(ElevatorSetting remove)
        {
            _elevatorSettings.Remove(remove);
            logger.Info($"Remove: {remove}");
        }

        public List<ElevatorSetting> GetAll()
        {
            lock (_lock)
            {
                return _elevatorSettings.ToList();
            }
        }

        public ElevatorSetting GetById(string id)
        {
            lock (_lock)
            {
                return _elevatorSettings.FirstOrDefault(m => m.id == id);
            }
        }
    }
}