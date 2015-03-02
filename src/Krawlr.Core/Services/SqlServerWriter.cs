namespace Krawlr.Core.Services
{
    using System;
    using System.IO;
    using Krawlr.Core.Extensions;
    using MZMemoize.Extensions;
    using System.Data.SqlClient;
    using MZMemoize;
    using Dapper;
    using System.Linq;

    public class SqlServerWriter : IWriterService
    {
        protected IConfiguration _configuration;
        protected ILog _log;
        protected StreamWriter _writer;

        static Func<string, string, string, int> CrawlRunId = new Func<string, string, string, int>((connString, domain, metadata) =>
        {
            int id;
            string sql = @"INSERT INTO CrawlRun (Created, Domain, Metadata) VALUES (@created, @domain, @metadata);
                           SELECT CAST(SCOPE_IDENTITY() as int)";
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                id = conn.Query<int>(sql, new { Created = DateTime.Now, Domain = domain, Metadata = metadata }).Single();
            }
            return id;
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
                int id = CrawlRunId(_configuration.Output, response.Domain, _configuration.Metadata);
                string sql = @"INSERT INTO CrawlResults 
                        (CrawlRunId, Domain, Url, Created, Code, HasJavascriptErrors, TimeTakenMs, JavascriptErrors) 
                        VALUES 
                        (@crawlRunId, @domain, @url, @created, @code, @hasJavascriptErrors, @timeTakenMs, @javascriptErrors)";
                conn.Open();
                conn.Execute(sql, new
                {
                    crawlRunId = id,
                    domain = response.Domain,
                    url = response.RelativeUrl,
                    created = response.Created,
                    code = response.Code,
                    hasJavascriptErrors = response.HasJavscriptErrors,
                    timeTakenMs = response.TimeTakenMs,
                    javascriptErrors = response.LoggableJavascriptErrors,
                });
            }
        }
    }
}
