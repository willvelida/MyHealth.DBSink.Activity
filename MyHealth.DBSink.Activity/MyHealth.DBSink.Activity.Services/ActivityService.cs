using mdl = MyHealth.Common.Models;
using MyHealth.DBSink.Activity.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyHealth.DBSink.Activity.Repository.Interfaces;

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
