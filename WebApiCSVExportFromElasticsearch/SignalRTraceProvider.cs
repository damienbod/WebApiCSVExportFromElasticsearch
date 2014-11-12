using System;
using System.Diagnostics;
using System.Text;
using ElasticsearchCRUD.Tracing;
using Microsoft.AspNet.SignalR;

namespace WebApiCSVExportFromElasticsearch
{
	public class SignalRTraceProvider : ITraceProvider
	{
		private readonly TraceEventType _traceEventTypelogLevel;
		private readonly IHubContext _hubContext;

		public SignalRTraceProvider(IHubContext hubContext, TraceEventType traceEventTypelogLevel)
		{
			_traceEventTypelogLevel = traceEventTypelogLevel;
			_hubContext = hubContext;
		}

		public void Trace(TraceEventType level, string message, params object[] args)
		{
			if (_traceEventTypelogLevel >= level)
			{
				var sb = new StringBuilder();
				sb.AppendLine();
				sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": ");
				sb.Append(string.Format(message, args));

				_hubContext.Clients.All.addDiagnosisMessage(string.Format("{0}: {1}", level, sb.ToString()));
			}
		}

		public void Trace(TraceEventType level, Exception ex, string message, params object[] args)
		{
			if (_traceEventTypelogLevel >= level)
			{
				var sb = new StringBuilder();
				sb.AppendLine();
				sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": ");
				sb.Append(string.Format(message, args));
				sb.AppendFormat("{2}: {0} , {1}", ex.Message, ex.StackTrace, ex.GetType());
				_hubContext.Clients.All.addDiagnosisMessage(string.Format("{0}: {1}", level, sb.ToString()));
			}
		}	
	}
}