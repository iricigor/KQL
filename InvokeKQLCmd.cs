using System.Management.Automation;

namespace KQL
{
    [Cmdlet(VerbsLifecycle.Invoke,"KQL")]
    public class InvokeKQLCmd : PSCmdlet
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

        protected override void BeginProcessing()
        {
            WriteVerbose("Begin!");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            foreach (string Q1 in Query) {
                WriteVerbose("Process!");
                var client = Kusto.Data.Net.Client.KustoClientFactory.CreateCslQueryProvider($"{Database};Fed=true;");
                var reader = client.ExecuteQuery(Q1);
                WriteObject(reader);
            }
        }

        // This method will be called once at the end of pipeline execution; if no input is received, this method is not called
        protected override void EndProcessing()
        {
            WriteVerbose("End!");
        }
    }
}
