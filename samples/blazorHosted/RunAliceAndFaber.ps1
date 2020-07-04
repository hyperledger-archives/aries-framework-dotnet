dotnet build
Start-Process -FilePath 'dotnet' -ArgumentList 'run --project .\Source\Server\ --no-build --launch-profile "Hyperledger.Aries.OpenApi.Server Alice"'
Start-Process -FilePath 'dotnet' -ArgumentList 'run --project .\Source\Server\ --no-build --launch-profile "Hyperledger.Aries.OpenApi.Server Faber"'
