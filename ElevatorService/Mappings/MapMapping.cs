using Common.DTOs.Rests.Maps;
using Common.Models;

namespace ElevatorService.Mappings
{
    public class MapMapping
    {
        public Map Response(Response_MapDto model)
        {
            var response = new Map
            {
                id = model._id,
                mapId = model.mapId,
                source = model.source,
                level = model.level,
                name = model.name,
            };
            return response;
        }
    }
}