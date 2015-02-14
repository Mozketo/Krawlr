using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Krawlr.Core.Extensions;

namespace Krawlr.Core
{
    public interface IOutputService// : IDisposable
    {
        void Write(Response response);
    }

    public class OutputService : IOutputService, IDisposable
    {
        protected IConfiguration _configuration;
        protected StreamWriter _writer;
        protected CsvWriter _csv;

        public OutputService(IConfiguration configuration)
        {
            _configuration = configuration;

            if (_configuration.OutputPath.HasValue())
            {
                if (File.Exists(_configuration.OutputPath))
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
                Console.WriteLine(response.ToString());

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
