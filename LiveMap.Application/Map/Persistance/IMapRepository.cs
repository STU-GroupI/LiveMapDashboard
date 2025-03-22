using LiveMap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveMap.Application.Map.Persistance;
public interface IMapRepository
{
    public Task<Domain.Models.Map?> GetSingle(Guid id);

    public Task<ICollection<Domain.Models.Map>> GetMultiple(int? skip, int? take);

    public Task<bool> UpdateMapBorder(Guid id, Coordinate[] coords);
}

