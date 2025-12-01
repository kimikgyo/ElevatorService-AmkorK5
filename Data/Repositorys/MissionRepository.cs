using Common.Models;
using Dapper;
using log4net;
using Microsoft.Data.SqlClient;

namespace Data.Repositorys
{
    public class MissionRepository
    {
        private static readonly ILog logger = LogManager.GetLogger("Mission"); //Function 실행관련 Log
        private readonly string connectionString;
        private readonly List<Mission> _missions = new List<Mission>(); // cached data
        private readonly object _lock = new object();

        public MissionRepository(string connectionString)
        {
            this.connectionString = connectionString;
            createTable();
            Load();
        }

        private void createTable()
        {
            //VARCHAR 대신 NVARCHAR로 저장해야함 VARCHAR은 영문만 가능함
            // 테이블 존재 여부 확인 쿼리
            string checkTableQuery = @"
               IF OBJECT_id('dbo.[Mission]', 'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.[Mission]
                    (
                         [guid]                  NVARCHAR(64)     NULL
                        ,[state]                 NVARCHAR(64)     NULL
                        ,[createdAt]             datetime         NULL
                        ,[updatedAt]             datetime         NULL
                        ,[finishedAt]            datetime         NULL
                        ,[elevatorId]            NVARCHAR(64)     NULL
                        ,[sourceFloor]           NVARCHAR(64)     NULL
                        ,[destinationFloor]      NVARCHAR(64)     NULL
                        ,[requestMode]           NVARCHAR(64)     NULL
                        ,[terminationType]       NVARCHAR(64)     NULL
                        ,[orderId]               NVARCHAR(64)     NULL
                        ,[jobId]                 NVARCHAR(64)     NULL
                        ,[acsMissionId]          NVARCHAR(64)     NULL
                        ,[carrierId]             NVARCHAR(64)     NULL
                        ,[service]               NVARCHAR(64)     NULL
                        ,[type]                  NVARCHAR(64)     NULL
                        ,[subType]               NVARCHAR(64)     NULL
                        ,[linkedFacility]        NVARCHAR(64)     NULL
                        ,[sequence]              int              NULL
                        ,[isLocked]              int              NULL
                        ,[sequenceChangeCount]   int              NULL
                        ,[retryCount]            int              NULL
                        ,[specifiedWorkerId]     NVARCHAR(64)     NULL
                        ,[assignedWorkerId]      NVARCHAR(64)     NULL
                    );
                END;
            ";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(checkTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void Load()
        {
            _missions.Clear();
            using (var con = new SqlConnection(connectionString))
            {
                foreach (var data in con.Query<Mission>("SELECT * FROM [Mission]"))
                {
                    _missions.Add(data);

                    logger.Info($"Load:{data}");
                }
            }
        }

        public void Add(Mission add)
        {
            lock (_lock)
            {
                using (var con = new SqlConnection(connectionString))
                {
                    const string INSERT_SQL = @"
                            INSERT INTO [Mission]
                                 (
                                       [guid]
                                      ,[state]
                                      ,[elevatorId]
                                      ,[sourceFloor]
                                      ,[destinationFloor]
                                      ,[requestMode]
                                      ,[createdAt]
                                      ,[updatedAt]
                                      ,[finishedAt]
                                      ,[terminationType]
                                      ,[orderId]
                                      ,[jobId]
                                      ,[acsMissionId]
                                      ,[carrierId]
                                      ,[service]
                                      ,[type]
                                      ,[subType]
                                      ,[linkedFacility]
                                      ,[sequence]
                                      ,[isLocked]
                                      ,[sequenceChangeCount]
                                      ,[retryCount]
                                      ,[specifiedWorkerId]
                                      ,[assignedWorkerId]
                                   )
                                  values
                                  (
                                         @guid
                                        ,@state
                                        ,@elevatorId
                                        ,@sourceFloor
                                        ,@destinationFloor
                                        ,@requestMode
                                        ,@createdAt
                                        ,@updatedAt
                                        ,@finishedAt
                                        ,@terminationType
                                        ,@orderId
                                        ,@jobId
                                        ,@acsMissionId
                                        ,@carrierId
                                        ,@service
                                        ,@type
                                        ,@subType
                                        ,@linkedFacility
                                        ,@sequence
                                        ,@isLocked
                                        ,@sequenceChangeCount
                                        ,@retryCount
                                        ,@specifiedWorkerId
                                        ,@assignedWorkerId
                                  );";
                    //TimeOut 시간을 60초로 연장 [기본30초]
                    //con.Execute(INSERT_SQL, param: add, commandTimeout: 60);
                    con.Execute(INSERT_SQL, param: add);
                    _missions.Add(add);
                    logger.Info($"Add: {add}");
                }
            }
        }

        public void Update(Mission update)
        {
            lock (_lock)
            {
                using (var con = new SqlConnection(connectionString))
                {
                    const string UPDATE_SQL = @"
                            UPDATE [Mission]
                            SET
                                 [state]                  = @state
                                ,[elevatorId]             = @elevatorId
                                ,[sourceFloor]            = @sourceFloor
                                ,[destinationFloor]       = @destinationFloor
                                ,[requestMode]            = @requestMode
                                ,[createdAt]              = @createdAt
                                ,[updatedAt]              = @updatedAt
                                ,[finishedAt]             = @finishedAt
                                ,[terminationType]        = @terminationType
                                ,[orderId]                = @orderId
                                ,[jobId]                  = @jobId
                                ,[acsMissionId]           = @acsMissionId
                                ,[carrierId]              = @carrierId
                                ,[service]                = @service
                                ,[type]                   = @type
                                ,[subType]                = @subType
                                ,[linkedFacility]         = @linkedFacility
                                ,[sequence]               = @sequence
                                ,[isLocked]               = @isLocked
                                ,[sequenceChangeCount]    = @sequenceChangeCount
                                ,[retryCount]             = @retryCount
                                ,[specifiedWorkerId]      = @specifiedWorkerId
                                ,[assignedWorkerId]       = @assignedWorkerId

                            WHERE [guid] = @guid";
                    //TimeOut 시간을 60초로 연장 [기본30초]
                    //con.Execute(UPDATE_SQL, param: update, commandTimeout: 60);
                    con.Execute(UPDATE_SQL, param: update);
                    logger.Info($"Update: {update}");
                }
            }
        }

        public void Delete()
        {
            lock (_lock)
            {
                string massage = null;

                using (var con = new SqlConnection(connectionString))
                {
                    con.Execute("DELETE FROM [Mission]");
                    _missions.Clear();
                    logger.Info($"Delete");
                }
            }
        }

        public void Remove(Mission remove)
        {
            lock (_lock)
            {
                string massage = null;

                using (var con = new SqlConnection(connectionString))
                {
                    con.Execute("DELETE FROM [Mission] WHERE guid = @guid", param: new { guid = remove.guid });
                    _missions.Remove(remove);
                    logger.Info($"Remove: {remove}");
                }
            }
        }

        public List<Mission> GetAll()
        {
            lock (_lock)
            {
                return _missions.ToList();
            }
        }

        public List<Mission> GetByJobId(string jobId)
        {
            lock (_lock)
            {
                return _missions.Where(m => m.jobId == jobId).ToList();
            }
        }

        public List<Mission> GetByOrderId(string orderId)
        {
            lock (_lock)
            {
                return _missions.Where(m => m.orderId == orderId).ToList();
            }
        }

        public List<Mission> GetByAssignedWorkerId(string AssignedWorkerId)
        {
            lock (_lock)
            {
                return _missions.Where(m => m.assignedWorkerId == AssignedWorkerId).ToList();
            }
        }

        public Mission GetByAcsMissionId(string acsMissionId)
        {
            lock (_lock)
            {
                return _missions.FirstOrDefault(m => m.acsMissionId == acsMissionId);
            }
        }
    }
}