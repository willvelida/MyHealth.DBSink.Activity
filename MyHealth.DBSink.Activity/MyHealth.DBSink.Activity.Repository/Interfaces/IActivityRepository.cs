﻿using MyHealth.Common.Models;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Activity.Repository.Interfaces
{
    public interface IActivityRepository
    {
        Task CreateActivity(ActivityEnvelope activityEnvelope);
    }
}
