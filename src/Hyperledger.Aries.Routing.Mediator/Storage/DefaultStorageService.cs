using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hyperledger.Aries.Configuration;
using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Extensions;
using Microsoft.Extensions.Options;

namespace Hyperledger.Aries.Routing.Mediator.Storage
{
    /// <summary>
    /// Default backup storage service. Uses filesystem for storing backup.
    /// </summary>
    /// <seealso cref="Hyperledger.Aries.Routing.Mediator.Storage.IStorageService" />
    public class DefaultStorageService : IStorageService
    {
        private readonly AgentOptions agentOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultStorageService"/> class.
        /// </summary>
        /// <param name="agentOptions">The agent options.</param>
        public DefaultStorageService(IOptions<AgentOptions> agentOptions)
        {
            this.agentOptions = agentOptions.Value;

            if (!Directory.Exists(this.agentOptions.BackupDirectory))
            {
                Directory.CreateDirectory(this.agentOptions.BackupDirectory);
            }
        }

        /// <summary>Stores the backup and all attachments.</summary>
        /// <param name="backupId">The backup identifier.</param>
        /// <param name="attachments">The attachments.</param>
        /// <returns>The datetime offset used for this backup</returns>
        /// <exception cref="ArgumentException">Invalid payload - attachments</exception>
        public Task<DateTimeOffset> StoreBackupAsync(string backupId, IEnumerable<Attachment> attachments)
        {
            if (attachments is null || !attachments.Any())
            {
                throw new ArgumentException("Invalid payload", nameof(attachments));
            }

            var nowUnix = DateTimeOffset.Now;
            var backupDir = Path.Combine(agentOptions.BackupDirectory, backupId, nowUnix.ToUnixTimeSeconds().ToString());

            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }

            foreach (var attachment in attachments)
            {
                var payloadInBytes = attachment.Data.Base64.GetBytesFromBase64();
                File.WriteAllBytes(Path.Combine(backupDir, attachment.Id), payloadInBytes);
            }
            
            return Task.FromResult(nowUnix);
        }

        /// <summary>
        /// Retrieves the latest backup.
        /// </summary>
        /// <param name="backupId">The backup identifier.</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">Wallet for key {backupId} was not found.</exception>
        public async Task<List<Attachment>> RetrieveBackupAsync(string backupId)
        {
            string backupDirectory = null;
            try
            {
                backupDirectory = Directory.EnumerateDirectories(GetBackupPath(backupId))
                    .OrderByDescending(x => x)
                    .FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't enumerate directory backup: {backupId}", e);
            }

            if (backupDirectory == null) throw new Exception($"Backup directory not found: {backupDirectory}");
            var filename = Path.GetFileName(backupDirectory);

            return await RetrieveBackupAsync(
                backupId: backupId,
                timestamp: long.Parse(filename));
        }

        /// <summary>
        /// Lists the available backups for the given <c>backupId</c>.
        /// </summary>
        /// <param name="backupId">The backup identifier.</param>
        /// <returns></returns>
        public Task<IEnumerable<string>> ListBackupsAsync(string backupId)
        {
            return Task.FromResult(Directory.EnumerateDirectories(GetBackupPath(backupId)));
        }

        public Task<List<Attachment>> RetrieveBackupAsync(string backupId, long timestamp)
        {
            var path = Path.Combine(GetBackupPath(backupId), $"{timestamp}");

            return Task.FromResult(
                Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly)
                .Select(x => new Attachment
                {
                    Id = Path.GetFileName(x),
                    Data = new AttachmentContent
                    {
                        Base64 = File.ReadAllBytes(x).ToBase64String()
                    }
                })
                .ToList());
        }
        
        private string GetBackupPath(string backupId) => Path.Combine(agentOptions.BackupDirectory, backupId);
    }
}