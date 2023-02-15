# KQL

PowerShell module that enables running KQL (Kusto Query Language) queries directly from PowerShell

## Usage

```ps
Install-Module KQL
Invoke-KQL 'StormEvents | take 2'
```

## Links
- [What is KQL?](https://learn.microsoft.com/en-us/azure/data-explorer/kusto/query/)
- [Online KQL editor](https://dataexplorer.azure.com/clusters/help/databases/Samples?query=H4sIAAAAAAAAAwsuyS/KdS1LzSsp5uWqUShJzE5VMAIA/JK50hUAAAA=)
- [Azure Data Explorer documentation](https://learn.microsoft.com/en-us/azure/data-explorer/)

## Known issues
- Due to nuget compatibility issues, the module is running only on Windows PowerShell at the moment

## Examples

```ps
# Use Verbose flag
Invoke-KQL 'StormEvents | project StartTime, State, EventNarrative | take 5' -Verbose

# Use alternate database
Invoke-KQL -Database 'https://help.kusto.windows.net/SampleLogs' -Query 'RawSysLogs | summarize count() by name'

# Use Invoke-KQL inside standard PowerShell pipeline
'StormEvents', 'Covid19_Bing' | % {Invoke-KQL "$_ | count"}

# Use multiple queries in one command
'StormEvents', 'Covid19_Bing' | % {"$_ | count"} | Invoke-KQL

# Run Kusto commands
Invoke-KQL '.show tables'
```