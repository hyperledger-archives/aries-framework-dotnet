using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Hyperledger.Indy.AnonCredsApi;
using Hyperledger.TestHarness;
using Hyperledger.TestHarness.Mock;
using Hyperledger.Aries.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Diagnostics;

namespace Hyperledger.Aries.Tests.Protocols
{
    public class RevocationTests : TestSingleWallet
    {
        [Fact(DisplayName = "Test Proof Presentation Protocol with revocation")]
        public async Task TestCredentialIssuanceV1()
        {
            var pair = await InProcAgent.CreatePairedAsync(true);

            // Configure agent1 as issuer
            var issuerConfiguration = await pair.Agent1.Provider.GetRequiredService<IProvisioningService>()
                .GetProvisioningAsync(pair.Agent1.Context.Wallet);
            await PromoteTrustAnchor(issuerConfiguration.IssuerDid, issuerConfiguration.IssuerVerkey);

            var schemaId = await pair.Agent1.Provider.GetRequiredService<ISchemaService>()
                .CreateSchemaAsync(
                    context: pair.Agent1.Context,
                    issuerDid: issuerConfiguration.IssuerDid,
                    name: $"test-schema-{Guid.NewGuid()}",
                    version: "1.0",
                    attributeNames: new[] { "name", "age" });

            var definitionWithRevocationId = await pair.Agent1.Provider.GetRequiredService<ISchemaService>()
                .CreateCredentialDefinitionAsync(
                    context: pair.Agent1.Context,
                    new CredentialDefinitionConfiguration
                    {
                        SchemaId = schemaId,
                        EnableRevocation = true,
                        RevocationRegistryBaseUri = "http://localhost",
                        Tag = "revoc"
                    });

            var definitionId = await pair.Agent1.Provider.GetRequiredService<ISchemaService>()
                .CreateCredentialDefinitionAsync(
                    context: pair.Agent1.Context,
                    new CredentialDefinitionConfiguration
                    {
                        SchemaId = schemaId,
                        EnableRevocation = false,
                        RevocationRegistryBaseUri = "http://localhost",
                        Tag = "norevoc"
                    });

            // Send offer for two credentials
            var (offer, record) = await pair.Agent1.Provider.GetRequiredService<ICredentialService>()
                .CreateOfferAsync(pair.Agent1.Context, new OfferConfiguration
                {
                    CredentialDefinitionId = definitionWithRevocationId,
                    IssuerDid = issuerConfiguration.IssuerDid,
                    CredentialAttributeValues = new[]
                    {
                        new CredentialPreviewAttribute("name", "random"),
                        new CredentialPreviewAttribute("age", "22")
                    }
                });
            await pair.Agent1.Provider.GetRequiredService<IMessageService>()
                .SendAsync(pair.Agent1.Context.Wallet, offer, pair.Connection1);

            var issuerCredentialWithRevocationId = record.Id;

            (offer, record) = await pair.Agent1.Provider.GetRequiredService<ICredentialService>()
               .CreateOfferAsync(pair.Agent1.Context, new OfferConfiguration
               {
                   CredentialDefinitionId = definitionId,
                   IssuerDid = issuerConfiguration.IssuerDid,
                   CredentialAttributeValues = new[]
                   {
                        new CredentialPreviewAttribute("name", "random"),
                        new CredentialPreviewAttribute("age", "22")
                   }
               });
            await pair.Agent1.Provider.GetRequiredService<IMessageService>()
                .SendAsync(pair.Agent1.Context.Wallet, offer, pair.Connection1);

            // Find credential for Agent 2 and accept all offers
            var credentials = await pair.Agent2.Provider.GetService<ICredentialService>()
                .ListAsync(pair.Agent2.Context);
            foreach (var credential in credentials.Where(x => x.State == CredentialState.Offered))
            {
                var (request, _) = await pair.Agent2.Provider.GetService<ICredentialService>()
                .CreateRequestAsync(pair.Agent2.Context, credential.Id);
                await pair.Agent2.Provider.GetService<IMessageService>()
                    .SendAsync(pair.Agent2.Context.Wallet, request, pair.Connection2);
            }

            // Issue credential
            credentials = await pair.Agent1.Provider.GetService<ICredentialService>()
                .ListRequestsAsync(pair.Agent1.Context);
            foreach (var credential in credentials)
            {
                var (issue, _) = await pair.Agent1.Provider.GetRequiredService<ICredentialService>()
                .CreateCredentialAsync(pair.Agent1.Context, credential.Id);
                await pair.Agent1.Provider.GetService<IMessageService>()
                    .SendAsync(pair.Agent1.Context.Wallet, issue, pair.Connection1);
            }

            // Assert
            foreach (var credential in await pair.Agent1.Provider.GetService<ICredentialService>()
                .ListAsync(pair.Agent1.Context))
            {
                Assert.Equal(CredentialState.Issued, credential.State);
            }
            foreach (var credential in await pair.Agent2.Provider.GetService<ICredentialService>()
                 .ListAsync(pair.Agent2.Context))
            {
                Assert.Equal(CredentialState.Issued, credential.State);
            }


            // Verification - without revocation
            var (requestPresentationMessage, proofRecordIssuer) = await pair.Agent1.Provider.GetService<IProofService>()
                .CreateRequestAsync(pair.Agent1.Context, new ProofRequest
                {
                    Name = "Test Verification",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        { "id-verification", new ProofAttributeInfo { Names = new [] { "name", "age" } } }
                    }
                });

            var proofRecordHolder = await pair.Agent2.Provider.GetService<IProofService>()
                .ProcessRequestAsync(pair.Agent2.Context, requestPresentationMessage, pair.Connection2);

            var availableCredentials = await pair.Agent2.Provider.GetService<IProofService>()
                .ListCredentialsForProofRequestAsync(pair.Agent2.Context, proofRecordHolder.RequestJson.ToObject<ProofRequest>(), "id-verification");
            
            var (presentationMessage, _) = await pair.Agent2.Provider.GetService<IProofService>()
                .CreatePresentationAsync(pair.Agent2.Context, proofRecordHolder.Id, new RequestedCredentials
                {
                    RequestedAttributes = new Dictionary<string, RequestedAttribute>
                    {
                        { "id-verification", new RequestedAttribute
                            {
                                CredentialId = availableCredentials.First().CredentialInfo.Referent,
                                Revealed = true
                            }
                        }
                    }
                });

