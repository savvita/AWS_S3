using Microsoft.AspNetCore.Mvc;

namespace S3Web.Controllers
{
    [ApiController]
    [Route("api/s3/rekognition")]
    public class RekognitionController : ControllerBase
    {
        private readonly S3Access.Rekognition _rekognition;
        private readonly string _bucketName = "rekognitionimagesbucket";
        public RekognitionController(IConfiguration configuration)
        {
            _rekognition = new S3Access.Rekognition(configuration["S3:AccessKey"], configuration["S3:SecretKey"], Amazon.RegionEndpoint.EUWest2);
        }

        [HttpGet("")]
        public IActionResult Get([FromQuery]string objectName)
        {
            return Ok(new { text = _rekognition.Rekognize(_bucketName, objectName) });
        }
    }
}
