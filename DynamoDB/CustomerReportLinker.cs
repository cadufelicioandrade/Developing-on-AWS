using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Diagnostics;
using Newtonsoft.Json;


namespace AplicacaoAWS
{
    public static class CustomerReportLinker
    {
        public static readonly string ReservationsTableName = ReservationsTableCreator.ReservationsTableName;
        public static readonly string CustomerReportPrefix = Utils.CustomerReportPrefix;
        public static readonly string S3BucketName = Utils.LabS3BucketName;
        public static readonly string S3BucketRegion = Utils.LabS3BucketRegion;
        public static AmazonDynamoDBClient dynamoDBClient = null;

        public static void main()
        {
            Init();
            LinkCustomerReport();
        }

        private static void LinkCustomerReport()
        {
            string reportUrl = null;
            string objectKey = null;

            // Sample reports exist for customer ids 1, 2, 3
            for (int i = 0; i < 4; i++)
            {
                objectKey = CustomerReportPrefix + i + ".txt";
                reportUrl = "https://s3-" + S3BucketRegion + ".amazonaws.com/" + S3BucketName + "/" + objectKey;
                UpdateItemWithLink(("" + i), reportUrl);
            }
        }

        private static void UpdateItemWithLink(string customerId, string reportUrl)
        {
            UpdateItemRequest requestUpdate = new UpdateItemRequest
            {
                TableName = ReservationsTableName,
                Key = new Dictionary<string, AttributeValue>()
                {
                    {"CustomerID", new AttributeValue{ S = customerId } }
                },
                ExpressionAttributeNames = new Dictionary<string, string>()
                {
                    { "#curl", "CustomerReportUrl" },
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                {
                    { ":val1", new AttributeValue{ S = reportUrl} }
                },
                // This expression does the following
                // Add a new attribute to the item
                UpdateExpression = "SET #curl = :val1"
            };

            UpdateItemResponse responseUpdate = UpdateItem(requestUpdate);

            string jsonDisplayText = JsonConvert.SerializeObject(responseUpdate);

        }

        private static void Init()
        {
            dynamoDBClient = new AmazonDynamoDBClient();
        }

        private static UpdateItemResponse UpdateItem(UpdateItemRequest requestUpdate)
        {
            return Solution.UpdateItem(dynamoDBClient, requestUpdate);
        }

    }
}
