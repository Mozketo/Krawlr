using CsvHelper;
using System;
using System.IO;
using Krawlr.Core.Extensions;
using MZMemoize.Extensions;

namespace Krawlr.Core.Services
{
    public interface IWriterService// : IDisposable
    {
        void Write(Response response);
    }

    public class WriterService : IWriterService, IDisposable
    {
        protected IConfiguration _configuration;
        protected ILog _log;
        protected StreamWriter _writer;
        protected CsvWriter _csv;

        public WriterService(IConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;

            if (_configuration.OutputPath.HasValue())
            {
                if (_configuration.OutputPath.ExistsEx())
                    File.Delete(_configuration.OutputPath);
                _writer = new StreamWriter(_configuration.OutputPath);
                _writer.AutoFlush = false;
                _csv = new CsvWriter(_writer);
                _csv.Configuration.HasHeaderRecord = true;
                var map = _csv.Configuration.AutoMap<Response>();
            }
        }

        public void Write(Response response)
        {
            if (!_configuration.Silent)
            {
                var color = response.HasJavscriptErrors ? ConsoleColor.Red : ConsoleColor.Gray;
                _log.WriteLine(response.ToString(), color);
            }

            if (_csv == null)
                return;

            _csv.WriteRecord<Response>(response);
        }

        public void Dispose()
        {
            _csv.Dispose();
            try
            {
                _writer.Flush();
            }
            catch { }
            _writer.Dispose();
        }
    }
}
