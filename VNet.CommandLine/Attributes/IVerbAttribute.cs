namespace VNet.CommandLine.Attributes
{
    public interface IVerbAttribute : IBaseAttribute
    {
        int ExecutionOrder { get; set; }
    }
}