using System;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3.Model;
using static System.Net.Mime.MediaTypeNames;

namespace S3Access
{
    public class Rekognition
    {
        private readonly AmazonRekognitionClient _client;
        public Rekognition(string accessKey, string secretKey, Amazon.RegionEndpoint region)
        {
            _client = new AmazonRekognitionClient(accessKey, secretKey, region);
        }

        public string Rekognize(string bucket, string objectName)
        {
            string fileInfo = "";
            DetectTextRequest request = new DetectTextRequest()
            {
                Image = new Amazon.Rekognition.Model.Image()
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object()
                    {
                        Name = objectName,
                        Bucket = bucket
                    }
                }
            };

            try
            {
                DetectTextResponse detectTextResponse = _client.DetectTextAsync(request).GetAwaiter().GetResult();
                foreach (TextDetection text in detectTextResponse.TextDetections)
                {
                    if (text.DetectedText.Contains(';'))
                    {
                        text.DetectedText += "\n";
                    }
                    fileInfo += text.DetectedText + " ";
                }
            }

            catch (Exception e)
            {
                return e.Message;
            }
            return fileInfo;
        }
    }
}

