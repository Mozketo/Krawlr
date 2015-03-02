namespace Krawlr.Core.Services
{
    using System;
    using System.IO;
    using Krawlr.Core.Extensions;
    using MZMemoize.Extensions;

    public interface IWriterService
    {
        void Write(Response response);
    }

    public class CsvWriter : IWriterService
    {
        protected IConfiguration _configuration;
        protected ILog _log;
        protected StreamWriter _writer;

        public CsvWriter(IConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;

            if (_configuration.OutputPath.ExistsEx())
                File.Delete(_configuration.OutputPath);
        }

        public void Write(Response response)
        {
            if (!_configuration.Quiet)
            {
                var color = response.HasJavscriptErrors ? ConsoleColor.Red : ConsoleColor.Gray;
                _log.WriteLine(response.ToString(), color);
            }

            if (!_configuration.OutputPath.HasValue())
                return;

            var csv = response.ToCsv();
            File.WriteAllLines(_configuration.OutputPath, new[] { csv });
        }
    }
}
