using System;
using System.ComponentModel;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Handlers.Internal;
using AgentFramework.Core.Models.Wallets;
using Autofac;
using Hyperledger.Indy.DidApi;
using Hyperledger.Indy.WalletApi;
using Xamarin.Forms;

namespace AFMobileSample
{
    public partial class MainPage : ContentPage
    {
        private readonly IWalletService _walletService = App.Container.Resolve<IWalletService>();
        private readonly IConnectionService _connectionService = App.Container.Resolve<IConnectionService>();
        private readonly IProvisioningService _provisioningService = App.Container.Resolve<IProvisioningService>();

        private WalletConfiguration _config = new WalletConfiguration { Id = "MyWallet" };
        private WalletCredentials _creds = new WalletCredentials { Key = "SecretKey" };

        private string _agentKey;
        public string AgentKey { get => _agentKey; set { _agentKey = value; OnPropertyChanged(); } }

        public MainPage()
        {
            InitializeComponent();
        }

        async void OnProvision(object sender, EventArgs e)
        {
            IsBusy = true;

            try
            {
                var configuration = new ProvisioningConfiguration
                {
                    WalletCredentials = _creds,
                    WalletConfiguration = _config,
                    EndpointUri = new Uri("http://example.com/agent")
                };

                await _provisioningService.ProvisionAgentAsync(configuration);
            }
            catch (Exception ex)
            when ((ex is AgentFrameworkException) || (ex is WalletExistsException))
            {
                await DisplayAlert("Error", "An agent has already been provisioned.", "OK");
            }

            var wallet = await _walletService.GetWalletAsync(_config, _creds);
            var provisioningDetails = await _provisioningService.GetProvisioningAsync(wallet);

            AgentKey = provisioningDetails.Endpoint.Verkey;

            IsBusy = false;
        }

        async void OnMakeInvitation(object sender, EventArgs e)
        {
            var wallet = await _walletService.GetWalletAsync(_config, _creds);
            var invitation = await _connectionService.CreateInvitationAsync(new AgentContext { Wallet = wallet });

            InvitationDetails.Text = invitation.Invitation.ToJson();
        }
    }
}
