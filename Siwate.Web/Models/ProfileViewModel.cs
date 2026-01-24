using System.Collections.Generic;

namespace Siwate.Web.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<InterviewResult> History { get; set; }
    }
}
