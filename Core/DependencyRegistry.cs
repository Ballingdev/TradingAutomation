using StructureMap;

namespace TradingAutomation.Core
{
    public class DependencyRegistry : Registry
    {
        public DependencyRegistry()
        {
            For<AccountVariables>().Singleton();
        }    
    }
}