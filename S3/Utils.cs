﻿using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S3
{
    public class Utils
    {
        public static string labS3BucketName = "us-west-2-aws-staging";
        public static string labS3BucketRegion = "us-west-2";

        public static string[] labBucketDataFileKeys =
        {
            "awsu-ilt/AWS-100-DEV/v2.2/binaries/input/lab-6-lambda/DrugadverseEvents_September.txt",
            "awsu-ilt/AWS-100-DEV/v2.2/binaries/input/lab-6-lambda/DrugadverseEvents_October.txt"
        };

        public static string[] StudentBucketDataFileKeys =
        {
            "DrugAdverseEvent_September.txt",
            "DrugAdverseEvent_October.txt"
        };

        // Sets up the student's input bucket with sample data files retrieved
        // from the lab bucket
        public static void Setup(AmazonS3Client s3ForStudentBuckets)
        {
            RegionEndpoint region = RegionEndpoint.USWest2;
            AmazonS3Client s3ForLabBucket;
            string textContent = null;

            s3ForLabBucket = new AmazonS3Client(region);

            ListBucketsResponse responseBuckets = s3ForStudentBuckets.ListBuckets();

            foreach (S3Bucket bucket in responseBuckets.Buckets)
            {
                if (bucket.BucketName == DataTransformer.InputBucketName)
                {
                    DataTransformer.VerifyBucketOwnership(DataTransformer.InputBucketName);
                    break;
                }
                else
                {
                    DataTransformer.CreateBucket(DataTransformer.InputBucketName);
                }
            }

            for (int i = 0; i < labBucketDataFileKeys.Length; i++)
            {
                GetObjectRequest requestForStream = new GetObjectRequest
                {
                    BucketName = labS3BucketName,
                    Key = labBucketDataFileKeys[i]
                };

                using (GetObjectResponse responseForStream = s3ForLabBucket.GetObject(requestForStream))
                {
                    using (StreamReader reader = new StreamReader(responseForStream.ResponseStream))
                    {
                        textContent = reader.ReadToEnd();

                        PutObjectRequest putRequest = new PutObjectRequest
                        {
                            BucketName = DataTransformer.InputBucketName,
                            Key = labBucketDataFileKeys[i].ToString().Split('/').Last(),
                            ContentBody = textContent
                        };

                        putRequest.Metadata.Add("ContentLength", responseForStream.ContentLength.ToString());
                        s3ForStudentBuckets.PutObject(putRequest);
                    }
                }
            }
        }

    }
}