            proofRecordIssuer = await pair.Agent1.Provider.GetService<IProofService>()
                .ProcessPresentationAsync(pair.Agent1.Context, presentationMessage);

            var valid = await pair.Agent1.Provider.GetService<IProofService>()
                .VerifyProofAsync(pair.Agent1.Context, proofRecordIssuer.Id);

            Assert.True(valid);

            // Verification - with revocation
            var now = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();

            (requestPresentationMessage, proofRecordIssuer) = await pair.Agent1.Provider.GetService<IProofService>()
                .CreateRequestAsync(pair.Agent1.Context, new ProofRequest
                {
                    Name = "Test Verification",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        { "id-verification", new ProofAttributeInfo { Names = new [] { "name", "age" } } }
                    },
                    NonRevoked = new RevocationInterval
                    {
                        From = 0,
                        To = now
                    }
                });

            proofRecordHolder = await pair.Agent2.Provider.GetService<IProofService>()
                .ProcessRequestAsync(pair.Agent2.Context, requestPresentationMessage, pair.Connection2);
            availableCredentials = await pair.Agent2.Provider.GetService<IProofService>()
                .ListCredentialsForProofRequestAsync(pair.Agent2.Context, proofRecordHolder.RequestJson.ToObject<ProofRequest>(), "id-verification");

            (presentationMessage, _) = await pair.Agent2.Provider.GetService<IProofService>()
                .CreatePresentationAsync(pair.Agent2.Context, proofRecordHolder.Id, new RequestedCredentials
                {
                    RequestedAttributes = new Dictionary<string, RequestedAttribute>
                    {
                        { "id-verification", new RequestedAttribute
                            {
                                CredentialId = availableCredentials.First().CredentialInfo.Referent,
                                Revealed = true
                            }
                        }
                    }
                });

            proofRecordIssuer = await pair.Agent1.Provider.GetService<IProofService>()
                .ProcessPresentationAsync(pair.Agent1.Context, presentationMessage);

            valid = await pair.Agent1.Provider.GetService<IProofService>()
                .VerifyProofAsync(pair.Agent1.Context, proofRecordIssuer.Id);

            Assert.True(valid);

            // Revoke the credential
            await pair.Agent1.Provider.GetService<ICredentialService>()
               .RevokeCredentialAsync(pair.Agent1.Context, issuerCredentialWithRevocationId);

            await Task.Delay(TimeSpan.FromSeconds(5));

            now = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();

            (requestPresentationMessage, proofRecordIssuer) = await pair.Agent1.Provider.GetService<IProofService>()
                .CreateRequestAsync(pair.Agent1.Context, new ProofRequest
                {
                    Name = "Test Verification",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        { "id-verification", new ProofAttributeInfo { Names = new [] { "name", "age" } } }
                    },
                    NonRevoked = new RevocationInterval
                    {
                        From = now - 1,
                        To = now
                    }
                });

            proofRecordHolder = await pair.Agent2.Provider.GetService<IProofService>()
                .ProcessRequestAsync(pair.Agent2.Context, requestPresentationMessage, pair.Connection2);
            availableCredentials = await pair.Agent2.Provider.GetService<IProofService>()
                .ListCredentialsForProofRequestAsync(pair.Agent2.Context, proofRecordHolder.RequestJson.ToObject<ProofRequest>(), "id-verification");

            (presentationMessage, _) = await pair.Agent2.Provider.GetService<IProofService>()
                .CreatePresentationAsync(pair.Agent2.Context, proofRecordHolder.Id, new RequestedCredentials
                {
                    RequestedAttributes = new Dictionary<string, RequestedAttribute>
                    {
                        { "id-verification", new RequestedAttribute
                            {
                                CredentialId = availableCredentials.First().CredentialInfo.Referent,
                                Revealed = true
                            }
                        }
                    }
                });

            proofRecordIssuer = await pair.Agent1.Provider.GetService<IProofService>()
                .ProcessPresentationAsync(pair.Agent1.Context, presentationMessage);

            valid = await pair.Agent1.Provider.GetService<IProofService>()
                .VerifyProofAsync(pair.Agent1.Context, proofRecordIssuer.Id);

            Assert.False(valid);

            // Issue new credential
            {
                (offer, record) = await pair.Agent1.Provider.GetRequiredService<ICredentialService>()
                .CreateOfferAsync(pair.Agent1.Context, new OfferConfiguration
                {
                    CredentialDefinitionId = definitionWithRevocationId,
                    IssuerDid = issuerConfiguration.IssuerDid,
                    CredentialAttributeValues = new[]
                    {
                        new CredentialPreviewAttribute("name", "random"),
                        new CredentialPreviewAttribute("age", "22")
                    }
                });
                await pair.Agent1.Provider.GetRequiredService<IMessageService>()
                    .SendAsync(pair.Agent1.Context.Wallet, offer, pair.Connection1);

                string holderCredentialId = null;
                credentials = await pair.Agent2.Provider.GetService<ICredentialService>()
                .ListAsync(pair.Agent2.Context);

                foreach (var credential in credentials.Where(x => x.State == CredentialState.Offered))
                {
                    var (request, _) = await pair.Agent2.Provider.GetService<ICredentialService>()
                    .CreateRequestAsync(pair.Agent2.Context, credential.Id);
                    holderCredentialId = credential.Id;

                    await pair.Agent2.Provider.GetService<IMessageService>()
                        .SendAsync(pair.Agent2.Context.Wallet, request, pair.Connection2);
                }

                // Issue credential
                credentials = await pair.Agent1.Provider.GetService<ICredentialService>()
                    .ListRequestsAsync(pair.Agent1.Context);
                foreach (var credential in credentials)
                {
                    var (issue, _) = await pair.Agent1.Provider.GetRequiredService<ICredentialService>()
                    .CreateCredentialAsync(pair.Agent1.Context, credential.Id);
                    await pair.Agent1.Provider.GetService<IMessageService>()
                        .SendAsync(pair.Agent1.Context.Wallet, issue, pair.Connection1);
                }

                now = (uint)DateTimeOffset.Now.ToUnixTimeSeconds();
                (requestPresentationMessage, proofRecordIssuer) = await pair.Agent1.Provider.GetService<IProofService>()
                .CreateRequestAsync(pair.Agent1.Context, new ProofRequest
                {
                    Name = "Test Verification",
                    Version = "1.0",
                    Nonce = await AnonCreds.GenerateNonceAsync(),
                    RequestedAttributes = new Dictionary<string, ProofAttributeInfo>
                    {
                        { "id-verification", new ProofAttributeInfo { Names = new [] { "name", "age" } } }
                    },
                    NonRevoked = new RevocationInterval
                    {
                        From = now - 1,
                        To = now
                    }
                });

                proofRecordHolder = await pair.Agent2.Provider.GetService<IProofService>()
                    .ProcessRequestAsync(pair.Agent2.Context, requestPresentationMessage, pair.Connection2);
                
                (presentationMessage, _) = await pair.Agent2.Provider.GetService<IProofService>()
                    .CreatePresentationAsync(pair.Agent2.Context, proofRecordHolder.Id, new RequestedCredentials
                    {
                        RequestedAttributes = new Dictionary<string, RequestedAttribute>
                        {
                        { "id-verification", new RequestedAttribute
                            {
                                CredentialId = holderCredentialId,
                                Revealed = true
                            }
                        }
                        }
                    });

                proofRecordIssuer = await pair.Agent1.Provider.GetService<IProofService>()
                    .ProcessPresentationAsync(pair.Agent1.Context, presentationMessage);

                valid = await pair.Agent1.Provider.GetService<IProofService>()
                    .VerifyProofAsync(pair.Agent1.Context, proofRecordIssuer.Id);

                Assert.True(valid);
            }
        }

        //[Fact]
