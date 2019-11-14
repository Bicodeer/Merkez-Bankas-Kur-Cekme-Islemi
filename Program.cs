using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace kur
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<anaClass>(s =>
                {
                    s.ConstructUsing(anaSinif => new anaClass());
                    s.WhenStarted(anaSinif => anaSinif.start());
                    s.WhenStopped(anaSinif => anaSinif.stop());
                });
                x.RunAsLocalSystem();
                x.SetServiceName("KurServis");
                x.SetDisplayName("DovizKurServis");
                x.SetDescription("Doviz Kuru Çeken Windows Servis");

            });
            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetType());
            Environment.ExitCode = exitCodeValue;

        }
    }
}
