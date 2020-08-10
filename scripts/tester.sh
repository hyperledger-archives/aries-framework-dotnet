#! /bin/sh

# Will run any tests that have the Keyword in their name. 
# Example: DisplayName~Aries will run all Tests
# Example: DisplayName~ProofTests will only run ProofTests

(cd test/Hyperledger.Aries.Tests && \ 
dotnet test --nologo --filter DisplayName~Aries --verbosity normal)