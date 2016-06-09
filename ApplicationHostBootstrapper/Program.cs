using System;
using System.Threading;
using ApplicationHost;

namespace ApplicationHostBootstrapper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var setup = new AppDomainSetup();
            setup.ApplicationBase = args[0];
            setup.ConfigurationFile = args[1];
            var appDomain = AppDomain.CreateDomain("", null, setup);
            appDomain.DoCallBack(new CrossAppDomainDelegate(ApplicationBootstrapper.Bootstrap));
            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
}
