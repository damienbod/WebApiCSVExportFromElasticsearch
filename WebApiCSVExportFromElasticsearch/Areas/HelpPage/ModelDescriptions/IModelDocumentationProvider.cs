using System;
using System.Reflection;

namespace WebApiCSVExportFromElasticsearch.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}