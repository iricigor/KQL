# KQL

PowerShell module that enables running KQL (Kusto Query Language) queries directly from PowerShell


[![latest version](https://img.shields.io/powershellgallery/v/KQL.svg?label=latest+version)](https://www.powershellgallery.com/packages/KQL)
[![downloads](https://img.shields.io/powershellgallery/dt/KQL.svg?label=downloads)](https://www.powershellgallery.com/pagitckages/KQL)
[![platforms](https://img.shields.io/powershellgallery/p/KQL)](https://www.powershellgallery.com/pagitckages/KQL)



## Usage

```ps
Install-Module -Name KQL -Scope CurrentUser
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

## Testing pipeline

[![Build Status](https://dev.azure.com/iiric/PS1/_apis/build/status/iricigor.KQL?branchName=master)](https://dev.azure.com/iiric/PS1/_build/latest?definitionId=50&branchName=master)
