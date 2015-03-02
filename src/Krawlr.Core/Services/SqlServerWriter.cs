namespace Krawlr.Core.Services
{
    using System;
    using System.IO;
    using Krawlr.Core.Extensions;
    using MZMemoize.Extensions;
    using System.Data.SqlClient;
    using MZMemoize;

    public class SqlServerWriter : IWriterService
    {
        protected IConfiguration _configuration;
        protected ILog _log;
        protected StreamWriter _writer;

        static Func<string> CrawlRunId = new Func<string>(() =>
        {
            return Guid.NewGuid().ToString().Substring(0, 6);
        })
        .Memoize(threadSafe: true);

        public SqlServerWriter(IConfiguration configuration, ILog log)
        {
            _configuration = configuration;
            _log = log;
        }

        public void Write(Response response)
        {
            if (!_configuration.Quiet)
            {
                var color = response.HasJavscriptErrors ? ConsoleColor.Red : ConsoleColor.Gray;
                _log.WriteLine(response.ToString(), color);
            }

            if (!_configuration.Output.HasValue())
                return;

            using (SqlConnection conn = new SqlConnection(_configuration.Output))
            {
                conn.Open();
                using (SqlCommand command = conn.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO CrawlResults 
                        (CrawlRunId, Domain, Url, Created, Code, HasJavascriptErrors, TimeTakenMs, JavascriptErrors) 
                        VALUES 
                        (@crawlRunId, @domain, @url, @created, @code, @hasJavascriptErrors, @timeTakenMs, @javascriptErrors)";

                    command.Parameters.AddWithValue("@crawlRunId", CrawlRunId());
                    command.Parameters.AddWithValue("@domain", response.Domain);
                    command.Parameters.AddWithValue("@url", response.Url);
                    command.Parameters.AddWithValue("@created", response.Created);
                    command.Parameters.AddWithValue("@code", response.Code);
                    command.Parameters.AddWithValue("@hasJavascriptErrors", response.HasJavscriptErrors);
                    command.Parameters.AddWithValue("@timeTakenMs", response.TimeTakenMs);
                    command.Parameters.AddWithValue("@javascriptErrors", response.LoggableJavascriptErrors);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
