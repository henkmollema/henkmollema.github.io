using System;
using Microsoft.Extensions.OptionsModel;

namespace ComponentOptions
{
    public interface IConsoleWriter
    {
        void Write();
    }

    public class ConsoleWriter : IConsoleWriter
    {
        private readonly IOptions<ConsoleWriterOptions> _consoleWriterOptions;
        private readonly IOptions<AppSettings> _appSettings;

        public ConsoleWriter(IOptions<ConsoleWriterOptions> consoleWriterOptions, IOptions<AppSettings> appSettings)
        {
            _consoleWriterOptions = consoleWriterOptions;
            _appSettings = appSettings;
        }

        public void Write()
        {
            Console.WriteLine(_consoleWriterOptions.Value.Message);
            Console.WriteLine(_appSettings.Value.Option);
        }
    }
}
