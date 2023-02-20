using System.ServiceProcess;

namespace HUMTBot
{
    internal static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new HUMTBotService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
