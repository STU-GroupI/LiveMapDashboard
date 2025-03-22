using LiveMap.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveMap.Application.Map.Requests;
public sealed record UpdateBorderRequest(Guid Id, Coordinate[] Coords);