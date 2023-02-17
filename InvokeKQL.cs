using System.Management.Automation;
using System.Linq;
using System.Collections.Generic;

namespace KQL
{
    [Cmdlet(VerbsLifecycle.Invoke,"KQL")]
    public class InvokeKQL : PSCmdlet
    {
        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string[] Query { get; set; } = { "StormEvents | count" };
        // As per first example at https://learn.microsoft.com/en-us/azure/data-explorer/kusto/query/tutorial?pivots=azuredataexplorer

        [Parameter(
            Mandatory = false,
            Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Database { get; set; } = "https://help.kusto.windows.net/Samples";

        private Kusto.Data.Common.ICslQueryProvider client;
        private Kusto.Data.Common.ClientRequestProperties clientRP;

        protected override void BeginProcessing()
        {
            WriteVerbose($"Connecting database {Database}...");
            client = Kusto.Data.Net.Client.KustoClientFactory.CreateCslQueryProvider($"{Database};Fed=true;");
            clientRP = new Kusto.Data.Common.ClientRequestProperties() {
                ClientRequestId = "Invoke-KQL;ActivityId=" + System.Guid.NewGuid().ToString(),
                AuthorizationScheme = null
            };
            // clientRP.ClientRequestId = "Invoke-KQL;ActivityId=" + System.Guid.NewGuid().ToString();
            // clientRP.PrincipalIdentity = null;
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            foreach (string Q1 in Query) {
                WriteVerbose($"Executing query '{Q1}' ...");
                var reader = client.ExecuteQuery(Q1, clientRP);
                WriteVerbose("Parsing query results...");
                while (reader.Read()) {
                    PSObject rowData = new PSObject();
                    Enumerable.Range(0, reader.FieldCount)
                        .Select(i => new PSNoteProperty(reader.GetName(i), reader.GetValue(i)))
                        .ToList()
                        .ForEach(rowData.Properties.Add);
                    WriteObject(rowData);
                };
                WriteVerbose("Query complete!");
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            client.Dispose();
            WriteVerbose("End!");
        }
    }
}
