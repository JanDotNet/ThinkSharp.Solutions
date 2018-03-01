using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using ThinkSharp.Solutions.ServiceFromTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkSharp.Solutions.Infrastructure
{
    public static class VisualStudioHelper
    {
        public static string GetSolutionDirectory()
        {
            var application = ServiceFromTemplateCommand.Instance.ServiceProvider.GetService(typeof(SDTE)) as DTE2;
            if (!application.Solution.IsOpen)
                return null;

            return System.IO.Path.GetDirectoryName(application.Solution.FullName);
        }
    }
}
