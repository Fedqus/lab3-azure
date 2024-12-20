using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace lab3_azure.Services
{
    public class BlobUploadService
    {
        private readonly IConfiguration _configuration;
        private BlobServiceClient _blobServiceClient;
        private BlobContainerClient _blobContainerClient;

        public BlobUploadService(IConfiguration configuration)
        {
            _configuration = configuration;
            _blobServiceClient = new BlobServiceClient(_configuration.GetConnectionString("Default"));
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetConnectionString("FileContainerName"));
            _blobContainerClient.CreateIfNotExists();
        }

        public void UploadFile(string fileName, Stream stream)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            blobClient.Upload(stream, true);
        }

        public List<string> GetBlobList()
        {
            var blobNames = new List<string>();

            foreach (BlobItem blobItem in _blobContainerClient.GetBlobs())
            {
                blobNames.Add(blobItem.Name);
            }

            return blobNames;
        }

        public Stream DownloadBlob(string fileName)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);

            if (blobClient.Exists())
            {
                var memoryStream = new MemoryStream();
                blobClient.DownloadTo(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }

            throw new FileNotFoundException($"Blob with name '{fileName}' not found.");
        }
    }
}
