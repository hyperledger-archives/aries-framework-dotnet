using System.Collections.Generic;
using Hyperledger.Aries.Models.Records;
using Hyperledger.Aries.Features.DidExchange;

namespace WebAgent.Models
{
    public class CredentialFormModel
    {
        public List<DefinitionRecord> CredentialDefinitions { get; set; }
        public List<SchemaRecord> Schemas { get; set; }
        public List<ConnectionRecord> Connections { get; set; }

        public static string DefaultAttributes = 
@"[
    { 'name': 'name', 'value': 'Alice Smith' },
    { 'name': 'date', 'value': '2020-01-01' },
    { 'name': 'degree', 'value': 'Maths' },
    { 'name': 'age', 'value': '24' }
]";
    }
}