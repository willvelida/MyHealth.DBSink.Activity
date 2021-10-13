using MyHealth.DBSink.Activity.Repository.Interfaces;
using MyHealth.DBSink.Activity.Services.Interfaces;
using System;
using System.Threading.Tasks;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IActivityRepository _activityRepository;

        public ActivityService(
            IActivityRepository activityRepository)
        {
            _activityRepository = activityRepository;
        }

        public async Task AddActivityDocument(mdl.ActivityEnvelope activityEnvelope)
        {
            try
            {
                await _activityRepository.CreateActivity(activityEnvelope);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mdl.ActivityEnvelope MapActivityToActivityEnvelope(mdl.Activity activity)
        {
            if (activity == null)
                throw new Exception("No Activity Document to Map!");

            mdl.ActivityEnvelope activityEnvelope = new mdl.ActivityEnvelope
            {
                Id = Guid.NewGuid().ToString(),
                Activity = activity,
                DocumentType = "Activity",
                Date = activity.ActivityDate
            };

            return activityEnvelope;
        }
    }
}
