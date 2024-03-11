using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;


namespace AplicacaoAWS
{
    public class ReservationsStatistic
    {
        public static readonly string ReservationsTableName = ReservationsTableCreator.ReservationsTableName;
        public static readonly string CityDateIndexName = ReservationsTableCreator.CityDateIndexName;
        private static AmazonDynamoDBClient dynamoDBClient = null;
        public static int itemCount = 0;

        public static void Principal(string[] args)
        {
            Init();

            try
            {
                QueryByCity(args[0]);
            }
            catch (AmazonServiceException ase)
            {

            }
            catch (AmazonClientException ace)
            {

            }
        }

        public static int QueryByCity(string inputCity)
        {
            QueryResponse response = QueryCityRelatedItems(inputCity);

            string itemPID = null;
            string itemCity = null;
            string itemDate = null;

            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                itemPID = item["CustomerID"].S;
                itemCity = item["City"].S;
                itemDate = item["Date"].S;

                // Increments the itemCount variable to find the total number of
                // items returned by the query

                itemCount++;
            }

            return itemCount;
        }

        private static void Init()
        {
            dynamoDBClient = new AmazonDynamoDBClient();
        }

        // Create an instance of the QueryRequest class and query the global secondaty index by using the QueryRequest object defined
        private static QueryResponse QueryCityRelatedItems(string inputCity)
        {
            return Solution.QueryCityRelatedItems(dynamoDBClient, inputCity, ReservationsTableName, CityDateIndexName);
        }
    }
}
