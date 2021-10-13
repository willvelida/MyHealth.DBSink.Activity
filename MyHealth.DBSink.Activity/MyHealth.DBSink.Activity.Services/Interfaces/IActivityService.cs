using mdl = MyHealth.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        mdl.ActivityEnvelope MapActivityToActivityEnvelope(mdl.Activity activity);
        Task AddActivityDocument(mdl.ActivityEnvelope activityEnvelope);
    }
}
