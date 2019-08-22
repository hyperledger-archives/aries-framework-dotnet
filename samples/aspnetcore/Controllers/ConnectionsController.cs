using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using AgentFramework.Core.Contracts;
using AgentFramework.Core.Extensions;
using AgentFramework.Core.Handlers;
using AgentFramework.Core.Handlers.Agents;
using AgentFramework.Core.Messages.Connections;
using AgentFramework.Core.Models;
using AgentFramework.Core.Models.Connections;
using AgentFramework.Core.Models.Events;
using AgentFramework.Core.Models.Records.Search;
using AgentFramework.Core.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebAgent.Messages;
using WebAgent.Models;
using WebAgent.Protocols.BasicMessage;

namespace WebAgent.Controllers
{
    public class ConnectionsController : Controller
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IConnectionService _connectionService;
        private readonly IWalletService _walletService;
        private readonly IWalletRecordService _recordService;
        private readonly IProvisioningService _provisioningService;
        private readonly IAgentProvider _agentContextProvider;
        private readonly IMessageService _messageService;
        private readonly WalletOptions _walletOptions;

        public ConnectionsController(
            IEventAggregator eventAggregator,
            IConnectionService connectionService, 
            IWalletService walletService, 
            IWalletRecordService recordService,
            IProvisioningService provisioningService,
            IAgentProvider agentContextProvider,
            IMessageService messageService,
            IOptions<WalletOptions> walletOptions)
        {
            _eventAggregator = eventAggregator;
            _connectionService = connectionService;
            _walletService = walletService;
            _recordService = recordService;
            _provisioningService = provisioningService;
            _agentContextProvider = agentContextProvider;
            _messageService = messageService;
            _walletOptions = walletOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var context = await _agentContextProvider.GetContextAsync();

            return View(new ConnectionsViewModel
            {
                Connections = await _connectionService.ListAsync(context)
            });
        }

        [HttpGet]
        public async Task<IActionResult> CreateInvitation()
        {
            var context = await _agentContextProvider.GetContextAsync();

            var (invitation, _) = await _connectionService.CreateInvitationAsync(context, new InviteConfiguration { AutoAcceptConnection = true });
            ViewData["Invitation"] = $"{(await _provisioningService.GetProvisioningAsync(context.Wallet)).Endpoint.Uri}?c_i={EncodeInvitation(invitation)}";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AcceptInvitation(AcceptConnectionViewModel model)
        {
            var context = await _agentContextProvider.GetContextAsync();

            var invite = MessageUtils.DecodeMessageFromUrlFormat<ConnectionInvitationMessage>(model.InvitationDetails);
            var (request, record) = await _connectionService.CreateRequestAsync(context, invite);
            await _messageService.SendAsync(context.Wallet, request, record, invite.RecipientKeys[0]);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ViewInvitation(AcceptConnectionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }

            ViewData["InvitationDetails"] = model.InvitationDetails;

            var invite = MessageUtils.DecodeMessageFromUrlFormat<ConnectionInvitationMessage>(model.InvitationDetails);

            return View(invite);
        }

        [HttpPost]
        public async Task<IActionResult> SendTrustPing(string connectionId)
        {
            var context = await _agentContextProvider.GetContextAsync();
            var connection = await _connectionService.GetAsync(context, connectionId);
            var message = new TrustPingMessage
            {
                ResponseRequested = true,
                Comment = "Hello"
            };

            var slim = new SemaphoreSlim(0, 1);
            var success = false;

            using (var subscription = _eventAggregator.GetEventByType<ServiceMessageProcessingEvent>()
                .Where(_ => _.MessageType == CustomMessageTypes.TrustPingResponseMessageType)
                .Subscribe(_ => { success = true; slim.Release(); }))
            {
                await _messageService.SendAsync(context.Wallet, message, connection);

                await slim.WaitAsync(TimeSpan.FromSeconds(5));

                return RedirectToAction("Details", new { id = connectionId, trustPingSuccess = success });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id, bool? trustPingSuccess = null)
        {
            var context = new AgentContext
            {
                Wallet = await _walletService.GetWalletAsync(_walletOptions.WalletConfiguration,
                    _walletOptions.WalletCredentials)
            };

            var model = new ConnectionDetailsViewModel
            {
                Connection = await _connectionService.GetAsync(context, id),
                Messages = await _recordService.SearchAsync<BasicMessageRecord>(context.Wallet,
                    SearchQuery.Equal(nameof(BasicMessageRecord.ConnectionId), id), null, 10),
                TrustPingSuccess = trustPingSuccess
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string connectionId, string text)
        {
            if (string.IsNullOrEmpty(text)) return RedirectToAction("Details", new { id = connectionId });

            var context = new AgentContext
            {
                Wallet = await _walletService.GetWalletAsync(_walletOptions.WalletConfiguration,
                    _walletOptions.WalletCredentials)
            };

            var sentTime = DateTime.UtcNow;
            var messageRecord = new BasicMessageRecord
            {
                Id = Guid.NewGuid().ToString(),
                Direction = MessageDirection.Outgoing,
                Text = text,
                SentTime = sentTime,
                ConnectionId = connectionId
            };
            var message = new BasicMessage
            {
                Content = text,
                SentTime = sentTime.ToString("s", CultureInfo.InvariantCulture)
            };
            var connection = await _connectionService.GetAsync(context, connectionId);

            // Save the outgoing message to the local wallet for chat history purposes
            await _recordService.AddAsync(context.Wallet, messageRecord);

            // Send an agent message using the secure connection
            await _messageService.SendAsync(context.Wallet, message, connection);

            return RedirectToAction("Details", new {id = connectionId});
        }

        [HttpPost]
        public IActionResult LaunchApp(LaunchAppViewModel model)
        {
            return Redirect($"{model.UriSchema}{Uri.EscapeDataString(model.InvitationDetails)}");
        }

        /// <summary>
        /// Encodes the invitation to a base64 string which can be presented to the user as QR code or a deep link Url
        /// </summary>
        /// <returns>The invitation.</returns>
        /// <param name="invitation">Invitation.</param>
        public string EncodeInvitation(ConnectionInvitationMessage invitation)
        {
            return invitation.ToJson().ToBase64();
        }

        /// <summary>
        /// Decodes the invitation from base64 to strongly typed object
        /// </summary>
        /// <returns>The invitation.</returns>
        /// <param name="invitation">Invitation.</param>
        public ConnectionInvitationMessage DecodeInvitation(string invitation)
        {
            return JsonConvert.DeserializeObject<ConnectionInvitationMessage>(Encoding.UTF8.GetString(Convert.FromBase64String(invitation)));
        }
    }
}
