# Define build properties
$VersionPrefix= "0.1"
$VersionSuffix= ""
$Configuration = "Release"

# Get version from git tag
$git_description = git describe --long --tags --always --dirty
if ($null -ne $git_description) {
    # Parse for tag and commit
    if ($git_description -match '[v](.*)-([0-9]*)-(.*)') {
        # extract version from tag and suffix from commit/extras
        $VersionPrefix = $Matches[1]
        $Commits = $Matches[2]
        $VersionSuffix = $Matches[3]

        # when building source that exactly matches tag, remove suffix
        if (($Commits -eq '0') -and (!$Matches[3].Contains('dirty'))) {
            $VersionSuffix = ''
        }
    } else {
        # version tag not found - just set the suffix to the git description
        $VersionSuffix = $git_description;
    }
} else {
    # Git not found or failed to retrieve git tag
    $VersionSuffix = "no-git";
}

$VersionString = $VersionPrefix
if ($VersionSuffix) {
    $VersionString = "$VersionPrefix-$VersionSuffix"
}

Write-Host "Cleaning..."
dotnet clean --configuration $Configuration

Write-Host "Compiling SDK Library Version: $VersionString"
dotnet build --configuration $Configuration -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix `
        .\lib\DSIO.Modality.Api.Sdk.Types\DSIO.Modality.Api.Sdk.Types.csproj
dotnet build --configuration $Configuration -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix `
        .\lib\DSIO.Modality.Api.Sdk.Client\DSIO.Modality.Api.Sdk.Client.csproj

Write-Host "Compiling SDK Samples Version: $VersionString"
dotnet build --configuration $Configuration -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix `
        .\samples\ConsoleApp\ConsoleApp.csproj
dotnet build --configuration $Configuration -p:VersionPrefix=$VersionPrefix -p:VersionSuffix=$VersionSuffix `
        .\samples\WpfSample\WpfSample.csproj

Write-Host "Finished building SDK with Version $VersionString and Configuration $Configuration"