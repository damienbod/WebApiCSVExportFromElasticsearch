using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web.Http;
using ElasticsearchCRUD;
using ElasticsearchCRUD.ContextSearch;
using ElasticsearchCRUD.Tracing;
using WebApiCSVExportFromElasticsearch.Models;

namespace WebApiCSVExportFromElasticsearch.Controllers
{
    public class PersonsCsvExportController : ApiController
    {
		[Route("api/PersonsCsvExport")]
		public IHttpActionResult GetPersonsCsvExport()
		{	
			// force that this method always returns an excel document.
			Request.Headers.Accept.Clear();
			Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.ms-excel"));

			var result = new List<Person>(); 
			using (var context = new ElasticsearchContext("http://localhost:9200/", new ElasticsearchMappingResolver()))
			{
				context.TraceProvider = new ConsoleTraceProvider();

				var scanScrollConfig = new ScanAndScrollConfiguration(1, TimeUnits.Second, 300);
				var scrollIdResult = context.SearchCreateScanAndScroll<Person>(BuildSearchMatchAll(), scanScrollConfig);

				var scrollId = scrollIdResult.ScrollId;

				int processedResults = 0;
				while (scrollIdResult.TotalHits > processedResults)
				{
					var resultCollection = context.Search<Person>("", scrollId, scanScrollConfig);
					scrollId = resultCollection.ScrollId;

					result.AddRange(resultCollection.PayloadResult);
					processedResults = result.Count;
				}
			}

			return Ok(result);
		}

			//{
		//	"query" : {
		//		"match_all" : {}
		//	}
		//}
		private string BuildSearchMatchAll()
		{
			var buildJson = new StringBuilder();
			buildJson.AppendLine("{");
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"match_all\" : {}");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");

			return buildJson.ToString();
		}
    }
}
