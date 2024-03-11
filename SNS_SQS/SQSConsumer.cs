using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SNS_SQS
{
    public class SQSConsumer
    {
        public static readonly string QueueName = "MySQSQueue_A";
        private static AmazonSQSClient sqsClient = null;

        public static void principal()
        {
            SQSConsumer sqsConsumer = new SQSConsumer();
            sqsConsumer.Init();
            sqsConsumer.ConsumeMessages();
        }

        private void Init()
        {
            sqsClient = CreateSQSClient();
        }

        public void ConsumeMessages()
        {
            Order order = null;
            string queueUrl = null;

            queueUrl = GetURL();
            ReceiveMessageResponse receiveMessageResult = null;
            ReceiveMessageRequest request = CreateRequest(queueUrl);
            receiveMessageResult = GetMessageResult(request);

            
            foreach (Message message in receiveMessageResult.Messages)
            {
                string messageBody = message.Body;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Order));
                MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(messageBody));
                order = (Order)ser.ReadObject(stream);

                stream.Dispose();
                DeleteMessage(queueUrl, message);
            }

            Thread.Sleep(20000);
        }

        private static AmazonSQSClient CreateSQSClient()
        {
            return Solution.CreateSQSClient();
        }

        private static string GetURL()
        {
            return Solution.GetURL(sqsClient, QueueName);
        }

        private static ReceiveMessageRequest CreateRequest(string queueUrl)
        {
            return Solution.CreateRequest(queueUrl);
        }

        private static ReceiveMessageResponse GetMessageResult(ReceiveMessageRequest request)
        {
            return Solution.GetMessageResult(sqsClient, request);
        }

        private static void DeleteMessage(string queueUrl, Message message)
        {
            Solution.DeleteMessage(sqsClient, queueUrl, message);
        }
    }
}
