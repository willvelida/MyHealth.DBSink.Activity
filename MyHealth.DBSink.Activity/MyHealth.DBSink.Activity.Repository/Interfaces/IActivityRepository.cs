using MyHealth.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Activity.Repository.Interfaces
{
    public interface IActivityRepository
    {
        Task CreateActivity(ActivityEnvelope activityEnvelope);
    }
}
