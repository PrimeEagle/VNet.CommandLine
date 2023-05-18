namespace VNet.CommandLine
{
    public interface IConditionProcessor
    {
        IUsage Process(IConfiguration configuration, IUsage usage);
    }
}
