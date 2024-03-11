using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3
{
    // The DataTransformer class transfoms objects in the input S3 bucket and
    // puts the transformed objects into the output S3 buckets.
    public class DataTransformer
    {
        public static readonly string[] Attributes = { "genericDrugName", "adverseReaction" };

        // Set input bucket name (must be globally unique)
        public static readonly string InputBucketName = "<name>";

        // Set output bucket name (must be blobally unique)
        public static readonly string OutputBucketName = "<name>";

        public static readonly string JsonComment = "\"comment\": \"DataTransformer JSON\",";

        // The Amazon S3 client allows you to manage buckets and objects programatically.
        public static AmazonS3Client s3ForStudentBuckets;

        // List used to store pre-signed URLs generated.
        public static Collection<string> preSignedUrls = new Collection<string>();

        public static void Principal()
        {
            ListObjectsRequest inputFileObjects;
            string fileKey = null;
            string transformedFile = null;
            string url = null;

            Init();

            try
            {
                CreateBucket(InputBucketName);
                CreateBucket(OutputBucketName);

                inputFileObjects = new ListObjectsRequest
                {
                    BucketName = InputBucketName
                };

                ListObjectsResponse listResponse;

                do
                {
                    // Get a list of objects
                    listResponse = s3ForStudentBuckets.ListObjects(inputFileObjects);
                    foreach (S3Object obj in listResponse.S3Objects)
                    {
                        fileKey = obj.Key;
                        if (fileKey.EndsWith(".txt"))
                        {
                            GetObjectResponse curObject = GetS3Object(s3ForStudentBuckets, InputBucketName, fileKey);
                            transformedFile = TransformText(curObject);

                            // Switch to enhaced file upload
                            PutObjectBasic(OutputBucketName, fileKey, transformedFile);

                            url = GeneratePresignedURL(fileKey);
                            if(url != null)
                            {
                                preSignedUrls.Add(url);
                            }
                        }
                    }

                    // Set the marker property
                    inputFileObjects.Marker = listResponse.NextMarker;
                } while (listResponse.IsTruncated);

                PrintPresignedUrls();

            }
            catch (AmazonServiceException ase)
            {
            }
            catch(AmazonClientException ace)
            {
            }
        }

        private static void PrintPresignedUrls()
        {
            foreach (string url in preSignedUrls)
            {
                Debug.WriteLine(url + "\n");
            }
        }

        public static void CreateBucket(string bucket)
        {
            ListBucketsResponse responseBuckets = s3ForStudentBuckets.ListBuckets();
            bool found = false;

            foreach (S3Bucket s3Bucket in responseBuckets.Buckets)
            {
                if (s3Bucket.BucketName == bucket)
                {
                    found = true;
                    VerifyBucketOwnership(bucket);
                    break;
                }
                else
                {
                    found = false;
                }
            }

            if (!found)
            {
                PutBucketRequest request = new PutBucketRequest();
                request.BucketName = bucket;
                s3ForStudentBuckets.PutBucket(request);

            }
        }

        // Verify that this AWS account is the owner of the bucket.
        public static void VerifyBucketOwnership(string bucketName)
        {
            bool ownedByYou = false;
            ListBucketsResponse responseBuckets = s3ForStudentBuckets.ListBuckets();

            foreach (S3Bucket bucket in responseBuckets.Buckets)
            {
                if (bucket.BucketName.Equals(bucketName))
                {
                    ownedByYou = true;
                }
            }

            if (!ownedByYou)
            {
                //print tela
            }
        }

        private static void Init()
        {
            s3ForStudentBuckets = CreateS3Client();
            Utils.Setup(s3ForStudentBuckets);
        }

        private static string TransformText(GetObjectResponse response)
        {
            string transformedText = null;
            StringBuilder sbJSON = new StringBuilder();
            string line;

            try
            {
                // Transform to JSON then write to file
                StreamReader reader = new StreamReader(response.ResponseStream);
                while ((line = reader.ReadLine()) != null)
                {
                    sbJSON.Append(TransformLineToJson(line));
                }

                reader.Close();
            }
            catch (IOException ex)
            {
            }

            transformedText = sbJSON.ToString();
            return transformedText;
        }

        private static string TransformLineToJson(string inputLine)
        {
            string[] inputLineParts = inputLine.Split(',');
            int len = inputLineParts.Length;

            string jsonAttrText = "{\n " + JsonComment + "\n";

            for (int i = 0; i < len; i++)
            {
                jsonAttrText = jsonAttrText + "\"" + Attributes[i] + "\"" + ":" + "\"" + inputLineParts[i] + "\"";
                if (i != len - 1)
                {
                    jsonAttrText = jsonAttrText + ",\n";
                }
                else
                {
                    jsonAttrText = jsonAttrText + "\n";
                }
            }

            jsonAttrText = jsonAttrText + "}, \n";
            return jsonAttrText;
        }

        // Create an instance of the AmazonS3Client object
        private static AmazonS3Client CreateS3Client()
        {
            return Solution.CreateS3Client();
        }

        // Retrieve each object from the input S3 bucket
        private static GetObjectResponse GetS3Object(AmazonS3Client s3Client, string bucketName, string fileKey)
        {
            return Solution.GetS3Object(s3Client, bucketName, fileKey);
        }

        // Upload object to output bucket
        private static void PutObjectBasic(string bucketName, string fileKey, string transformedFile)
        {
            Solution.PutObjectBasic(s3ForStudentBuckets, bucketName, fileKey, transformedFile);
        }

        // Generate a pre-signed URL to retrieve object
        private static string GeneratePresignedURL(string objectKey)
        {
            return Solution.GeneratePresignedURL(s3ForStudentBuckets, OutputBucketName, objectKey);
        }

        // Upload a file to a S3 bucket AES 256 server-side encrytion
        private static void PutObjectEnhanced(string bucketName, string fileKey, string transformedFile)
        {
            Solution.PutObjectEnhanced(s3ForStudentBuckets, bucketName, fileKey, transformedFile);
        }
    }
}
