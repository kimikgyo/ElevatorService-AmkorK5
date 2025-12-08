using Common.Models;

namespace ElevatorService.Services
{
    public partial class ElevatorService
    {
        private void StatusChangeControl()
        {
            missionExecutingControl();
            missionCompleteControl();
            missionCancelCompleteControl();
        }

        private void missionExecutingControl()
        {
            //미션중 하나의미션이라도 Robot에게 전달이 되어 있을경우! Job 및 Order 상태를 변경한다.
            foreach (var command in _repository.Commands.GetAll().Where(m => (m.state == nameof(CommandState.COMMANDREQUESTCOMPLETED))
                                                                          || (m.state == nameof(CommandState.PENDING))
                                                                          || (m.state == nameof(CommandState.SKIPPED))).ToList())
            {
                var mission = _repository.Missions.GetByAcsMissionId(command.acsMissionId);
                if (mission != null)
                {
                    updateStateMission(mission, nameof(MissionState.EXECUTING), true);
                }
            }
        }

        private void missionCompleteControl()
        {
            foreach (var mission in _repository.Missions.GetAll().Where(x => x.state == nameof(MissionState.EXECUTING)))
            {
                var Commands = _repository.Commands.GetByAcsMissionId(mission.acsMissionId);
                if (Commands == null || Commands.Count == 0) continue;

                var command = Commands.FirstOrDefault(s => s.state != nameof(CommandState.COMPLETED) && s.state != nameof(CommandState.SKIPPED));
                if (command == null)
                {
                    updateStateMission(mission, nameof(MissionState.COMPLETED));

                    foreach (var removeCommand in Commands)
                    {
                        _repository.Commands.Remove(removeCommand);
                    }
                    _repository.Missions.Remove(mission);
                }
            }
        }

        private void missionCancelCompleteControl()
        {
            var cancelmissions = _repository.Missions.GetAll().Where(m => m.terminationType == nameof(TerminateType.CANCEL)).ToList();
            if (cancelmissions == null || cancelmissions.Count() == 0) return;

            foreach (var mission in cancelmissions)
            {
                //미션에 여러개의 커맨드가 있을경우 Cancel 아직 되지않은 상태는 클리어하지않는다.
                var cancelCommand = _repository.Commands.GetByAcsMissionId(mission.acsMissionId).FirstOrDefault(r => r.state == nameof(CommandState.CANCELED));
                if (cancelCommand != null)
                {
                    _repository.Commands.Remove(cancelCommand);
                    updateStateMission(mission, nameof(CommandState.CANCELED), true);
                    _repository.Missions.Remove(mission);
                }
            }


          
        }

    }
}