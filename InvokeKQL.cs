using System.Management.Automation;
using System.Linq;
using Kusto.Data.Net.Client;
using Kusto.Data.Common;

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

        private ICslQueryProvider client;
        private ClientRequestProperties requestProperties;

        protected override void BeginProcessing()
        {
            WriteVerbose($"Creating database connection {Database}...");
            Kusto.Data.KustoConnectionStringBuilder kcsb = new Kusto.Data.KustoConnectionStringBuilder(Database) {
                //FederatedSecurity = true,
                ApplicationClientId = "683e0216-8599-48d9-90ee-ca7e54dff2df",
            };
            client = KustoClientFactory.CreateCslQueryProvider(kcsb);
            requestProperties = new ClientRequestProperties() {
                ClientRequestId = "Invoke-KQL;ActivityId=" + System.Guid.NewGuid().ToString(),
            };
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            foreach (string Q1 in Query) {
                WriteVerbose($"Executing query '{Q1}' ...");
                var reader = client.ExecuteQuery(Q1, requestProperties);
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
