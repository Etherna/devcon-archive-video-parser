﻿using Etherna.DevconArchiveVideoParser.CommonData.Json;
using Etherna.DevconArchiveVideoParser.CommonData.Models;
using Etherna.DevconArchiveVideoParser.CommonData.Requests;
using Etherna.DevconArchiveVideoParser.CommonData.Responses;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Etherna.DevconArchiveVideoParser.Services
{
    public class IndexerService
    {
        private readonly HttpClient httpClient;
        private readonly string indexUrl;
        private const string ETHERNA_INDEX_PARAMS_INFO = "api/v0.3/System/parameters";
        private const string INDEX_API_CREATEBATCH = "api/v0.3/videos";
        private const string INDEX_API_MANIFEST = "api/v0.3/videos/{0}";

        public IndexerService(
            HttpClient httpClient,
            string indexUrl)
        {
            this.httpClient = httpClient;
            this.indexUrl = indexUrl;
        }

        // Methods.
        public async Task<string> IndexManifestAsync(
            string hashReferenceMetadata,
            string? videoIndexIdReference)
        {
            var httpGetResponse = await httpClient.GetAsync(new Uri(indexUrl + INDEX_API_CREATEBATCH + $"/{videoIndexIdReference}")).ConfigureAwait(false);
            var haveIndexLink = httpGetResponse.StatusCode == System.Net.HttpStatusCode.OK;

            HttpResponseMessage httpResponse;
            if (haveIndexLink)
            {
                // Update manifest index.
                Console.WriteLine($"Update Index: {videoIndexIdReference}\t{hashReferenceMetadata}");
                using var httpContent = new StringContent("{}", Encoding.UTF8, "application/json");
                httpResponse = await httpClient.PutAsync(new Uri(indexUrl + INDEX_API_CREATEBATCH + $"/{videoIndexIdReference}?newHash={hashReferenceMetadata}"), httpContent).ConfigureAwait(false);
                httpResponse.EnsureSuccessStatusCode();
                return videoIndexIdReference!;
            }
            else
            {
                // Create new manifest index.
                Console.WriteLine($"Create Index: {videoIndexIdReference}\t{hashReferenceMetadata}");
                var indexManifestRequest = new IndexManifestRequest(hashReferenceMetadata);
                using var httpContent = new StringContent(JsonUtility.ToJson(indexManifestRequest), Encoding.UTF8, "application/json");
                httpResponse = await httpClient.PostAsync(new Uri(indexUrl + INDEX_API_CREATEBATCH), httpContent).ConfigureAwait(false);
                httpResponse.EnsureSuccessStatusCode();
                return await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        public async Task<MetadataVideo?> GetLastValidManifestAsync(string? videoId)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                return null;

            var manifestApi = string.Format(CultureInfo.InvariantCulture, INDEX_API_MANIFEST, videoId);
            var httpResponse = await httpClient.GetAsync(new Uri($"{indexUrl}{manifestApi}")).ConfigureAwait(false);

            httpResponse.EnsureSuccessStatusCode();

            var responseText = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonUtility.FromJson<MetadataVideo?>(responseText);
        }

        public async Task<IndexParamsResponse> GetParamsInfoAsync()
        {
            var httpResponse = await httpClient.GetAsync(new Uri($"{indexUrl}{ETHERNA_INDEX_PARAMS_INFO}")).ConfigureAwait(false);

            httpResponse.EnsureSuccessStatusCode();

            var responseText = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonUtility.FromJson<IndexParamsResponse>(responseText) ?? new IndexParamsResponse();
        }

    }
}
