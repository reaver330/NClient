﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace NClient.Providers.Transport
{
    /// <summary>
    /// Response content.
    /// </summary>
    public class Content : IContent
    {
        /// <summary>
        /// Gets stream representation of response content
        /// </summary>
        public Stream StreamContent { get; }

        /// <summary>
        /// Gets response content encoding.
        /// </summary>
        public Encoding? Encoding { get; }
        
        /// <summary>
        /// Gets metadata returned by server with the response content.
        /// </summary>
        public IMetadataContainer Metadatas { get; }

        [SuppressMessage("ReSharper", "UnusedVariable")]
        public Content(Stream? streamContent = null, string? encoding = null, IMetadataContainer? headerContainer = null)
        {
            StreamContent = streamContent ?? new MemoryStream(Array.Empty<byte>());
            Encoding = string.IsNullOrEmpty(encoding) ? null : Encoding.GetEncoding(encoding);
            Metadatas = headerContainer ?? new MetadataContainer(Array.Empty<IMetadata>());
        }

        /// <summary>
        /// Gets string representation of response content.
        /// </summary>
        public override string ToString() => throw new NotImplementedException();

        public async Task<string> ReadToEndAsync() => await new StreamReader(StreamContent).ReadToEndAsync().ConfigureAwait(false);
    }
}
