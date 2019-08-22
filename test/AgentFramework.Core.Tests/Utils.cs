using NLog;
using NLog.Config;
using NLog.Targets;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace AgentFramework.Core.Tests
{
    public static class Utils
    {
        public static void EnableIndyLogging()
        {
            //Initialization code goes here.
            var config = new LoggingConfiguration();

            // Step 2. Create targets
            var consoleTarget = new ColoredConsoleTarget("target1")
            {
                Layout = @"${date:format=HH\:mm\:ss} ${level} ${message} ${exception}"
            };
            consoleTarget.DetectConsoleAvailable = false;
            config.AddTarget(consoleTarget);

            config.AddRuleForAllLevels(consoleTarget); // all to console

            // Step 4. Activate the configuration
            LogManager.Configuration = config;

            Hyperledger.Indy.Utils.Logger.Init();
        }
    }
}
