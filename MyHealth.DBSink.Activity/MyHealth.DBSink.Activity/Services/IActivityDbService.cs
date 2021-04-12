using MyHealth.DBSink.Activity.Models;
using System.Threading.Tasks;

namespace MyHealth.DBSink.Activity.Services
{
    public interface IActivityDbService
    {
        Task AddActivityDocument(ActivityDocument activityDocument);
    }
}
