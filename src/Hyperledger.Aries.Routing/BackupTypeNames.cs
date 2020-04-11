using Hyperledger.Aries.Agents;

namespace Hyperledger.Aries.Routing
{
    public static class BackupTypeNames
    {
        public const string RetrieveBackupAgentMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/backup_restore/1.0/retrieve_backup";
        public const string RetrieveBackupResponseAgentMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/backup_restore/1.0/retrieve_backup_response";
        
        public const string StoreBackupAgentMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/backup_restore/1.0/store_backup";
        public const string StoreBackupResponseAgentMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/backup_restore/1.0/store_backup_response";

        public const string ListBackupsAgentMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/backup_restore/1.0/list_backups";
        public const string ListBackupsResponseAgentMessage = "did:sov:BzCbsNYhMrjHiqZDTUASHg;spec/backup_restore/1.0/list_backups_response";
    }
}