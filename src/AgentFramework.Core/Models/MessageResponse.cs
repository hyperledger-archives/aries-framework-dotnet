using System;
using System.IO;

namespace AgentFramework.Core.Handlers
{
    /// <summary>
    /// Message response.
    /// </summary>
    public class MessageResponse : IDisposable
    {
        private readonly BinaryWriter _writer;

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public Stream Stream { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AgentFramework.Core.Handlers.MessageResponse"/> class.
        /// </summary>
        public MessageResponse()
        {
            Stream = new MemoryStream();
            _writer = new BinaryWriter(Stream);
        }

        /// <summary>
        /// Write the specified data to the response stream.
        /// </summary>
        /// <param name="data">Data.</param>
        public void Write(byte[] data)
        {
            if (data == null) return;

            Stream.Seek(0, SeekOrigin.End);
            _writer.Write(data);
        }

        /// <summary>
        /// Writes the stream contents to a byte array, regardless of the Position property.
        /// </summary>
        /// <returns>The data.</returns>
        public byte[] GetData() => (Stream as MemoryStream).ToArray();

        #region IDisposable Support
        private bool _disposedValue;

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Stream.Flush();
                    Stream.Dispose();
                }
                _disposedValue = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}