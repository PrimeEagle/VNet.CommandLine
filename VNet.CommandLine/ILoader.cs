namespace VNet.CommandLine
{
	public interface ILoader
    {
        IConfiguration Load(object[] parameters);
    }
}
