using Amazon.S3.Model.Internal.MarshallTransformations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using S3Access;
using System.ComponentModel;

namespace S3Web.Controllers
{
    [ApiController]
    [Route("api/s3")]
    public class S3Controller : ControllerBase
    {
        private readonly S3Access.S3Access _access;
        private readonly string _bucketName = "watchshopbucket";
        public S3Controller(IConfiguration configuration)
        {
            _access = new S3Access.S3Access(configuration["S3:AccessKey"], configuration["S3:SecretKey"], Amazon.RegionEndpoint.EUNorth1);
        }

        [HttpGet("")]
        public async Task<IActionResult> Get([FromQuery]string? objectName)
        {
            if(string.IsNullOrEmpty(objectName))
            {
                var result = await _access.ListBucketContentAsync(_bucketName);
                return Ok(result);
            }

            try
            {
                var file = await _access.DownloadFromBucketAsync(_bucketName, objectName);
                return File(file, "application/octet-stream", objectName);
            }
            catch(FileNotFoundException)
            {
                return NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }


        [HttpPost("")]
        public async Task<IActionResult> Upload(IFormFileCollection upload)
        {
            if(Request.Form.Files.Count == 0)
            {
                return Ok(false);
            }

            var res = await _access.UploadToBucketAsync(_bucketName, Request.Form.Files[0].FileName, Request.Form.Files[0].OpenReadStream());
            return Ok(res);
        }

        [HttpDelete("")]
        public async Task<IActionResult> Remove([FromQuery]string objectName)
        {
            var res = await _access.DeleteFromBucketAsync(_bucketName, objectName); 
            return Ok(res);
        }

    }
}
