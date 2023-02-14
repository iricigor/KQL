# KQL

PowerShell module that enables running KQL (Kusto Query Language) queries directly from PowerShell

## Usage

```ps
Install-Module KQL
Invoke-KQL 'StormEvents | take 2'
```

## Links
- [What is KQL?](https://learn.microsoft.com/en-us/azure/data-explorer/kusto/query/)
- [Online KQL editor](https://dataexplorer.azure.com/clusters/help/databases/Samples?query=H4sIAAAAAAAAAwsuyS/KdS1LzSsp5uWqUShJzE5VMAIA/JK50hUAAAA=) executed against Help/Samples database

## Known issues
- Due to nuget compatibility issues, the module is running only on Windows PowerShell at the moment