namespace sample.console.Services
{
    public interface IConsoleOutput
    {
        void WriteLine(string message);

        void WriteError(string message);
    }
}