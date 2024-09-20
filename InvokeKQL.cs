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

        protected override void BeginProcessing()
        {
            WriteVerbose($"Connecting database {Database}...");
            client = Kusto.Data.Net.Client.KustoClientFactory.CreateCslQueryProvider($"{Database};Fed=true;");
        }

        // This method will be called for each input received from the pipeline to this cmdlet; if no input is received, this method is not called
        protected override void ProcessRecord()
        {
            foreach (string Q1 in Query)
            {
                WriteVerbose($"Executing query '{Q1}' ...");
                try
                {
                    var reader = client.ExecuteQuery(Q1);
                    WriteVerbose("Parsing query results...");
                    
                    var fieldNames = Enumerable.Range(0, reader.FieldCount)
                        .Select(i => reader.GetName(i))
                        .ToArray();

                    while (reader.Read())
                    {
                        var rowData = new PSObject();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            rowData.Properties.Add(new PSNoteProperty(fieldNames[i], reader.GetValue(i)));
                        }
                        WriteObject(rowData);
                    }

                    WriteVerbose("Query complete!");
                }
                catch (Kusto.Data.Exceptions.KustoException ex)
                {
                    WriteError(new ErrorRecord(ex, "KustoQueryError", ErrorCategory.InvalidOperation, Q1));
                }
                catch (System.Exception ex)
                {
                    WriteError(new ErrorRecord(ex, "QueryExecutionError", ErrorCategory.InvalidOperation, Q1));
                }
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