param (
  [string]$table = 'StormEvents'
)

$sample = Invoke-KQL "$table | take 5"
$cols = $sample | gm -MemberType NoteProperty

$cc = 0
$ct = $cols.Count

$cols | % {
    $name = $_.Name
    $type = ($_.Definition -split ' ')[0]
    Write-Progress -Activity "Checking column $name..." -PercentComplete (100*$cc/$ct)
    # $aggr = if ($type -in ('int','double','string')) {$name} else {"tostring($name)"} <== not helping in speed at all
    $q0 = Invoke-KQL "$table | summarize count() by tostring($name) | sort by count_ desc"

    $q1 = $q0 | where $name -eq ''
    $q2 = $q0 | where $name -ne '' | where $name -ne '0' | select -First 1

    [PSCustomObject]@{
        Column = $name
        Type = $type
        Empty = if ($q1) {$q1.count_} else {0}
        Distinct = $q0.Count
        TopValue = $q2.$name
        TopCount = $q2.count_
    }
    $cc++
} | ft
Write-Progress -Completed -Activity 'done'

# TODO:
# add mandatory, not mandatory, allmost empty, empty
# if all unique add primary key
# if >50% unique => custom value, 
# if >50% the same value
# 
