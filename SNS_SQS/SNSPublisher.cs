using Amazon.SimpleNotificationService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SNS_SQS
{
    // the SNSPublisher class publishes messages to SNS topics.
    public class SNSPublisher
    {

        // Set ARN for sns topic for email messages.
        private static string topicArnEmail = "<Email-SNS-Topic-ARN>"; //colocar o email

        // Set ARN for SNS topic for order messages.
        private static string topicArnOrder = "<Order-SNS-Topic- ARN>";

        private static string emailSubject = "Status of pharmaceuticals order.";
        private static string emailMessage = "Your pharmaceutical supplies will be shipped 5 business days from the date of order.";
        private static string orderDetails = "Ibuprofen, Acetaminophen";
        public static readonly int NumberOfMessages = 10;
        private static AmazonSimpleNotificationServiceClient snsClient = null;

        public static void Principal()
        {
            SNSPublisher snsPublisher = new SNSPublisher();
            snsPublisher.Init();
            snsPublisher.PublishMessages();
        }

        private void Init()
        {
            snsClient = CreateSNSClient();
        }

        private void PublishMessages()
        {
            PublishEmailMessage();
            PublishOrderMessages();
        }

        private void PublishOrderMessages()
        {
            string jsonOrder = null;
            // order in json format.
            Order order = null;
            MemoryStream stream = null;
            DataContractJsonSerializer ser = null;
            StreamReader reader = null;

            for (int i = 1; i < (NumberOfMessages + 1); i++)
            {
                order = new Order(i, "2015/10/" + i, orderDetails);
                stream = new MemoryStream();
                ser = new DataContractJsonSerializer(typeof(Order));
                ConvertOrderToJSON(order, stream, ser);

                stream.Position = 0;
                reader = new StreamReader(stream);
                jsonOrder = reader.ReadToEnd().ToString();

                stream.Dispose();
                PublishOrder(jsonOrder);
            }
        }

        private static AmazonSimpleNotificationServiceClient CreateSNSClient()
        {
            return Solution.CreateSNSClient();
        }

        private static void PublishEmailMessage()
        {
            Solution.PublishEmailMessage(snsClient, topicArnEmail, emailMessage, emailSubject);
        }

        private static void ConvertOrderToJSON(Order order, MemoryStream stream1, DataContractJsonSerializer ser)
        {
            Solution.ConvertOrderToJSON(order, stream1, ser);
        }

        private static void PublishOrder(string jsonOrder)
        {
            Solution.PublishOrder(snsClient, topicArnOrder, jsonOrder);
        }
    }
}
