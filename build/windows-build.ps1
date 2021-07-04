$version = ${env:GITHUB_REF} -replace 'refs/\w+/', ''
Write-Host "$version"