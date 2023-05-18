using System;

namespace VNet.CommandLine.Attributes
{
    public interface IOptionAttribute : IBaseAttribute
    {
        Type DataType { get; set; }
        bool NeedsValue { get; set; }
        bool AllowDuplicates { get; set; }
        bool AllowNullValue { get; set; }
    }
}