#pragma warning disable xUnit1013 // Public method should be marked as test
        public async Task VerifyFail()
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            var request = "{\"name\": \"Proof of Education\", \"version\": \"1.0\", \"requested_attributes\": {\"0_name_uuid\": {\"name\": \"name\", \"restrictions\": [{\"issuer_did\": \"UG9KQCej9LwDwfEjuKNZhe\"}]}, \"0_date_uuid\": {\"name\": \"date\", \"restrictions\": [{\"issuer_did\": \"UG9KQCej9LwDwfEjuKNZhe\"}]}, \"0_degree_uuid\": {\"name\": \"degree\", \"restrictions\": [{\"issuer_did\": \"UG9KQCej9LwDwfEjuKNZhe\"}], \"non_revoked\": {\"to\": 1586454393}}}, \"requested_predicates\": {\"0_age_GE_uuid\": {\"name\": \"age\", \"p_type\": \">=\", \"p_value\": 18, \"restrictions\": [{\"issuer_did\": \"UG9KQCej9LwDwfEjuKNZhe\"}]}}, \"non_revoked\": {\"to\": 1586454394}, \"nonce\": \"352167127950916803138329\"}";
            var proof = "{\"proof\": {\"proofs\": [{\"primary_proof\": {\"eq_proof\": {\"revealed_attrs\": {\"date\": \"23402637423876324098256519317695433196813217785795317220680415812348801086586\", \"degree\": \"460273229220408542178729328948548235132905393400001582342944147813984660772\", \"name\": \"62816810226936654797779705000772968058283780124309077049681734835796332704413\"}, \"a_prime\": \"1796377138787164098845339821386692855637339943530932071357378878270751648152203036263396468168171721606590786652018041464862002243846058358587888462560101605957654717499617238562208975933002570662516693996776889139328711331456576537288615705688391153886409970706141398916366069908503161415478611572974207761638782976051401568231139887848492072661247188229013793267473005992650219207057205853368250003129214490608583978986343024408665627426791751560449621666279152313017678604849914373306337137854739772198986779601065278710347647237490031777588000115598282303013736824705972310381154342531540416466247066942240546373\", \"e\": \"57673865696230787037039739530822306442633708610924053985632461569473547187062601689631438097885802632201699624333303017800426310434390784\", \"v\": \"1300020676210025316890525607011137802535723179691503587199778012391539718868013501909335302440427182655449838000848567366685617182599329506378737704082246069230986611064454130168519669132593923983003477787289067526730963538892173523766995533462987566479377145640361231544893704837937732406342752359218337222292187883081114838322246623798027224178523439993541100639268132443414465168546834775377840033083367944838875685309167557710846105423381015240904559178733728816189185141798131775325402746684699111573741984303551905351719643146955252932651790522749446310428916069613573242754353510769106302679260409963439341597533239956165157850717632455436348448703909344793509534254741929830180708173967071621238093666981767817069749131476207657389403256521441317098270937769459983932821504743830061065404783478092844133010126251263920931598340783034839308598422305466141460247561205090287305515337819372034737631848898767847361443\", \"m\": {\"timestamp\": \"2058503774303791830030758111886343414345220023314422834792362006479082713081642298202157702052889036826826344898363783087588104125313543311326025581028499910526696655712845588520\", \"age\": \"2890965156594925439448198613140452478713041323510290532437979209778030709526159564804967423746042834628981102363343303846945753722604074250477268293678033815413253042318120984819\", \"master_secret\": \"2114899047006837479755176667847440549305788678628233695985843461520253116245162193177509388707656214768643008195302948513194395093553070768707240656550561327835850953169628863996\"}, \"m2\": \"2210508743751390849270225911292735313976516657433788235376260236508091800671411921877587286493117003484780906058858601980516337765345331834658890582093406\"}, \"ge_proofs\": [{\"u\": {\"0\": \"10492807304764039120774884530964681228263327728711227166707972175112192542849176504672284303294186697698429346130195695077146493397746239236451578349685271130544321875258751434711\", \"1\": \"15002439082484195391913525696930850000845693466305192350362190836577803320967586638464456783267661004785908936750505243455402023139181847687100639952377716443239766465493514715901\", \"3\": \"3446132378638420884969056911356930161442026675126796877780516109818482158947595405716731379244386094473959904731608596146095098512958865290299336495645247466825272726944390068194\", \"2\": \"12492540388505416187233339814703810720515326760455515941994109030639119401441786175794371679107283075503510694234322412560079390149011635321524038005899358313776327546973328246349\"}, \"r\": {\"DELTA\": \"1000468102817610691309093359980796684096187225082728150701792583437843628299637655698161321392976434974658133849264260921747308412210439036332774065065222902245458207965105719738434962492454711658300928887132898935854874984558898099164176736578057398052038535156114970974467749515987120233005544480049070831555341038495553171868460714327514214340892959072876431952100989469206652469343115185978488092260211071891807007313512808618550507608482111119799842479092544675018576666584724358003525716791304869868932001542933123449832186300568752781729241312422834462013874606215914683027745743800458131340452007331754860621398172802837267520057037583494412659601092926090172711907243989378483191982788968758629947130104014310\", \"0\": \"685356291899915792684180081756288116115010801187552697303783635826429805196197866952704250490430980670853319779312845095545338815921602443486454489712882593379744054004859792676272026982980087596820650992857283902918115442278814491691241049720989496044557266182876008596026281014409552892065706726587989421693115173395935916447912789057393715987731423942293726567946774133065210325277354519832768801554912779540257878466152920872436278516942031498280808509205627863770148371025309011581976616613606619381986447683644359806973133609816890793038229070399025163344702692998639177877562346590667506933888524958979981459776233518906508816587029048308545937731263711858634762941838777788784289471574153172399044544585155355\", \"1\": \"292956782897692409944739110465800645427754956066873882736677655380402040321754466253232602191787202218307206799021107948328387480495796666184059293322847067575883166142878644315423493094029902278815234127986039958323561644567349329532933398741309206920815471936907302035781828258840319999127210576923088503641761423180230502580295067262522309392746997446408249913480717351875318262458476460809111643172877023156111151295358255236373355070832767075498948839414231933230810410520514348141121782158663119180576651236676009354689705373777727349019880554084347337441803484884542750372475794177176632630254850440023264123972893825220999255603888755122895356916554937318117416502999945671832855755432479763957320579568446712\", \"3\": \"617210688862180945659810156369724765355090206359247464644972355472439679076218214696725954692888656205981196920512022030172759230336855691126195177944760116201714007425145852631822062773510937358940912686902507071676365429900976065865069702608634211831983861763486818094461638158608645440674623114182541206230178251419820802735022150539592897186418355794232869423953314660346759358117194911610579753777314968320997903062334610854019506294787662619676190291609659328010831188594338605776362550290844837322491098279776249932356736147605654906251708184746978103638992363376086292128914821755007085093361866764073664064388105649605371603446813741431790086795839531217756635042867166754168701098378692397719751887331821143\", \"2\": \"1171058414000639933697766596859161837763905958624760266722014529298318868628838234580889797249412162515461763405206834682042754770529368724578809112321780252311987409989060414564630253143918871555025735934749068414853285757120921813465281134279893835141733831470524704945165157781371308030125588625724906936956749090986481099799990327753849099818278967769777011638833048043046713359825830680059949819432118823753280386622176624543399458739632353227607796782975383962156430553256230061158023961803434465537291278377252792885732112996386008464624431581121557100860967942716979362729726307629378740561187490841378089600866831426362019840062830099934989934214125336424288967510842134286781833423743841916463950284534089179\"}, \"mj\": \"2890965156594925439448198613140452478713041323510290532437979209778030709526159564804967423746042834628981102363343303846945753722604074250477268293678033815413253042318120984819\", \"alpha\": \"87262050696340679105461487646509904331696274744807487670989820311303878580699882445133236667049581748458223085628288048746712664866048172510018685469889422058948565796466982992632515059324168590857244740259573649949028188148240495262037839664116394434656245816007968596566060468888313069529998807363104673880434172333398048871660006670797178943062726217282151075078748900115263264601713874862802503510340245400515100197920934622420741878975048269369013762733112678193667181664610703268247613085034755726758883870762909463112807107850004705744806870075047847572579536609309503812709259482087392797751437314684395366238990742354798944662024348321027498285369629427563871413284909357187656495272526600558999428237730094758470430465692624188188955752974951786231345670331018181501124360540492122218632528692864955201802538620438925940517529665\", \"t\": {\"1\": \"26355762175091513206595633766430269298375300340553385841837123208809620700195730877297356141657293350355126931855489065466177039372043548696239715597262018718240560254256818168683857079117004034241622537352277104704134943672206719511459622601165843116376034263158838919768659023688117692642426834864162088553794517052783316529564082742637300062051419401064296241915888730752454595243294959931537770137937756862735361384019344801613123230029035428144380465040914328679889206306809469474044270120959661577705233111579444934518552128751914511211147699772214775516514431156020176978188013915530261009503709249507485351721\", \"DELTA\": \"14802675718532031948310356873535408787889497226690514222562069669354624914649258379578017770956274489235676840992322072181539253413233397018317774613442410302823078890916251829990719714026486527199348269098914627371511407781191515048116472107893101881975593964571138695782209780359621247691389606167564981411718900682562021006614350319472547029013345841625575373651526384405510853096853726361758427010989967312651759667410979117315879310196704807931277091655676954661342512182338311519263615446808275205900382578400088138584100700575114417461125139330105165674752535485787450949035838292720256658899454693628544402777\", \"3\": \"24723876826062889449939728906648736442942791571905107417994431330416652541195049728880140605654818943044683361526600081927616586106162821270586178171099533891937978957006990275537326535082291779199473275690098140015057792162177679444453055112263911413070638335081538575299857367789978420373489824775640826586161460123823804391362835467726026016713858927307635921841957824303596493595292549622436605493511451947463958625704830262984341268316528119580284338436322543127488143974352628436050425547726460842865428992850975964866175332464053555510252661440225871974495768973196399810125671204854963312172570956967270191813\", \"0\": \"9593131939911681804683231749155464993181242062555076586278594977093156815974082015507083459267721293292060075548603907616378340053467176769980888721195983622302617177290785926887520294088884455159356114001623256121208905020013904147031986812323111996395674511534178849631188682478358636329463642409183232800755129756021864311296673641451290801614833598343157018293575282195350904512739888947961877106836066758889628859360035020810540912450208367392034943891098101489523347911196412755814178639638561916477690338058584030324052210073881450360455007437829690242675223124794616822534177845324277618398729247427042023746\", \"2\": \"44479510428267935496512817195794207903358455123831146764167003103299057038207040596816886080552484185386921467265619373492404004042337578621200359516816355299411420907062954124557274018215321055219697026678129719434016749085544346186843083983728694443010169400231912219439551863751548023239204729539688527547392740834018448731572867683512781930653105700974350749688630051052253155612328193543857443704788280861148876802552153017932060429394228443376990617457113903189280671015084451692219564521024389508650614028988504596084883708723948817062073779697360497382987767447170804457762141056546616906930650782939634731463\"}, \"predicate\": {\"attr_name\": \"age\", \"p_type\": \"GE\", \"value\": 18}}]}, \"non_revoc_proof\": {\"x_list\": {\"rho\": \"09A3535E1502701D448BF6F4FF56AC79BFE6A8B16ADFB904BAAF1322CA3E7D40\", \"r\": \"21CDE947704B53F55CB2F234CC01D6008C15BBBEF56A1A415FD598257231BB27\", \"r_prime\": \"0E52748FE51961EE65E9F4825FB1A75BEEB6FFDAA95C0301A026647CAECE2661\", \"r_prime_prime\": \"09C20E011399FF25AAAA3A63FA83664F9C27DB916D1675EE2C7210C51D328A9E\", \"r_prime_prime_prime\": \"0F4A940BE9658A81580D17148A8440F9D14D69CFDEFCA8D20E6B548E7DA9144E\", \"o\": \"08EE160A0BB13659192CC5B080C0F278EBC4D81457E72540A1C954EE5B5E6498\", \"o_prime\": \"20210CDDFA0AB6C3CE2F3E67DF87B86E77611F4B90DE49EC00F9AE89A75899E3\", \"m\": \"02AE6C44869A1FD8E2D3D82DF67C1A198D319EE0804E7ABEFE96B217721D38AB\", \"m_prime\": \"0EAE97346A5F48FD85BC7BFB68809453F9D21673856D5CE44080847FBD9D2FBB\", \"t\": \"19F45C233193E48B85BDB67CBD23B1C0122BEDDF163055E61EFFD5D9A2B0A58C\", \"t_prime\": \"0B91B797C80092CB622AF2EAE4B5369C56DAD7E0872CED0D7C1F524C87FEE68B\", \"m2\": \"21721FAE45C4F8DBA0B0E1C3E1E14055CFEF4EDDE1E91B76E7F1511A08B4EDE5\", \"s\": \"2212F34CE61D3E1661CF7CA0471424386FE3C4C04DC7DDCF8082C13F3F2AF2BC\", \"c\": \"1686AD6DB798E0815A74345CB832AF24D80F180FA408A40A9D6B3C9073447F50\"}, \"c_list\": {\"e\": \"6 5609D845B422FD07B8392099E7618A05749E28C0C0B766C44B0D9832D487B373 4 232ECF698D2F20942B0E98FF90EDDC5C838257FC4C61F2397E2B2A3D85429B38 4 2A549B32566566B2FBC6119E3F538A1B39086325D5A89C855C12CF9EF5CAAF21\", \"d\": \"6 2C16B5FB6BB7D95722645835BDFF7F772169BE80244F1E9BA9138FBBA7C0FF0A 4 1D7C0D10D2BA0D86942FFA810F1D13AAE93FCD7387B78DE95B8289E6129A698B 4 11203F39567249B163A3D7DBB479F0DE2E04C9468886AFAB0A17F1AA5F44A313\", \"a\": \"6 3FF88E6B45E182F3D44AFDDFF6EB4A53FFAA31F40ADD28D7C16232EEDEC18EFB 4 31DFBE262F1581A38EFFB5D82FBB04E16694C15FE4BC318D7C926A0333C8146E 4 21E9C0B1D8FEE15A9A3558E20422B107E72BA1C458CB5DC4E6E12A3514DA37F7\", \"g\": \"6 5C1F2CA3B2F386A101C81503675D9E7D555312098D190B4D5AC2531F06424A9A 4 2EF36A775CE908028DD5B0CBCBA126EBEF5B3348A978C5906D3E651EE73C7215 4 2A97B7C0160372CF53A67312C248151C4FD53E51A4CBA312FC40038E27AFC029\", \"w\": \"21 10B0E57D12B2C594BC8F12F015AEDE1591DFD78F0DE93576484331B2D8AA578C7 21 13A1FB366C0EC4157FD8E01E9AECE005DB351C4C8BDC5F96B5D14C1A987DFEE26 6 6791F55FDC9094F200F6C3C61C439BD967309CBFE3768B30F4BAC3BEACECB9E1 4 215D1EE1445713A552D1854A6ECFCD15EBF6B3BFB11F0BE8FEBC56D2C97FAE43 6 59042D96D404D156F241047CB14085291A7693EFAB1A557F3FB982A8868C2E74 4 358E06BB5EDA7F669A18753004DB3F08A0D6D87C3657B16B03900F4440FD78B3\", \"s\": \"21 1201708222F214FFBAFF5D342A7926798748BF16A69AFE71755A31734BBDB7207 21 124A0D9961C6609CEB6FC40A6D81CD2BC5B41AD943C679670E6173C40190DB0C4 6 75ADADC06AB90849E2ED06BC1713E31486BB8F2C311B076EBFDD1DC6FD99BB33 4 32551C5BD958EB2A03A0F72D3692700C300994F7328D7311E417F02F19547133 6 79FD5A81EB5463AAA1DB72297619F11584246CF327049CE98389697E79E10273 4 1BF4C94AB9DBC8F56E3A8E1DD4B42CADB1D4E5E704C204EB536F2BBF539BAD40\", \"u\": \"21 114C0567C4336926D32C9EAD9338796DAF52234D3105195E1DEE23BF6A84211C5 21 114BBCCF4AAE423A4194705300038A70CE29E981E9F7E03FD64D2E057C95B51C5 6 68CBE4EDD6C68CA588594FF74650AA3B8AA1987227F7D70020C2BA0DAE6719AF 4 1BA4A7BFB068AC406C64DC0FAD3DFBC3EC5A9A33ED818BDE7FF355AF096BC14D 6 68800B098B992952706D2BD04163C22C6CCEC0B3F45D6F5D5A23CC1EBFF4BEF7 4 17F549EFA7C320FB84558E2C7CECBD5D6A0281C5F60A62A116EDA11F6CBB2580\"}}}], \"aggregated_proof\": {\"c_hash\": \"33641206361173943242404561101907300064147950462760192925074930952486955954334\", \"c_list\": [[4, 29, 77, 223, 2, 162, 254, 49, 218, 8, 48, 183, 75, 163, 27, 40, 206, 152, 39, 153, 64, 1, 64, 38, 69, 92, 142, 206, 152, 150, 78, 152, 186, 17, 238, 82, 68, 228, 12, 91, 38, 165, 151, 183, 16, 7, 217, 150, 119, 175, 84, 98, 0, 211, 15, 31, 253, 219, 88, 229, 86, 50, 161, 30, 173, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], [4, 2, 112, 99, 221, 172, 123, 79, 32, 217, 5, 69, 165, 250, 171, 101, 137, 142, 35, 247, 64, 166, 109, 121, 172, 48, 1, 31, 97, 164, 109, 117, 138, 12, 15, 105, 250, 187, 25, 84, 178, 21, 147, 24, 107, 125, 194, 70, 149, 139, 8, 241, 235, 108, 230, 38, 85, 223, 70, 133, 68, 100, 206, 136, 150, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], [4, 24, 101, 74, 40, 151, 163, 98, 212, 98, 128, 102, 170, 242, 239, 27, 135, 108, 104, 98, 23, 249, 170, 67, 115, 139, 124, 129, 31, 20, 17, 140, 250, 22, 232, 84, 56, 61, 237, 180, 117, 73, 194, 80, 222, 59, 49, 140, 173, 240, 34, 83, 211, 233, 161, 87, 103, 78, 241, 118, 162, 94, 59, 167, 163, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], [4, 20, 202, 120, 42, 120, 97, 53, 149, 231, 224, 201, 209, 47, 62, 32, 188, 115, 76, 192, 226, 33, 184, 44, 204, 244, 12, 71, 175, 137, 226, 223, 53, 6, 144, 146, 254, 15, 1, 40, 193, 166, 132, 228, 241, 237, 108, 117, 31, 193, 114, 209, 188, 228, 57, 138, 210, 53, 5, 152, 138, 60, 62, 236, 174, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], [7, 155, 210, 194, 220, 237, 115, 40, 97, 19, 230, 114, 216, 100, 125, 88, 151, 72, 19, 72, 123, 221, 253, 79, 208, 205, 227, 203, 29, 211, 227, 24, 28, 183, 226, 237, 191, 143, 169, 250, 198, 137, 207, 183, 67, 42, 148, 31, 205, 52, 125, 77, 118, 54, 19, 16, 127, 156, 209, 148, 183, 126, 133, 39, 7, 252, 47, 117, 61, 208, 114, 224, 196, 49, 24, 46, 4, 217, 87, 172, 61, 80, 108, 230, 18, 120, 71, 160, 191, 107, 249, 196, 226, 110, 224, 126, 34, 169, 99, 165, 123, 114, 5, 54, 133, 146, 230, 141, 166, 17, 208, 146, 235, 12, 163, 9, 140, 0, 241, 186, 85, 147, 48, 40, 183, 186, 238, 166], [19, 249, 22, 138, 17, 122, 124, 109, 171, 37, 220, 218, 114, 207, 213, 171, 111, 236, 97, 131, 0, 218, 140, 219, 241, 118, 164, 143, 248, 98, 134, 55, 12, 213, 234, 134, 33, 233, 115, 102, 190, 6, 170, 78, 180, 154, 45, 53, 13, 124, 152, 237, 203, 132, 9, 227, 2, 17, 130, 63, 47, 253, 133, 218, 29, 236, 70, 147, 41, 38, 131, 42, 190, 14, 23, 54, 64, 156, 27, 161, 51, 28, 34, 234, 0, 248, 183, 26, 62, 81, 78, 86, 114, 212, 165, 209, 25, 216, 65, 99, 198, 224, 206, 23, 12, 202, 251, 165, 35, 68, 206, 112, 34, 233, 34, 233, 88, 220, 40, 168, 166, 49, 170, 130, 189, 74, 94, 78], [27, 58, 250, 90, 77, 210, 247, 66, 19, 41, 161, 226, 77, 2, 110, 130, 1, 212, 230, 97, 94, 20, 103, 105, 169, 178, 226, 255, 104, 183, 19, 88, 14, 242, 74, 95, 98, 201, 212, 178, 1, 15, 230, 123, 60, 60, 109, 249, 250, 55, 177, 145, 219, 24, 82, 181, 228, 23, 240, 246, 115, 80, 182, 111, 21, 35, 209, 205, 213, 179, 213, 103, 198, 170, 20, 186, 97, 46, 244, 140, 214, 183, 248, 62, 181, 212, 40, 78, 212, 121, 195, 229, 146, 39, 206, 21, 26, 105, 191, 175, 191, 118, 80, 44, 53, 30, 74, 31, 42, 53, 154, 88, 246, 123, 172, 15, 230, 213, 72, 153, 207, 11, 153, 122, 107, 218, 248, 202], [14, 58, 228, 108, 241, 172, 163, 167, 102, 201, 126, 4, 13, 92, 174, 144, 15, 179, 167, 7, 109, 117, 250, 183, 21, 198, 200, 25, 183, 229, 128, 46, 136, 173, 162, 96, 77, 31, 120, 125, 100, 227, 101, 200, 96, 42, 229, 234, 111, 18, 243, 24, 157, 160, 234, 193, 49, 208, 215, 151, 47, 236, 215, 131, 56, 163, 244, 67, 75, 82, 71, 62, 252, 97, 51, 235, 56, 229, 86, 129, 90, 186, 59, 185, 1, 184, 95, 171, 170, 97, 5, 194, 125, 212, 132, 42, 106, 129, 111, 66, 195, 144, 69, 32, 188, 89, 215, 209, 190, 210, 91, 100, 232, 36, 143, 130, 52, 73, 150, 14, 76, 200, 193, 50, 154, 170, 43, 164, 233, 5, 242, 41, 187, 245, 170, 59, 149, 209, 184, 217, 69, 67, 40, 134, 80, 102, 233, 95, 12, 112, 38, 112, 253, 198, 131, 88, 212, 230, 131, 181, 148, 205, 215, 132, 67, 44, 116, 235, 218, 9, 176, 149, 129, 212, 84, 232, 100, 6, 210, 66, 143, 206, 89, 116, 121, 102, 53, 200, 36, 221, 172, 176, 205, 48, 248, 90, 253, 184, 63, 215, 246, 215, 255, 151, 235, 250, 13, 72, 145, 173, 205, 167, 239, 141, 84, 56, 168, 33, 33, 118, 60, 198, 228, 16, 44, 26, 149, 40, 179, 98, 110, 152, 182, 179, 58, 113, 163, 232, 39, 118, 152, 80, 62, 134, 156, 46, 10, 193, 117, 208, 94, 253, 252, 5, 94, 69], [75, 254, 3, 162, 53, 77, 173, 52, 153, 100, 237, 243, 51, 58, 76, 84, 243, 86, 56, 78, 99, 232, 19, 64, 25, 246, 104, 2, 57, 176, 174, 125, 73, 169, 18, 229, 56, 215, 92, 166, 125, 217, 217, 143, 243, 91, 164, 32, 71, 42, 232, 47, 218, 171, 130, 62, 54, 116, 209, 249, 134, 90, 82, 86, 54, 8, 88, 89, 227, 5, 248, 149, 26, 209, 188, 85, 180, 123, 45, 57, 200, 109, 158, 198, 232, 50, 202, 103, 22, 213, 6, 65, 92, 172, 179, 111, 41, 102, 199, 126, 216, 34, 16, 190, 139, 138, 119, 71, 132, 33, 201, 39, 194, 125, 139, 71, 163, 180, 131, 149, 242, 30, 31, 2, 47, 74, 3, 239, 179, 155, 88, 223, 200, 41, 254, 1, 5, 187, 252, 4, 231, 253, 12, 124, 123, 40, 30, 242, 16, 55, 176, 229, 154, 146, 232, 114, 29, 76, 211, 247, 48, 38, 51, 113, 165, 62, 117, 186, 163, 240, 150, 61, 114, 168, 255, 204, 56, 172, 52, 229, 175, 146, 157, 152, 61, 132, 169, 33, 251, 7, 198, 123, 56, 239, 135, 178, 61, 109, 32, 39, 48, 41, 20, 28, 98, 199, 56, 172, 108, 54, 74, 11, 51, 195, 10, 243, 70, 78, 14, 122, 28, 12, 142, 199, 248, 142, 6, 147, 198, 40, 2, 25, 75, 94, 25, 78, 83, 209, 165, 23, 204, 185, 101, 241, 234, 253, 113, 177, 166, 202, 14, 89, 146, 237, 25, 66], [208, 199, 33, 82, 59, 2, 123, 196, 127, 34, 156, 114, 234, 235, 121, 174, 250, 165, 69, 230, 234, 235, 150, 82, 126, 203, 157, 86, 199, 233, 227, 57, 116, 49, 158, 43, 252, 254, 156, 88, 40, 251, 126, 132, 6, 96, 82, 180, 198, 17, 102, 119, 72, 36, 91, 78, 143, 245, 219, 177, 98, 18, 190, 219, 63, 237, 55, 4, 150, 21, 16, 60, 30, 11, 118, 119, 56, 22, 115, 123, 150, 72, 116, 112, 122, 194, 200, 127, 63, 25, 149, 211, 248, 24, 111, 39, 209, 15, 86, 231, 82, 116, 210, 122, 4, 241, 18, 45, 217, 152, 166, 128, 108, 177, 228, 252, 217, 35, 149, 247, 246, 229, 194, 183, 217, 118, 158, 184, 246, 194, 232, 245, 54, 233, 28, 192, 250, 170, 101, 94, 186, 214, 86, 132, 234, 63, 15, 156, 142, 253, 122, 44, 26, 20, 98, 175, 30, 73, 42, 35, 184, 83, 225, 5, 239, 14, 143, 171, 24, 85, 129, 239, 159, 249, 196, 36, 71, 175, 160, 54, 161, 247, 161, 141, 99, 22, 102, 241, 236, 96, 214, 214, 79, 98, 1, 11, 7, 95, 237, 12, 50, 87, 12, 7, 2, 28, 7, 46, 243, 215, 162, 193, 9, 206, 184, 241, 169, 13, 152, 164, 176, 237, 56, 255, 129, 152, 60, 220, 68, 92, 118, 0, 6, 95, 222, 138, 30, 125, 81, 27, 95, 226, 136, 39, 213, 192, 84, 20, 106, 152, 198, 128, 68, 228, 255, 41], [1, 96, 88, 120, 201, 42, 56, 30, 174, 160, 5, 49, 197, 53, 102, 113, 197, 198, 79, 12, 72, 97, 77, 153, 106, 167, 215, 121, 76, 151, 81, 130, 102, 106, 147, 123, 64, 142, 226, 128, 157, 133, 39, 231, 197, 8, 17, 203, 143, 183, 100, 197, 54, 124, 177, 195, 239, 231, 228, 153, 48, 130, 204, 117, 162, 194, 154, 115, 119, 157, 37, 95, 215, 47, 43, 90, 207, 110, 189, 100, 92, 202, 143, 16, 23, 58, 38, 116, 100, 167, 33, 161, 153, 245, 70, 41, 228, 98, 112, 138, 126, 88, 56, 238, 65, 104, 138, 48, 251, 93, 181, 53, 67, 100, 253, 128, 66, 49, 39, 53, 109, 141, 50, 151, 134, 243, 116, 51, 63, 128, 35, 103, 57, 155, 144, 192, 185, 83, 19, 204, 248, 228, 14, 100, 206, 226, 84, 36, 64, 88, 11, 233, 84, 229, 196, 56, 105, 153, 194, 125, 104, 226, 196, 185, 238, 81, 114, 54, 111, 130, 86, 19, 236, 136, 57, 246, 238, 87, 118, 61, 27, 126, 175, 18, 183, 87, 110, 1, 13, 176, 130, 245, 159, 97, 110, 36, 160, 66, 193, 216, 49, 65, 220, 157, 59, 205, 17, 6, 3, 24, 58, 161, 39, 94, 28, 60, 171, 101, 122, 233, 237, 67, 22, 111, 90, 86, 76, 138, 124, 91, 234, 53, 177, 159, 66, 165, 177, 172, 187, 66, 180, 151, 173, 21, 200, 66, 18, 114, 89, 101, 38, 192, 32, 234, 88, 9, 199], [195, 217, 208, 1, 111, 155, 234, 225, 3, 61, 62, 3, 140, 3, 183, 12, 171, 72, 51, 139, 231, 33, 162, 85, 190, 175, 181, 181, 93, 174, 247, 181, 234, 215, 96, 171, 23, 91, 187, 85, 81, 238, 94, 58, 175, 185, 229, 24, 232, 132, 247, 195, 29, 89, 4, 127, 66, 142, 193, 63, 90, 9, 184, 53, 12, 198, 26, 194, 128, 133, 153, 227, 137, 228, 181, 196, 59, 125, 203, 245, 114, 161, 204, 44, 246, 171, 160, 205, 40, 26, 209, 103, 229, 135, 20, 5, 109, 204, 100, 31, 182, 14, 107, 179, 194, 184, 48, 217, 115, 230, 112, 155, 120, 93, 29, 203, 166, 46, 23, 13, 20, 3, 60, 193, 214, 186, 215, 48, 245, 87, 23, 206, 91, 27, 200, 120, 109, 219, 89, 77, 170, 134, 10, 225, 106, 238, 108, 75, 221, 61, 107, 187, 121, 171, 70, 79, 230, 32, 45, 214, 157, 41, 192, 153, 74, 235, 24, 39, 147, 221, 107, 36, 38, 31, 5, 226, 65, 196, 213, 61, 43, 209, 230, 65, 108, 161, 40, 89, 53, 70, 74, 176, 221, 107, 12, 16, 155, 207, 69, 28, 23, 160, 79, 128, 234, 237, 125, 120, 188, 63, 72, 216, 17, 191, 220, 54, 241, 202, 18, 152, 10, 129, 67, 21, 178, 246, 179, 57, 172, 142, 150, 204, 61, 19, 73, 103, 184, 50, 12, 253, 117, 142, 200, 24, 151, 30, 94, 4, 126, 161, 22, 245, 141, 92, 146, 197], [117, 66, 128, 221, 82, 126, 136, 113, 147, 112, 28, 137, 211, 92, 25, 42, 80, 127, 135, 13, 33, 91, 185, 127, 4, 234, 169, 32, 157, 70, 204, 116, 195, 116, 183, 188, 40, 94, 216, 173, 201, 111, 170, 29, 198, 88, 81, 77, 31, 247, 220, 32, 74, 143, 138, 87, 214, 79, 78, 225, 4, 46, 118, 240, 30, 194, 238, 249, 124, 178, 215, 189, 244, 22, 197, 44, 8, 226, 198, 136, 121, 241, 11, 76, 75, 98, 15, 87, 92, 142, 10, 66, 200, 137, 18, 188, 213, 137, 182, 194, 169, 177, 28, 58, 237, 179, 81, 46, 102, 152, 174, 195, 227, 10, 11, 171, 67, 250, 112, 187, 105, 170, 84, 240, 196, 161, 45, 121, 183, 37, 94, 152, 100, 178, 83, 254, 203, 97, 54, 113, 16, 88, 237, 204, 64, 53, 207, 35, 105, 24, 139, 174, 196, 84, 243, 221, 10, 17, 114, 251, 163, 224, 243, 91, 218, 197, 13, 224, 46, 197, 233, 176, 151, 75, 105, 242, 74, 243, 80, 54, 199, 74, 222, 62, 22, 186, 223, 254, 82, 108, 92, 223, 162, 0, 39, 61, 248, 88, 135, 0, 142, 227, 51, 102, 53, 193, 134, 200, 38, 60, 223, 93, 24, 150, 70, 18, 128, 114, 108, 130, 200, 203, 186, 159, 247, 21, 51, 153, 82, 217, 30, 122, 163, 79, 200, 79, 114, 153, 231, 40, 238, 245, 248, 36, 28, 43, 164, 28, 246, 200, 175, 119, 190, 113, 197, 89]]}}, \"requested_proof\": {\"revealed_attrs\": {\"0_name_uuid\": {\"sub_proof_index\": 0, \"raw\": \"Alice Smith\", \"encoded\": \"62816810226936654797779705000772968058283780124309077049681734835796332704413\"}, \"0_degree_uuid\": {\"sub_proof_index\": 0, \"raw\": \"Maths\", \"encoded\": \"460273229220408542178729328948548235132905393400001582342944147813984660772\"}, \"0_date_uuid\": {\"sub_proof_index\": 0, \"raw\": \"2018-05-28\", \"encoded\": \"23402637423876324098256519317695433196813217785795317220680415812348801086586\"}}, \"self_attested_attrs\": {}, \"unrevealed_attrs\": {}, \"predicates\": {\"0_age_GE_uuid\": {\"sub_proof_index\": 0}}}, \"identifiers\": [{\"schema_id\": \"UG9KQCej9LwDwfEjuKNZhe:2:degree schema:32.13.70\", \"cred_def_id\": \"UG9KQCej9LwDwfEjuKNZhe:3:CL:7774:default\", \"rev_reg_id\": \"UG9KQCej9LwDwfEjuKNZhe:4:UG9KQCej9LwDwfEjuKNZhe:3:CL:7774:default:CL_ACCUM:9f68bc46-027a-42b8-a54f-c88cf9316194\", \"timestamp\": 1586454394}]}";

            var valid = await Host.Services.GetService<IProofService>()
                .VerifyProofAsync(Context, request, proof);
        }

        //protected override string GetPoolName()
        //{
        //    return "bcovrin-test";
        //}
    }
}
