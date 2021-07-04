param (
	[Parameter(Mandatory)]
	$SolutionPath,
	[Parameter(Mandatory)]
	$PublishFolder,
	[Parameter(Mandatory)]
	$PackageName
)
$version = ${env:GITHUB_REF} -replace 'refs/\w+/w+', ''
Write-Output "Restoring dependencies"
dotnet restore "$SolutionPath"

Write-Output "Publishing app"
dotnet publish "$SolutionPath" --output "$PublishFolder" --configuration Release /p:version="$version"

#Write-Output "Zipping files"
#cd "$PublishFolder" && tar.exe -a -c -f "$PackageName" *.* Template/*.* -C "$PublishFolder" && cd ..

