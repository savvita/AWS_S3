using Amazon.S3;
using Amazon.S3.Model;

namespace S3Access
{
    public class S3Access
    {
        private readonly AmazonS3Client s3Client;
        public S3Access(string accessKey, string secretKey, Amazon.RegionEndpoint region)
        {
            s3Client = new AmazonS3Client(accessKey, secretKey, region);
        }

        public async Task<bool> DownloadFromBucketAsync(string bucketName, string objectName, string filePath)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectName
            };

            using GetObjectResponse response = await s3Client.GetObjectAsync(request);

            try
            {
                await response.WriteResponseStreamToFileAsync($"{filePath}\\{objectName}", true, CancellationToken.None);
                return true;
            }
            catch (AmazonS3Exception)
            {
                return false;
            }
        }

        public async Task<bool> UploadToBucketAsync(string bucketName, string objectName, byte[] bytes)
        {
            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectName,
                    InputStream = new MemoryStream(bytes)
                };

                var response = await s3Client.PutObjectAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteFromBucketAsync(string bucketName, string objectName)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = objectName
            };

            try
            {
                var response = await s3Client.DeleteObjectAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<S3Object>> ListBucketContentAsync(string bucketName)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName
            };

            try
            {
                var response = await s3Client.ListObjectsV2Async(request);
                return response.S3Objects.ToList();
            }
            catch
            {
                return new List<S3Object>();
            }
        }
    }
}
