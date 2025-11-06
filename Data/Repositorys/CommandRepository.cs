using Common.Models;
using Dapper;
using log4net;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace Data.Repositorys
{
    public class CommandRepository
    {
        private static readonly ILog logger = LogManager.GetLogger("Command"); //Function 실행관련 Log
        private readonly string connectionString;
        private readonly List<Command> _commands = new List<Command>(); // cached data
        private readonly object _lock = new object();

        public CommandRepository(string connectionString)
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
               IF OBJECT_id('dbo.[Command]', 'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.[Command]
                    (
                         [acsMissionId]          NVARCHAR(64)     NULL
                        ,[guid]                  NVARCHAR(64)     NULL
                        ,[name]                  NVARCHAR(64)     NULL
                        ,[service]               NVARCHAR(64)     NULL
                        ,[type]                  NVARCHAR(64)     NULL
                        ,[subType]               NVARCHAR(64)     NULL
                        ,[sequence]              int              NULL
                        ,[state]                 NVARCHAR(64)     NULL
                        ,[assignedWorkerId]      NVARCHAR(64)     NULL
                        ,[createdAt]             datetime         NULL
                        ,[updatedAt]             datetime         NULL
                        ,[finishedAt]            datetime         NULL
                        ,[parameterJson]         NVARCHAR(2000)     NULL
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
            _commands.Clear();
            using (var con = new SqlConnection(connectionString))
            {
                foreach (var data in con.Query<Command>("SELECT * FROM [Command]"))
                {
                    if(data.parameterJson !=null)data.parameters = JsonSerializer.Deserialize<List<parameter>>(data.parameterJson);

                    _commands.Add(data);

                    logger.Info($"Load:{data}");
                }
            }
        }

        public void Add(Command add)
        {
            lock (_lock)
            {
                using (var con = new SqlConnection(connectionString))
                {
                    const string INSERT_SQL = @"
                            INSERT INTO [Command]
                                 (
                                     [acsMissionId]
                                    ,[guid]
                                    ,[name]
                                    ,[service]
                                    ,[type]
                                    ,[subType]
                                    ,[sequence]
                                    ,[state]
                                    ,[assignedWorkerId]
                                    ,[createdAt]
                                    ,[updatedAt]
                                    ,[finishedAt]
                                    ,[parameterJson]
                                   )
                                  values
                                  (
                                      @acsMissionId
                                     ,@guid
                                     ,@name
                                     ,@service
                                     ,@type
                                     ,@subType
                                     ,@sequence
                                     ,@state
                                     ,@assignedWorkerId
                                     ,@createdAt
                                     ,@updatedAt
                                     ,@finishedAt
                                     ,@parameterJson
                                  );";
                    //TimeOut 시간을 60초로 연장 [기본30초]
                    //con.Execute(INSERT_SQL, param: add, commandTimeout: 60);
                    con.Execute(INSERT_SQL, param: add);
                    _commands.Add(add);
                    logger.Info($"Add: {add}");
                }
            }
        }

        public void Update(Command update)
        {
            lock (_lock)
            {
                using (var con = new SqlConnection(connectionString))
                {
                    const string UPDATE_SQL = @"
                            UPDATE [Command]
                            SET
                               [acsMissionId]         = @acsMissionId
                              ,[name]                 = @name
                              ,[service]              = @service
                              ,[type]                 = @type
                              ,[subType]              = @subType
                              ,[sequence]             = @sequence
                              ,[state]                = @state
                              ,[assignedWorkerId]     = @assignedWorkerId
                              ,[createdAt]            = @createdAt
                              ,[updatedAt]            = @updatedAt
                              ,[finishedAt]           = @finishedAt
                              ,[parameterJson]        = @parameterJson

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
                    con.Execute("DELETE FROM [Command]");
                    _commands.Clear();
                    logger.Info($"Delete");
                }
            }
        }

        public void Remove(Command remove)
        {
            lock (_lock)
            {
                string massage = null;

                using (var con = new SqlConnection(connectionString))
                {
                    con.Execute("DELETE FROM [Command] WHERE guid = @guid", param: new { guid = remove.guid });
                    _commands.Remove(remove);
                    logger.Info($"Remove: {remove}");
                }
            }
        }

        public List<Command> GetAll()
        {
            lock (_lock)
            {
                return _commands.ToList();
            }
        }

        public List<Command> GetByMissionId(string missionId)
        {
            lock (_lock)
            {
                return _commands.Where(m => m.acsMissionId == missionId).ToList();
            }
        }

        public List<Command> GetByAssignedWorkerId(string AssignedWorkerId)
        {
            lock (_lock)
            {
                return _commands.Where(m => m.assignedWorkerId == AssignedWorkerId).ToList();
            }
        }

        public Command GetById(string id)
        {
            lock (_lock)
            {
                return _commands.FirstOrDefault(m => m.guid == id);
            }
        }
        public List<Command> GetByRunCommands(List<Command> command)
        {
            lock (_lock)
            {
                return _commands.Where(m => m.state == nameof(CommandState.PENDING) || m.state == nameof(CommandState.EXECUTING) || m.state == nameof(CommandState.COMMANDREQUESTCOMPLETED)).ToList();
            }
        }
    }
}