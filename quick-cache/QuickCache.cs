using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("quick-cache-test")]
internal class QuickCache()
{
    public string Get()
    {
        return "Hello World";
    }
}