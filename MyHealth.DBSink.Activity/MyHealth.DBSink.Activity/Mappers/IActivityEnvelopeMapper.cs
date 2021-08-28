using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.Mappers
{
    public interface IActivityEnvelopeMapper
    {
        mdl.ActivityEnvelope MapActivityToActivityEnvelope(mdl.Activity activity);
    }
}
