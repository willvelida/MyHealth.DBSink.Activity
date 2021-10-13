using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.Services.Interfaces
{
    public interface IActivityService
    {
        mdl.ActivityEnvelope MapActivityToActivityEnvelope(mdl.Activity activity);
        Task AddActivityDocument(mdl.ActivityEnvelope activityEnvelope);
    }
}
