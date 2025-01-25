using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;

namespace crasbuild.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _blobContainerClient;

        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            var containerName = configuration["AzureBlobStorage:ContainerName"];
            _blobContainerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Generate a unique blob name
            var blobName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var blobClient = _blobContainerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            // Return the public URL for the uploaded blob
            return blobClient.Uri.ToString();
        }

        public async Task<List<string>> ListImagesAsync()
        {
            var images = new List<string>();
            await foreach (BlobItem blobItem in _blobContainerClient.GetBlobsAsync())
            {
                var blobClient = _blobContainerClient.GetBlobClient(blobItem.Name);
                images.Add(blobClient.Uri.ToString());
            }

            return images;
        }

        // Method to fetch a file as a stream
        public async Task<Stream> GetFileAsync(string fileName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                // Download the file and return its content as a stream
                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }

            return null; // Return null if the file does not exist
        }
    }
}

