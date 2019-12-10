using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace LoggingService
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<Logging>(s =>
                {
                    s.ConstructUsing(logging => new Logging());
                    s.WhenStarted(logging => logging.Start());
                    s.WhenStopped(logging => logging.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("LoggingService");
                x.SetDisplayName("Logging Service");
                x.SetDescription("This is the sample service that logs events every second.");

               
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
