FROM streetcred/dotnet-indy:1.14.2
WORKDIR /app

COPY . .
RUN dotnet restore "test/Hyperledger.Aries.Tests/Hyperledger.Aries.Tests.csproj"

COPY docker/docker_pool_genesis.txn test/Hyperledger.Aries.Tests/pool_genesis.txn

WORKDIR /app/test/Hyperledger.Aries.Tests

ENTRYPOINT ["dotnet", "test", "--verbosity", "normal"]