using Common.DTOs.MQTTs.Messages;
using Common.Models;
using Data.Interfaces;
using ElevatorService.Mappings.Interfaces;
using ElevatorService.MQTTs.Interfaces;
using JOB.MQTTs.Interfaces;
using log4net;
using System.Diagnostics;

namespace ElevatorService.MQTTs
{
    public partial class MqttProcess
    {
        private static readonly ILog EventLogger = LogManager.GetLogger("Event");
        private readonly IMqttWorker _mqttWorker;
        private readonly IUnitOfWorkRepository _repository;
        private readonly IUnitOfWorkMapping _mapping;
        private readonly UnitofWorkMqttQueue _mqttQueue;

        public MqttProcess(UnitofWorkMqttQueue mqttQueue, IMqttWorker mqttWorker, IUnitOfWorkRepository repository, IUnitOfWorkMapping mapping)
        {
            _mqttQueue = mqttQueue;
            _mqttWorker = mqttWorker;
            _repository = repository;
            _mapping = mapping;
        }

        public void HandleReceivedMqttMessage()
        {
            while (QueueStorage.MqttTryDequeueSubscribe(out SubscribeDto message))
            {
                try
                {
                    //Console.WriteLine(string.Format("Process Message: [{0}] {1} at {2:yyyy-MM-dd HH:mm:ss,fff}", message.topic, message.Payload, message.Timestamp));

                    if (string.IsNullOrWhiteSpace(message.topic)) return;
                    if (string.IsNullOrWhiteSpace(message.Payload)) return;     // 페이로드 null check
                    if (!message.Payload.IsValidJson()) return;                 // 페이로드 json check
                    string[] topic = message.topic.Split('/');

                    message.type = topic[1];
                    message.id = topic[2];
                    message.subType = topic[3];

                    _mqttQueue.MqttSubscribe(message);
                }
                catch (Exception ex)
                {
                    LogExceptionMessage(ex);
                }
            }
        }

        public void LogExceptionMessage(Exception ex)
        {
            //string message = ex.InnerException?.Message ?? ex.Message;
            //string message = ex.ToString();
            string message = ex.GetFullMessage() + Environment.NewLine + ex.StackTrace;
            Debug.WriteLine(message);
            EventLogger.Info(message);
        }

        public void updateStateMission(Mission mission, string state, bool historyAdd = false)
        {
            mission.state = state;
            switch (mission.state)
            {
                case nameof(MissionState.PENDING):
                case nameof(MissionState.EXECUTING):
                case nameof(MissionState.FAILED):
                case nameof(MissionState.ABORTINITED):
                case nameof(MissionState.ABORTCOMPLETED):
                case nameof(MissionState.ABORTFAILED):
                case nameof(MissionState.CANCELINITED):
                case nameof(MissionState.CANCELINITCOMPLETED):
                case nameof(MissionState.CNACELFAILED):
                    mission.updatedAt = DateTime.Now;
                    break;

                case nameof(MissionState.SKIPPED):
                case nameof(MissionState.CANCELED):
                case nameof(MissionState.COMPLETED):
                    mission.finishedAt = DateTime.Now;
                    break;
            }

            _repository.Missions.Update(mission);
            // if (historyAdd) _repository.MissionHistorys.Add(mission);
            _mqttQueue.MqttPublishMessage(TopicType.mission, TopicSubType.status, _mapping.Missions.MqttPublish(mission));
        }

        public void updateStateCommand(Command command, string state, bool historyAdd = false)
        {
            command.state = state;

            switch (command.state)
            {
                case nameof(CommandState.INIT):
                case nameof(CommandState.WAITING):
                case nameof(CommandState.COMMANDREQUEST):
                case nameof(CommandState.COMMANDREQUESTCOMPLETED):
                case nameof(CommandState.PENDING):
                case nameof(CommandState.EXECUTING):
                case nameof(CommandState.FAILED):
                case nameof(CommandState.ABORTINITED):
                case nameof(CommandState.ABORTCOMPLETED):
                case nameof(CommandState.ABORTFAILED):
                case nameof(CommandState.CANCELINITED):
                case nameof(CommandState.CANCELINITCOMPLETED):
                case nameof(CommandState.CNACELFAILED):
                    command.updatedAt = DateTime.Now;
                    break;

                case nameof(CommandState.SKIPPED):
                case nameof(CommandState.CANCELED):
                case nameof(CommandState.COMPLETED):
                    command.finishedAt = DateTime.Now;
                    break;
            }

            _repository.Commands.Update(command);
            //if (historyAdd) _repository.Commands.Add(command);
            //_mqttQueue.MqttPublishMessage(TopicType., TopicSubType.status, _mapping.Missions.MqttPublish(mission));
        }
    }
}