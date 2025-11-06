using Common.Models;
using System.Reflection;

namespace ElevatorService.Services
{
    public partial class ElevatorService
    {
        public void Schduler()
        {
            postCommandControl();
        }

        private void postCommandControl()
        {
            var commands = _repository.Commands.GetAll();
            if (commands.Count == 0) return;
            var runCommands = _repository.Commands.GetByRunCommands(commands);

            var command = commands.Where(m => (m.state == nameof(CommandState.WAITING))
                                                   || (m.state == nameof(CommandState.FAILED))
                                                   || (m.state == nameof(CommandState.COMMANDREQUEST))
                                                      ).FirstOrDefault();
            if (command != null && runCommands.Count ==0)
            {
                postCommand(command);
            }
        }

        private bool postCommand(Command command)
        {
            bool CommandRequst = false;
            //[조건1] 미션 상태가 COMMANDREQUEST 다르면 COMMANDREQUEST 로 상태변경한다
            if (command.state != nameof(CommandState.COMMANDREQUEST))
            {
                updateStateCommand(command, nameof(CommandState.COMMANDREQUEST), true);
            }

            switch (command.service)
            {
                case nameof(Service.NO1):
                    //[조건2] Service Api를 조회한다.
                    var elevaotrApi = _repository.ServiceApis.GetAll().FirstOrDefault(r => r.type == nameof(Service.NO1));
                    if (elevaotrApi != null)
                    {
                        //[조건3] API 형식에 맞추어서 Mapping 을 한다.
                        var mapping_Command = _mapping.Commands.ApiRequestDtoPostCommand(command);
                        if (mapping_Command != null)
                        {
                            //[조건4] Service 로 Api Mission 전송을 한다.
                            var postCommand = elevaotrApi.Api.ElevatorPostCommandQueueAsync(mapping_Command).Result;
                            if (postCommand != null)
                            {
                                //[조건5] 상태코드 200~300 까지는 완료 처리
                                if (postCommand.statusCode >= 200 && postCommand.statusCode < 300)
                                {
                                    EventLogger.Info($"PostMission Success = Service = {nameof(Service.NO1)}, Message = {postCommand.statusText}, CommandName = {command.name}, CommandId = {command.guid}, AssignedWorkerId = {command.assignedWorkerId}");
                                    CommandRequst = true;
                                }
                                else EventLogger.Info($"PostMission Failed = Service = {nameof(Service.NO1)}, Message = {postCommand.message}, CommandName = {command.name}, CommandId = {command.guid}, AssignedWorkerId = {command.assignedWorkerId}");
                            }
                        }
                    }

                    break;
            }
            if (CommandRequst)
            {
                updateStateCommand(command, nameof(CommandState.COMMANDREQUESTCOMPLETED), true);
            }
            return CommandRequst;
        }
    }
}
