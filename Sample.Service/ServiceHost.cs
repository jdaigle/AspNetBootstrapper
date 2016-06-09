using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationHost;

namespace Sample.Service
{
    public class ServiceHost : IBootrapper
    {
        public void Init()
        {
            Debug.Assert(ConfigurationManager.AppSettings["setting1"] == "value1");
        }
    }
}
