using mdl = MyHealth.Common.Models;

namespace MyHealth.DBSink.Activity.Models
{
    public class ActivityDocument
    {
        public string Id { get; set; }
        public mdl.Activity Activity { get; set; }
        public string DocumentType { get; set; }
    }
}
