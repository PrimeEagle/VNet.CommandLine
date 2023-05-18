namespace VNet.CommandLine.Attributes
{
    public interface IConditionAttribute : IAttribute
    {
        ConditionType ConditionType { get; set; }
        object[] Values { get; set; }
        string MethodToExecute { get; init; }
        string PropertyForResult { get; init; }
    }
}