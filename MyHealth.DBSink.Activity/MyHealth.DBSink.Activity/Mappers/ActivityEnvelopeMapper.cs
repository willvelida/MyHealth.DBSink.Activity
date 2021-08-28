using MyHealth.Common.Models;
using System;
using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.Mappers
{
    public class ActivityEnvelopeMapper : IActivityEnvelopeMapper
    {
        public mdl.ActivityEnvelope MapActivityToActivityEnvelope(mdl.Activity activity)
        {
            if (activity == null)
                throw new Exception("No Activity Document to Map!");

            mdl.ActivityEnvelope activityEnvelope = new ActivityEnvelope
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
