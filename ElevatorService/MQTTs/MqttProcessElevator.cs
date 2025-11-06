using Common.DTOs;
using Common.Models;
using System.Text.Json;

namespace ElevatorService.MQTTs
{
    public partial class MqttProcess
    {
        //public void PublishElevaotr()
        //{
        //    while (QueueStorage.MqttTryDequeuePublishMission(out MqttPublishMessageDto cmd))
        //    {
        //        try
        //        {
        //            //Console.WriteLine(string.Format("Process Message: [{0}] {1} at {2:yyyy-MM-dd HH:mm:ss,fff}", cmd.Topic, cmd.Payload, cmd.Timestamp));

        //            _mqttWorker.PublishAsync(cmd.Topic, cmd.Payload).Wait();
        //        }
        //        catch (Exception ex)
        //        {
        //            LogExceptionMessage(ex);
        //        }
        //    }
        //}
        public void SubscribeElevator()
        {
            while (QueueStorage.MqttTryDequeueSubscribeElevator(out MqttSubscribeMessageDto subscribe))
            {
                try
                {
                    //Console.WriteLine(string.Format("Process Message: [{0}] {1} at {2:yyyy-MM-dd HH:mm:ss,fff}", subscribe.topic, subscribe.Payload, subscribe.Timestamp));

                    var elevator = _repository.Elevators.GetById(subscribe.id);
                    switch (subscribe.subType)
                    {
                        case nameof(TopicSubType.status):
                            var status = JsonSerializer.Deserialize<MqttSubscribeDtoStatusElevator>(subscribe.Payload!);
                            if (elevator == null)
                            {
                                var create = _mapping.Elevators.MqttCreateElevator(status);
                                _repository.Elevators.Add(create);
                            }
                            else
                            {
                                _mapping.Elevators.MqttUpdate(elevator, status);
                                _repository.Elevators.Update(elevator);
                            }

                            break;

                        case nameof(TopicSubType.command):
                            var commandStateDto = JsonSerializer.Deserialize<MqttSubscribeDtoCommand>(subscribe.Payload!);
                            var command = _repository.Commands.GetById(commandStateDto.commnadId);
                            if (command != null)
                            {
                                string commandstate = commandStateDto.state.Replace(" ", "").ToUpper();

                                if (commandstate != nameof(MissionState.COMPLETED))
                                {
                                    updateStateCommand(command, commandstate, true);
                                }
                                else
                                {
                                    updateStateCommand(command, commandstate);
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogExceptionMessage(ex);
                }
            }
        }
    }
}