using LiveMap.Application.Map.Persistance;
using LiveMap.Application.Map.Requests;
using LiveMap.Application.Map.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveMap.Application.Map.Handlers;
public class UpdateBorderHandler : IRequestHandler<UpdateBorderRequest, UpdateBorderResponse>
{
    private readonly IMapRepository _repository;

    public UpdateBorderHandler(IMapRepository repository)
    {
        _repository = repository;
    }

    public async Task<UpdateBorderResponse> Handle(UpdateBorderRequest request)
    {
        bool response = await _repository.UpdateMapBorder(request.Id, request.Coords);
        return new UpdateBorderResponse(response);
    }
}
