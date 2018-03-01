using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkSharp.Solutions.ServiceFromTemplate
{
    public class StepContext
    {
        public string TargetDirectory { get; set; }
        public List<string> Errors { get; } = new List<string>();
    }
}
