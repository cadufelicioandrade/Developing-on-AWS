using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;
using Amazon.SimpleNotificationService;
using System.IO;
using System.Runtime.Serialization.Json;

namespace SNS_SQS
{
    public class Solution
    {
        public static AmazonSimpleNotificationServiceClient CreateSNSClient()
        {
            AmazonSimpleNotificationServiceClient snsClient = new AmazonSimpleNotificationServiceClient();
            return snsClient;
        }

        public static void PublishEmailMessage(AmazonSimpleNotificationServiceClient snsClient, string topicArnEmail, string emailMessage, string emailSubject)
        {
            snsClient.Publish(topicArnEmail, emailMessage, emailSubject);
        }

        public static void ConvertOrderToJSON(Order order, MemoryStream stream, DataContractJsonSerializer ser)
        {
            ser.WriteObject(stream, order);
        }

        public static void PublishOrder(AmazonSimpleNotificationServiceClient snsClient, string topicArnOrder, string jsonOrder)
        {
            snsClient.Publish(topicArnOrder, jsonOrder);
        }

        public static AmazonSQSClient CreateSQSClient()
        {
            AmazonSQSClient sQSClient = new AmazonSQSClient();
            return sQSClient;
        }

        public static string GetURL(AmazonSQSClient sQSClient, string QueueName)
        {
            GetQueueUrlResponse queueUrlResponse = sQSClient.GetQueueUrl(QueueName);
            string queueUrl = queueUrlResponse.QueueUrl;
            return queueUrl;
        }

        public static ReceiveMessageRequest CreateRequest(string queueUrl)
        {
            ReceiveMessageRequest request = new ReceiveMessageRequest(queueUrl);
            request.WaitTimeSeconds = 20;
            request.MaxNumberOfMessages = 10;
            return request;
        }

        public static ReceiveMessageResponse GetMessageResult(AmazonSQSClient sqsClient, ReceiveMessageRequest request)
        {
            ReceiveMessageResponse receiveMessageResponse = sqsClient.ReceiveMessage(request);
            return receiveMessageResponse;
        }

        public static void DeleteMessage(AmazonSQSClient sQSClient, string queueUrl, Message message)
        {
            string messageReceipHandle = message.ReceiptHandle;
            sQSClient.DeleteMessage(queueUrl, messageReceipHandle);
        }
    }
}
