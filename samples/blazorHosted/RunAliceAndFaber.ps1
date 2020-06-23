Start-Process -FilePath 'dotnet' -ArgumentList 'run --project .\Source\Server\ --launch-profile "BlazorHosted.Server Alice"'
Start-Process -FilePath 'dotnet' -ArgumentList 'run --project .\Source\Server\ --launch-profile "BlazorHosted.Server Faber"'
