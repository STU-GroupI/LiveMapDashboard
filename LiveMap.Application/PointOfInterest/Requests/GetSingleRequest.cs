﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveMap.Application.PointOfInterest.Requests;

public sealed record GetSingleRequest(Guid Id);
