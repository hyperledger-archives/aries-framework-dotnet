﻿using System;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Storage;
using Hyperledger.Aries;
using Autofac;
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
                var configuration = new AgentOptions
                {
                    WalletCredentials = _creds,
                    WalletConfiguration = _config,
                    EndpointUri = "http://example.com/agent",
                };

                await _provisioningService.ProvisionAgentAsync(configuration);
            }
            catch (Exception ex)
            when ((ex is AriesFrameworkException) || (ex is WalletExistsException))
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
            var (invitation, rec) = await _connectionService.CreateInvitationAsync(new AgentContext { Wallet = wallet });

            InvitationDetails.Text = invitation.Label;
        }
    }
}
