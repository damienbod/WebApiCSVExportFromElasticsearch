using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using ElasticsearchCRUD;
using ElasticsearchCRUD.ContextSearch;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD.Model.Units;
using Microsoft.AspNet.SignalR;
using WebApiCSVExportFromElasticsearch.Models;

namespace WebApiCSVExportFromElasticsearch.Controllers
{
    public class PersonsCsvExportController : ApiController
    {
		private readonly IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<DiagnosisEventSourceService>();

		[Route("api/PersonsCsvExport")]
		public IHttpActionResult GetPersonsCsvExport()
		{
			_hubContext.Clients.All.addDiagnosisMessage(string.Format("Csv export starting"));

			// force that this method always returns an excel document.
			Request.Headers.Accept.Clear();
			Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.ms-excel"));

			_hubContext.Clients.All.addDiagnosisMessage(string.Format("ScanAndScrollConfiguration: 1s, 300 items pro shard"));
			_hubContext.Clients.All.addDiagnosisMessage(string.Format("sending scan and scroll _search"));
			_hubContext.Clients.All.addDiagnosisMessage(BuildSearchMatchAll());

			var result = new List<Person>(); 
			using (var context = new ElasticsearchContext("http://localhost:9200/", new ElasticsearchMappingResolver()))
			{
				context.TraceProvider = new SignalRTraceProvider(_hubContext, TraceEventType.Information);

				var scanScrollConfig = new ScanAndScrollConfiguration(new TimeUnitSecond(1), 300);
				var scrollIdResult = context.SearchCreateScanAndScroll<Person>(BuildSearchMatchAll(), scanScrollConfig);
				
				var scrollId = scrollIdResult.PayloadResult.ScrollId;
				_hubContext.Clients.All.addDiagnosisMessage(string.Format("Total Hits: {0}", scrollIdResult.PayloadResult.Hits.Total));

				int processedResults = 0;
				while (scrollIdResult.PayloadResult.Hits.Total > processedResults)
				{
					var resultCollection = context.SearchScanAndScroll<Person>(scrollId, scanScrollConfig);
					scrollId = resultCollection.PayloadResult.ScrollId;

					result.AddRange(resultCollection.PayloadResult.Hits.HitsResult.Select(t => t.Source));
					processedResults = result.Count;
					_hubContext.Clients.All.addDiagnosisMessage(string.Format("Total Hits: {0}, Processed: {1}", scrollIdResult.PayloadResult.Hits.Total, processedResults));
				}
			}

			_hubContext.Clients.All.addDiagnosisMessage(string.Format("Elasticsearch proccessing finished, starting to serialize csv"));
			return Ok(result);
		}

		//{
		//	"query" : {
		//		"match_all" : {}
		//	}
		//}
		private Search BuildSearchMatchAll()
		{
			return new Search()
			{
				Query = new Query(new MatchAllQuery())
			};

		}
    }
}
