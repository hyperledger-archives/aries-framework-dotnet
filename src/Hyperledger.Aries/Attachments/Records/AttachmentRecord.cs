using Hyperledger.Aries.Decorators.Attachments;
using Hyperledger.Aries.Storage;
using System;

namespace Hyperledger.Aries.Attachments.Records
{
    public class AttachmentRecord : RecordBase
    {
        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        /// <value>
        /// The nickname.
        /// </value>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets the type of the MIME.
        /// </summary>
        /// <value>
        /// The type of the MIME.
        /// </value>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>
        /// The filename.
        /// </value>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the last modified time.
        /// </summary>
        /// <value>
        /// The last modified time.
        /// </value>
        public DateTimeOffset? LastModifiedTime { get; set; }

        /// <summary>
        /// Gets or sets the attachment data
        /// </summary>
        /// <value></value>
        public AttachmentContent Data { get; set; }

        public string RecordId { get; set; }

        public override string TypeName => "AF.AttachmentRecord";
    }
}
