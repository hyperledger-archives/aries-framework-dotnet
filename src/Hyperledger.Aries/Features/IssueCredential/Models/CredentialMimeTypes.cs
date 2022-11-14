namespace Hyperledger.Aries.Features.IssueCredential
{
    /// <summary>
    /// Valid Mime types for credential attributes.
    /// </summary>
    public static class CredentialMimeTypes
    {
        /// <summary>
        /// Text mime type attribute.
        /// </summary>
        public const string TextMimeType = "text/plain";

        /// <summary>
        /// Application JSON mime type
        /// </summary>
        public const string ApplicationJsonMimeType = "application/json";
        
        /// <summary>
        /// MIME type for PDFs. The content should be a PDF file encoded in Base64.
        /// </summary>
        public const string ApplicationPdfMimeType = "application/pdf";
        
        /// <summary>
        /// MIME type for images in PNG format. The content should be a PNG file encoded in Base64.
        /// </summary>
        public const string ImagePngMimeType = "image/png";
    }
}
