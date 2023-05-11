Write-Host "Publishing module to PSGallery"

# Copy psd1 to module folder
New-Item -Path .\bin\KQL -ItemType Directory -Force | Out-Null
Copy-Item -Path .\bin\Debug\* -Destination .\bin\KQL\ -Force
Copy-Item -Path .\KQL.psd1    -Destination .\bin\KQL\ -Force

# Check environment variable
if ($env:PSGalleryApiKey -eq $null) {
    Write-Host "PSGalleryApiKey environment variable is not set. Exiting..."
    exit
}

# Publish to PSGallery
Publish-Module -Path .\bin\KQL\ -NuGetApiKey $env:PSGalleryApiKey -Verbose -Force
