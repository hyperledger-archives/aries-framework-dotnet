dotnet build
Start-Process -FilePath 'dotnet' -ArgumentList 'run --project .\Source\Server\ --no-build --launch-profile "Hyperledger.Aries.AspNetCore.Server Alice"'
Start-Process -FilePath 'dotnet' -ArgumentList 'run --project .\Source\Server\ --no-build --launch-profile "Hyperledger.Aries.AspNetCore.Server Faber"'
