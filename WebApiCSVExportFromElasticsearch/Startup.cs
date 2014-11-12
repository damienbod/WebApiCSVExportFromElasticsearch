using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebApiCSVExportFromElasticsearch.Startup))]

namespace WebApiCSVExportFromElasticsearch
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.MapSignalR();
		}
	}

	public class DiagnosisEventSourceService : Hub
	{
	}
}
