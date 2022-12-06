﻿using Etherna.ServicesClient.Clients.Index;
using System.Threading.Tasks;

namespace Etherna.DevconArchiveVideoImporter.Services
{
    /// <summary>
    /// Etherna services
    /// </summary>
    public interface IEthernaService
    {
        /// <summary>
        /// Create batch
        /// </summary>
        Task<string> CreateBatchAsync();

        /// <summary>
        /// Delete video from index
        /// </summary>
        /// <param name="videoId">Video id</param>
        Task DeleteIndexVideoAsync(string videoId);

        /// <summary>
        /// Get last vaid manifest
        /// </summary>
        /// <param name="videoId">Video id</param>
        Task<VideoManifestDto?> GetLastValidManifestAsync(string? videoId);

        /// <summary>
        /// Get indexer info
        /// </summary>
        Task<SystemParametersDto> GetInfoAsync();

        /// <summary>
        /// Get all user video metadata
        /// </summary>
        Task<VideoDtoPaginatedEnumerableDto> GetAllUserVideoAsync(string userAddress, int? page, int? take);

        /// <summary>
        /// Get batch id from reference
        /// </summary>
        /// <param name="referenceId">Reference id</param>
        Task<string> GetBatchIdFromBatchReferenceAsync(string referenceId);

        /// <summary>
        /// Get if batch usable
        /// </summary>
        /// <param name="batchId">Batch id</param>
        Task<bool> IsBatchUsableAsync(string batchId);

        /// <summary>
        /// Set video offer by creator
        /// </summary>
        /// <param name="hash">hash</param>
        Task OfferResourceAsync(string hash);
    }
}
