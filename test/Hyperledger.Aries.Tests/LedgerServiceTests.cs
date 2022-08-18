using System;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Contracts;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.TestHarness;
using Hyperledger.Indy.DidApi;
using Hyperledger.TestHarness;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Hyperledger.Aries.Tests
{
    public abstract class LedgerServiceTests
    {
        protected TestSingleWallet _fixture;

        [Fact(DisplayName = "Can lookup revocation registry delta")]
        public async Task CanLookupRevocRegistryDelta()
        {
            var context = await _fixture.Host.Services.GetService<IAgentProvider>().GetContextAsync();
            var schemaService = _fixture.Host.Services.GetService<ISchemaService>();
            var (credDefId, schemaId) = await Scenarios.CreateDummySchemaAndNonRevokableCredDef(context, schemaService, TestConstants.StewardDid, new[] {"name"});
            

            var data = $"{{\"ver\":\"1.0\",\"id\":\"Th7MpTaRZVRYnPiabds81Y:4:{credDefId}:CL_ACCUM:1-1024\",\"revocDefType\":\"CL_ACCUM\",\"tag\":\"1-1024\",\"credDefId\":\"{credDefId}\",\"value\":{{\"issuanceType\":\"ISSUANCE_BY_DEFAULT\",\"maxCredNum\":1024,\"publicKeys\":{{\"accumKey\":{{\"z\":\"1 1BA41CF61365C07D280F6C237E23D0A22327F2C2C30933E683F23F9A1961004D 1 0C49B7FE9A87DEDE72C9CBE8D36FC1C830A529BE9EA4CAC4FE607DEB33EF0CA6 1 1DBF57E6F0F05F93920F9CF999BCF89D6B66B9F0996772DBCB7859ADE6522BDF 1 215E94E6B68356A923C1CDE5D2D1EB571EE9F70A21F564C1428610E7AE5EACFA 1 06A281F280FFF4CCA8F97134110B4F2B9CC763871FBCAC4FC05444A3F7BF5D85 1 23C6358F3F4DF46FB3DD7082F526B9A5612B3E3EB35DD2B23EECB2E373142D1C 1 1F936F7854A804E4778F2700B6202A693A2EF3DAD89022ACAF44BD951463AA70 1 194E594CD8CD5EBC7520F7C4CE1539E0A40EDEFE52C644FDC19B26ACC22EFC00 1 16C73F99F212B0BF7DA2B2B5DBD95C69D14EB037E0E11996A3BED781FCE690F0 1 027DBAD128E0E738A710A77AB758565ED1EF93D3D6B37B48938F1ADA041C948D 1 0F548E8438C4838EA980E00395777FFD2D050A25825C4E3D6072193D8FE9BF4D 1 1EF41A717F34CC42539428B73BB88FB9EC4905DE14664365F5AC42EF035C36A0\"}}}},\"tailsHash\":\"E87MyHoLULzRAFyrEYKJRTyXmoS7hW3UnfsqRkaZJvNK\",\"tailsLocation\":\"http://test/tails/E87MyHoLULzRAFyrEYKJRTyXmoS7hW3UnfsqRkaZJvNK\"}}}}";

            await _fixture.Host.Services.GetService<ILedgerService>().RegisterRevocationRegistryDefinitionAsync(context, TestConstants.StewardDid, data);
            
            var value =
                "{\"ver\":\"1.0\",\"value\":{\"accum\":\"21 12143E26AF91226138E1579710AC40731C86489E11C895DA0A9C106BFA42AE3AB 21 133E2DE26A692ED56C033AC4FA2E266373F6A5583C931BF83A8B8D796631C0615 6 6A25ACC0FB32E78A3212AA8785DEB33EFEFE61E45C4B065F23206B82F9271279 4 264965EC18D503515C75F944D5AE87A1F5F891325C56B22F322DD989421B7910 6 6B3F8107C004E34C1BEE4D5C668F989FC3801BB91F9AF0B43221C30CE142C6E3 4 22BC3D18E6E1D895852A3EA4BAF0EEB05BEB9EC5DF7C28A6E0B3B573C33DF66C\"}}";

            await _fixture.Host.Services.GetService<ILedgerService>().SendRevocationRegistryEntryAsync(context, TestConstants.StewardDid, $"Th7MpTaRZVRYnPiabds81Y:4:{credDefId}:CL_ACCUM:1-1024", "CL_ACCUM",  value);
            
            var result = await _fixture.Host.Services.GetService<ILedgerService>().LookupRevocationRegistryDeltaAsync(context, $"Th7MpTaRZVRYnPiabds81Y:4:{credDefId}:CL_ACCUM:1-1024", 0, ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
            
            Assert.True(result != null);
        }
        
        [Fact(DisplayName = "Set Nym on ledger")]
        public async Task SetNymOnLedger()
        {
            var context = await _fixture.Host.Services.GetService<IAgentProvider>().GetContextAsync();

            var did = await Did.CreateAndStoreMyDidAsync(context.Wallet, "{}");

            await _fixture.Host.Services.GetService<ILedgerService>()
                .RegisterNymAsync(context, TestConstants.StewardDid, did.Did, did.VerKey, null);

            var result = await _fixture.Host.Services.GetService<ILedgerService>().LookupNymAsync(context, did.Did);
            var data = JObject.Parse(result)["result"]?["data"]?.ToString();
            
            Assert.Equal(did.Did, JObject.Parse(data!)["dest"]?.ToString());
        }
        
        [Fact(DisplayName = "Set Attribute on ledger")]
        public async Task SetAttributeOnLedger()
        {
            var expected = Guid.NewGuid().ToString();
            var context = await _fixture.Host.Services.GetService<IAgentProvider>().GetContextAsync();

            await _fixture.Host.Services.GetService<ILedgerService>().RegisterAttributeAsync(context, TestConstants.StewardDid, TestConstants.StewardDid, "name", expected);
            
            var actual = await _fixture.Host.Services.GetService<ILedgerService>().LookupAttributeAsync(context, TestConstants.StewardDid, "name");
            
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Set credential definition on ledger")]
        public async Task SetCredentialDefinitionOnLedger()
        {
            var context = await _fixture.Host.Services.GetService<IAgentProvider>().GetContextAsync();
            
            string name = "test-schema-" + Guid.NewGuid();
            
            string schema = string.Format("{{\"id\": \"Th7MpTaRZVRYnPiabds81Y:2:{0}:1.0\",\"name\": \"{0}\",\"version\": \"1.0\",\"ver\": \"1.0\",\"attrNames\": [\"name\"]}}", name) ;

            await _fixture.Host.Services.GetService<ILedgerService>().RegisterSchemaAsync(context, TestConstants.StewardDid, schema);

            var schemaResult = await _fixture.Host.Services.GetService<ILedgerService>().LookupSchemaAsync(context, JObject.Parse(schema)["id"]!.ToString());

            var seqNo = JObject.Parse(schemaResult.ObjectJson)["seqNo"]!.ToString();

            var credDef = $"{{\"ver\":\"1.0\",\"id\":\"Th7MpTaRZVRYnPiabds81Y:3:CL:{seqNo}:tag\",\"schemaId\":\"{seqNo}\",\"type\":\"CL\",\"tag\":\"tag\",\"value\":{{\"primary\":{{\"n\":\"103701508780726168143297499632560778642769106805376886979566430343909908479374304507793337079829104322462588516770753688111835879661873250870606990093909717625230762083558078700783821845871189525737072950244647208040583876691909824800070251268229300811757831897783003753154617808637307139771112583716989121492942356725389770671284653011802629749599153592797167144614009090524325451744273210839832585564437362835932431389572411714266015557859351960547294381261026253631968594995713030986543994957800871720480231361479580689427898901118357885337319817481823896444884485182803638597498516257488507382256030528939019960817\",\"s\":\"78404460608466140444896379330602899797771575402421088740866173474473075631034014489028116623103801434312749772834091097022863328870600813887634715447549387983425454797058149222044030750458385598644898568369092428672098763984577155413292208485482523286952404579829582378923246547432957463896789990439339943913797505120553682853156175642081354941127147680760709120949574973982667361458001929814334732428304176487607998037586425752771459664312309634785248247656825890387164638558555137076561830659421660909031032482994059251097905975494233833243480732689417875439737292604950549785158253575008621755713086562604661020474\",\"r\":{{\"master_secret\":\"64714051517540030980749087040493032612559392266201179284587874150178201294391427711197203049105355595169798092022641050180651078149835784981920927155418143820790230121526479097719506208431853616124682295321336752723255694102239284320069711981977149750542848508222887445130332057682548538360029568512219975777907661408351359750409702099066759274172694145723689034388349985171992139859788813324093629918587569560377008720707851241418132781314267589967238425851586715364073559650373811036795696357708645454560959705782749895346404827442813397434790548635526220987139351864777626634885355235153897173628702016397014326364\",\"name\":\"14580026769022124124692634645659861438830538797975657510579557711910309598950819630610873415634669835740466010859907866358934900728636362960576832223554133161209738824473711534462343774689745605016926090872407436377079332367156073486603586487820299162411267105457522185555292312129610089430166795242684004191210604460299808283125705985334948351055690471023824796391303812453354869693849554692266852341281765102706658912051082545794413426907895331827109195142132601140857640426329343775580507004533085166992807720429768159515891954770270614936762333552110250710518321058492705591023748116882751286387222045306365077292\"}},\"rctxt\":\"83817667798395259154389000038309403037824926429060326629779092234175118573829407461290325150471320744807128905500578219593978525573566674730513499471624822587966109733458795040197388744120775870897575365001742227326876678540967297335671840938123810056567249733959808495779521699304095356166980918454401664818996532262656302620744148068940775882048159701084168230691952418321630894519300568239940940575731748795907228456289573938328033810230338387559741810364527069611668166655488598548286923245960191208337204711850906676365865604089539092577802808569173207642057953941816051007462302436538947444421934930943000321395\",\"z\":\"66982472411032709914932262036715256528594860869147343878072345035822547761741039608124487969732478110045418806063525005595216711771859023795223375027797609074382960844670887759067931425247647232816881405152298272104060656527137779606157888377107379911666081239917923944811171681670019649292521487239348191569031448282067847757836762729305157087567714974399535401926169781897085147267925395660718958893385849137857412477516810073471498386253054080940874354216584179209625220635425132030578992183971733473884166996789070710113624197992521762368494899401749499325731221925669681066703588781714250909003191858391363623555\"}}}}}}";

            await _fixture.Host.Services.GetService<ILedgerService>().RegisterCredentialDefinitionAsync(context, TestConstants.StewardDid, credDef);

            var result = await _fixture.Host.Services.GetService<ILedgerService>().LookupDefinitionAsync(context, $"Th7MpTaRZVRYnPiabds81Y:3:CL:{seqNo}:tag");
            
            Assert.Equal(JObject.Parse(schemaResult.ObjectJson)["seqNo"],  JObject.Parse(result.ObjectJson)["schemaId"]!.ToString());
        }
        
        [Fact(DisplayName = "Set revocation registry definition on ledger")]
        public async Task SetRevocationRegistryDefinitionOnLedger()
        {
            var context = await _fixture.Host.Services.GetService<IAgentProvider>().GetContextAsync();
            var schemaService = _fixture.Host.Services.GetService<ISchemaService>();
            var (credDefId, schemaId) = await Scenarios.CreateDummySchemaAndNonRevokableCredDef(context, schemaService, TestConstants.StewardDid, new[] {"name"});
            

            var data = $"{{\"ver\":\"1.0\",\"id\":\"Th7MpTaRZVRYnPiabds81Y:4:{credDefId}:CL_ACCUM:1-1024\",\"revocDefType\":\"CL_ACCUM\",\"tag\":\"1-1024\",\"credDefId\":\"{credDefId}\",\"value\":{{\"issuanceType\":\"ISSUANCE_BY_DEFAULT\",\"maxCredNum\":1024,\"publicKeys\":{{\"accumKey\":{{\"z\":\"1 1BA41CF61365C07D280F6C237E23D0A22327F2C2C30933E683F23F9A1961004D 1 0C49B7FE9A87DEDE72C9CBE8D36FC1C830A529BE9EA4CAC4FE607DEB33EF0CA6 1 1DBF57E6F0F05F93920F9CF999BCF89D6B66B9F0996772DBCB7859ADE6522BDF 1 215E94E6B68356A923C1CDE5D2D1EB571EE9F70A21F564C1428610E7AE5EACFA 1 06A281F280FFF4CCA8F97134110B4F2B9CC763871FBCAC4FC05444A3F7BF5D85 1 23C6358F3F4DF46FB3DD7082F526B9A5612B3E3EB35DD2B23EECB2E373142D1C 1 1F936F7854A804E4778F2700B6202A693A2EF3DAD89022ACAF44BD951463AA70 1 194E594CD8CD5EBC7520F7C4CE1539E0A40EDEFE52C644FDC19B26ACC22EFC00 1 16C73F99F212B0BF7DA2B2B5DBD95C69D14EB037E0E11996A3BED781FCE690F0 1 027DBAD128E0E738A710A77AB758565ED1EF93D3D6B37B48938F1ADA041C948D 1 0F548E8438C4838EA980E00395777FFD2D050A25825C4E3D6072193D8FE9BF4D 1 1EF41A717F34CC42539428B73BB88FB9EC4905DE14664365F5AC42EF035C36A0\"}}}},\"tailsHash\":\"E87MyHoLULzRAFyrEYKJRTyXmoS7hW3UnfsqRkaZJvNK\",\"tailsLocation\":\"http://test/tails/E87MyHoLULzRAFyrEYKJRTyXmoS7hW3UnfsqRkaZJvNK\"}}}}";

            await _fixture.Host.Services.GetService<ILedgerService>().RegisterRevocationRegistryDefinitionAsync(context, TestConstants.StewardDid, data);

            var result = await _fixture.Host.Services.GetService<ILedgerService>().LookupRevocationRegistryDefinitionAsync(context, $"{TestConstants.StewardDid}:4:{credDefId}:CL_ACCUM:1-1024");
            
            Assert.Equal(JObject.Parse(data)["value"]!["tailsHash"]!.ToString(), JObject.Parse(result.ObjectJson)["value"]!["tailsHash"]!.ToString());
        }
        
        [Fact(DisplayName = "Set revocation registry entry on ledger")]
        public async Task SetRevocationRegistryEntryOnLedger()
        {
            var context = await _fixture.Host.Services.GetService<IAgentProvider>().GetContextAsync();
            var schemaService = _fixture.Host.Services.GetService<ISchemaService>();
            var (credDefId, schemaId) = await Scenarios.CreateDummySchemaAndNonRevokableCredDef(context, schemaService, TestConstants.StewardDid, new[] {"name"});
            

            var data = $"{{\"ver\":\"1.0\",\"id\":\"Th7MpTaRZVRYnPiabds81Y:4:{credDefId}:CL_ACCUM:1-1024\",\"revocDefType\":\"CL_ACCUM\",\"tag\":\"1-1024\",\"credDefId\":\"{credDefId}\",\"value\":{{\"issuanceType\":\"ISSUANCE_BY_DEFAULT\",\"maxCredNum\":1024,\"publicKeys\":{{\"accumKey\":{{\"z\":\"1 1BA41CF61365C07D280F6C237E23D0A22327F2C2C30933E683F23F9A1961004D 1 0C49B7FE9A87DEDE72C9CBE8D36FC1C830A529BE9EA4CAC4FE607DEB33EF0CA6 1 1DBF57E6F0F05F93920F9CF999BCF89D6B66B9F0996772DBCB7859ADE6522BDF 1 215E94E6B68356A923C1CDE5D2D1EB571EE9F70A21F564C1428610E7AE5EACFA 1 06A281F280FFF4CCA8F97134110B4F2B9CC763871FBCAC4FC05444A3F7BF5D85 1 23C6358F3F4DF46FB3DD7082F526B9A5612B3E3EB35DD2B23EECB2E373142D1C 1 1F936F7854A804E4778F2700B6202A693A2EF3DAD89022ACAF44BD951463AA70 1 194E594CD8CD5EBC7520F7C4CE1539E0A40EDEFE52C644FDC19B26ACC22EFC00 1 16C73F99F212B0BF7DA2B2B5DBD95C69D14EB037E0E11996A3BED781FCE690F0 1 027DBAD128E0E738A710A77AB758565ED1EF93D3D6B37B48938F1ADA041C948D 1 0F548E8438C4838EA980E00395777FFD2D050A25825C4E3D6072193D8FE9BF4D 1 1EF41A717F34CC42539428B73BB88FB9EC4905DE14664365F5AC42EF035C36A0\"}}}},\"tailsHash\":\"E87MyHoLULzRAFyrEYKJRTyXmoS7hW3UnfsqRkaZJvNK\",\"tailsLocation\":\"http://test/tails/E87MyHoLULzRAFyrEYKJRTyXmoS7hW3UnfsqRkaZJvNK\"}}}}";

            await _fixture.Host.Services.GetService<ILedgerService>().RegisterRevocationRegistryDefinitionAsync(context, TestConstants.StewardDid, data);
            
            var value =
                "{\"ver\":\"1.0\",\"value\":{\"accum\":\"21 12143E26AF91226138E1579710AC40731C86489E11C895DA0A9C106BFA42AE3AB 21 133E2DE26A692ED56C033AC4FA2E266373F6A5583C931BF83A8B8D796631C0615 6 6A25ACC0FB32E78A3212AA8785DEB33EFEFE61E45C4B065F23206B82F9271279 4 264965EC18D503515C75F944D5AE87A1F5F891325C56B22F322DD989421B7910 6 6B3F8107C004E34C1BEE4D5C668F989FC3801BB91F9AF0B43221C30CE142C6E3 4 22BC3D18E6E1D895852A3EA4BAF0EEB05BEB9EC5DF7C28A6E0B3B573C33DF66C\"}}";

            await _fixture.Host.Services.GetService<ILedgerService>().SendRevocationRegistryEntryAsync(context, TestConstants.StewardDid, $"Th7MpTaRZVRYnPiabds81Y:4:{credDefId}:CL_ACCUM:1-1024", "CL_ACCUM",  value);

            var result = await _fixture.Host.Services.GetService<ILedgerService>().LookupRevocationRegistryAsync(context, $"Th7MpTaRZVRYnPiabds81Y:4:{credDefId}:CL_ACCUM:1-1024", ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
            
            Assert.Equal(JObject.Parse(value)["value"]!["accum"]!.ToString(), JObject.Parse(result.ObjectJson)["value"]!["accum"]!.ToString());
        }
        
        [Fact(DisplayName = "Set schema on ledger")]
        public async Task SetSchemaOnLedger()
        {
            var context = await _fixture.Host.Services.GetService<IAgentProvider>().GetContextAsync();

            string name = "test-schema-" + Guid.NewGuid();
            
            string schema = string.Format("{{\"id\": \"Th7MpTaRZVRYnPiabds81Y:2:{0}:1.0\",\"name\": \"{0}\",\"version\": \"1.0\",\"ver\": \"1.0\",\"attrNames\": [\"name\"]}}", name) ;

            await _fixture.Host.Services.GetService<ILedgerService>().RegisterSchemaAsync(context, TestConstants.StewardDid, schema);

            var result = await _fixture.Host.Services.GetService<ILedgerService>().LookupSchemaAsync(context, $"Th7MpTaRZVRYnPiabds81Y:2:{name}:1.0");
             
             Assert.Equal(name, JObject.Parse(result.ObjectJson)["name"]?.ToString());
        }
        
        [Fact(DisplayName = "Set service endpoint on ledger")]
        public async Task SetServiceEndpointOnLedger()
        {
            var context = await _fixture.Host.Services.GetService<IAgentProvider>().GetContextAsync();
            var endpoint = "https://example.com/" + Guid.NewGuid().ToString();
            
            await _fixture.Host.Services.GetService<ILedgerService>().RegisterServiceEndpointAsync(context, TestConstants.StewardDid, endpoint);
            var result = await _fixture.Host.Services.GetService<ILedgerService>().LookupServiceEndpointAsync(context, TestConstants.StewardDid);
            
            Assert.Equal(endpoint, result.Result.Endpoint);
        }
        
        public class LedgerServiceTestsV1 : LedgerServiceTests, IClassFixture<LedgerServiceTestsV1.SingleTestWalletFixture>
        {
            public class SingleTestWalletFixture : TestSingleWallet
            {
                protected override string GetIssuerSeed() => TestConstants.StewardSeed;
            }
        
            public LedgerServiceTestsV1(SingleTestWalletFixture fixture)
            {
                _fixture = fixture;
            }
        }

        public class LedgerServiceTestsV2 : LedgerServiceTests, IClassFixture<LedgerServiceTestsV2.SingleTestWalletV2Fixture>
        {
            public class SingleTestWalletV2Fixture : TestSingleWalletV2
            {
                protected override string GetIssuerSeed() => TestConstants.StewardSeed;
            }
        
            public LedgerServiceTestsV2(SingleTestWalletV2Fixture fixture)
            {
                _fixture = fixture;
            }
        }
    }
}
