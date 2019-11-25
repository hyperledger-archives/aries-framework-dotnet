namespace Hyperledger.Aries.Routing
{
    public static class RoutingTypeNames
    {
        public const string CreateInboxMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basic-routing/1.0/create-inbox";
        public const string CreateInboxResponseMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basic-routing/1.0/create-inbox-response";
        
        public const string RestoreInboxMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basic-routing/1.0/restore-inbox";
        public const string UpdateInboxMetadataMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basic-routing/1.0/update-inbox-metadata";

        public const string AddRouteMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basic-routing/1.0/add-route";

        public const string GetInboxItemsMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basic-routing/1.0/get-inbox-items";
        public const string GetInboxItemsResponseMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basic-routing/1.0/get-inbox-items-response";
        
        public const string DeleteInboxItemsMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basic-routing/1.0/delete-inbox-items";
        public const string InboxItemMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basic-routing/1.0/inbox-item";

        public const string AddDeviceInfoMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/basic-routing/1.0/add-device-info";
    }
}