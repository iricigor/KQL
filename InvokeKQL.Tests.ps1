BeforeAll {
    # Import module
    Import-Module -Name ./bin/Debug/KQL.dll -Force
}

Describe "Fake test" -Tags "Fake" {
    It "should start testing" {
        $true | Should -BeTrue
    }
}

Describe "Module tests" -Tags "Module" {
    It "has module imported" {
        Get-Module KQL | Should -Not -BeNullOrEmpty
    }
    It "had command imported" {
        Get-Command Invoke-KQL | Should -Not -BeNullOrEmpty
    }

    It "can display help for command" {
        Get-Help Invoke-KQL | Should -Not -BeNullOrEmpty
    }
}

Describe 'Default command tests' -Tags "Default" {

    # Default values for parameters are set in InvokeKQL.ps1
    # Query = "StormEvents | count"
    # Database = "https://help.kusto.windows.net/Samples"

    BeforeAll {
        $result = Invoke-KQL
    }

    It 'runs command without parameters' {
        $result | Should -Not -BeNullOrEmpty
    }

    It 'returns one row' {
        $result | Should -HaveCount 1
    }

    It 'has property "Count"' {
        $result.Count | Should -Not -BeNullOrEmpty
    }
}

Describe 'README tests' -Tags "Readme" {

    It 'accepts Verbose flag' {
        Invoke-KQL 'StormEvents | project StartTime, State, EventNarrative | take 5' -Verbose | Should -HaveCount 5
    }

    It 'runs inside pipeline' {
        'StormEvents', 'Covid19_Bing' | % {Invoke-KQL "$_ | count"} | Should -HaveCount 2
    }

    It 'runs multiple queries' {
        'StormEvents', 'Covid19_Bing' | % {"$_ | count"} | Invoke-KQL | Should -HaveCount 2
    }

    It 'runs Kusto commands' {
        Invoke-KQL '.show tables' | Should -Not -BeNullOrEmpty
    }

    It 'connects to alternate database' {
        Invoke-KQL 'StormEvents | count' -Database 'https://help.kusto.windows.net/Samples' | Should -Not -BeNullOrEmpty
    }
}

Describe 'Negative tests' -Tags "Negative" {

    It 'fails on invalid query' {
        {Invoke-KQL 'StormEvents | count | invalid'} | Should -Throw
    }

    It 'fails on invalid table' {
        {Invoke-KQL 'Invalid | count'} | Should -Throw
    }

    It 'fails on invalid database' {
        {Invoke-KQL 'StormEvents | count' -Database 'https://help.kusto.windows.net/Invalid'} | Should -Throw
    }

    It 'fails on invalid cluster' {
        {Invoke-KQL 'StormEvents | count' -Database 'https://help2.kusto.windows.net/Samples'} | Should -Throw
    }
}
