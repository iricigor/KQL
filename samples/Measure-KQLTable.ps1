function Measure-KQLTable {

[CmdletBinding()]
param (
    [string]$table = 'StormEvents',
    [string]$database = 'https://help.kusto.windows.net/Samples',
    [string[]]$column = @(),
    [switch]$AddSamples
)

if (!$column) {
    Write-Verbose "Getting column names from $table..."
    $sample = Invoke-KQL "$table | take 5" -Database $database -Verbose:$false
    $cols = $sample | gm -MemberType NoteProperty
    $column = $cols.Name
}

foreach ($c1 in $column) {

    if ($Column.count -gt 1) {
        Write-Progress -Activity "Checking columns..." -PercentComplete (100*(++$currentColumnCount)/($column.count)) -Status "Column $c1"
    } else {
        Write-Verbose "Checking column $c1..."
    }
    $stat = Invoke-KQL "$table | summarize count() by tostring($c1) | sort by count_ desc" -Database $database -Verbose:$false
    if (!$totalRows) {
        $totalRows = ($stat | Measure-Object -Property count_ -sum).Sum
        Write-Verbose "Total rows: $totalRows"
    }
    $blank = $stat | where $c1 -eq ''
    $top5 = [int](($stat[0..4] | Measure-Object -Property count_ -Sum).Sum)

    [PSCustomObject]@{
        ColumnName = $c1
        TotalRows = $totalRows
        UniqueValues = $stat.Count
        UniquePercentage = [int](100 * $stat.Count / $totalRows)
        IsPrimaryKey = ($stat.Count -eq $totalRows)
        BlankValues = if ($blank) {$blank.count_} else {0}
        BlankPercentage = if ($blank) {[int](100 * $blank.count_ / $totalRows)} else {0}

        Top1Value = if ($AddSamples) {$stat[0].$c1} else {''}
        Top1Count = $stat[0].count_
        Top1Percentage = [int](100 * $stat[0].count_ / $totalRows)

        Top5Values = if ($AddSamples) {$stat[0..4].$c1 -join ', '} else {''}
        Top5Count = $top5
        Top5Percentage = [int](100 * $top5 / $totalRows)
    }
}

    Write-Progress -Completed -Activity 'done'

}