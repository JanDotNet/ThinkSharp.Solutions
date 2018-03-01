using ThinkSharp.Solutions.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkSharp.Solutions.Credentials
{
    public class CredentialsViewModel : DialogViewModel
    {
        public CredentialsViewModel(string username) : base("Enter Credentials")
        {
            UserName = username;
        }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
