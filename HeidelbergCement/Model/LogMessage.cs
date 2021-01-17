using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeidelbergCement.Model
{
    public class LogMessage
    {
        public string Id { get; set; } = null;
        public string Title { get; set; } = null;

        public string Text { get; set; } = null;

        public DateTime? ReceivedAt { get; set; } = null;
    }
}
