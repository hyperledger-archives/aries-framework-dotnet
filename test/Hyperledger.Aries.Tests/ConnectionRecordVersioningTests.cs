using System.Threading.Tasks;
using Hyperledger.Aries.Features.Handshakes;
using Hyperledger.Aries.Features.Handshakes.Common;
using Hyperledger.Aries.Payments;
using Hyperledger.TestHarness;
using Newtonsoft.Json;
using Xunit;

namespace Hyperledger.Aries.Tests
{
    public class ConnectionRecordVersioningTests : TestSingleWallet
    {
        [Fact]
        public void NewConnectionRecordsWillReturnNewRecordVersion()
        {
            var record = new ConnectionRecord();
            
            Assert.Equal(1, record.RecordVersion);
        }

        [Fact]
        public void OldConnectionRecordsWillReturnDefaultRecordVersion()
        {
            var json = "{\"State\":\"Invited\"}";

            var obj = JsonConvert.DeserializeObject<ConnectionRecord>(json);
            
            Assert.Equal(0, obj.RecordVersion);
        }

        [Fact]
        public async Task RecordVersionWillReturnDefaultValue()
        {
            var record = new PaymentRecord();
            
            await recordService.AddAsync(Context.Wallet, record);

            var result = await recordService.GetAsync<PaymentRecord>(Context.Wallet, record.Id);
            
            Assert.Equal(0, result.RecordVersion);
        }
        
        [Fact]
        public async Task RoleWillReturnInviteeAsDefault()
        {
            var record = new ConnectionRecord();
            
            await recordService.AddAsync(Context.Wallet, record);

            var result = await recordService.GetAsync<ConnectionRecord>(Context.Wallet, record.Id);
            
            Assert.Equal(ConnectionRole.Inviter, result.Role);
        }
        
        [Fact]
        public async Task HandshakeProtocolWillReturnConnectionsAsDefault()
        {
            var record = new ConnectionRecord();

            await recordService.AddAsync(Context.Wallet, record);

            var result = await recordService.GetAsync<ConnectionRecord>(Context.Wallet, record.Id);
            
            Assert.Equal(HandshakeProtocol.Connections, result.HandshakeProtocol);
        }
        
        [Fact]
        public async Task HandshakeProtocolCanStoreAndRetrieveDidExchange()
        {
            var record = new ConnectionRecord() {HandshakeProtocol = HandshakeProtocol.DidExchange};
            
            await recordService.AddAsync(Context.Wallet, record);

            var result = await recordService.GetAsync<ConnectionRecord>(Context.Wallet, record.Id);
            
            Assert.Equal(HandshakeProtocol.DidExchange, result.HandshakeProtocol);
        }
    }
}
