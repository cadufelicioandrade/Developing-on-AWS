using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3
{
    public class Solution
    {
        public static AmazonS3Client CreateS3Client()
        {
            AmazonS3Client s3ForStudentBuckets = new AmazonS3Client();
            return s3ForStudentBuckets;
        }

        public static GetObjectResponse GetS3Object(AmazonS3Client s3Client, string bucketName, string fileKey)
        {
            GetObjectRequest request = new GetObjectRequest()
            {
                BucketName = bucketName,
                Key = fileKey
            };
            GetObjectResponse response = s3Client.GetObject(request);
            return response;
        }

        public static void PutObjectBasic(AmazonS3Client s3ForStudentBuckets, string bucketName, string fileKey, string transformedFile)
        {
            PutObjectRequest putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileKey,
                ContentBody = transformedFile
            };

            s3ForStudentBuckets.PutObject(putRequest);
        }

        public static string GeneratePresignedURL(AmazonS3Client s3ForStudentBuckets, string OutputBucketName, string objectKey)
        {
            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
            {
                BucketName = OutputBucketName,
                Key = objectKey,
                Protocol = Protocol.HTTP,
                Verb = HttpVerb.GET,
                Expires = DateTime.Now.AddSeconds(900) // 15 min
            };

            return s3ForStudentBuckets.GetPreSignedURL(request);
        }

        public static void PutObjectEnhanced(AmazonS3Client s3ForStudentBuckets, string bucketName, string fileKey, string transformedFile)
        {
            PutObjectRequest putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileKey,
                ContentBody = transformedFile,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };

            putRequest.Metadata.Add("contact", "carlos edu");
            s3ForStudentBuckets.PutObject(putRequest);

            GetObjectMetadataRequest encrtptionRequest = new GetObjectMetadataRequest()
            {
                BucketName = bucketName,
                Key = fileKey
            };

            ServerSideEncryptionMethod objectEncryption = s3ForStudentBuckets.GetObjectMetadata(encrtptionRequest).ServerSideEncryptionMethod;
            GetObjectMetadataResponse metadataResponse = s3ForStudentBuckets.GetObjectMetadata(encrtptionRequest);
            string contactName = metadataResponse.Metadata["x-amz-meta-contact"];
        }
    }
}
