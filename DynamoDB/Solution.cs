using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicacaoAWS
{
    public class Solution
    {
        public static void AddItemToTable(AmazonDynamoDBClient dynamoDBClient, string[] reservationsDataAttrValues, string ReservationTableName)
        {
            var requestReservationsListing = new PutItemRequest
            {
                TableName = ReservationTableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    { "CustomerID", new AttributeValue{S = reservationsDataAttrValues[0]} },
                    { "City", new AttributeValue { S = reservationsDataAttrValues[1] } },
                    { "Date", new AttributeValue { S = reservationsDataAttrValues[2] } }
                }
            };
            dynamoDBClient.PutItem(requestReservationsListing);
        }

        public static QueryResponse QueryCityRelatedItems(AmazonDynamoDBClient dynamoDBClient, string inputCity, string ReservtionsTableName, string CityDateIndexName)
        {
            QueryRequest request = new QueryRequest
            {
                TableName = ReservtionsTableName,
                IndexName = CityDateIndexName,
                KeyConditionExpression = "City = :v_city",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":v_city", new AttributeValue{S = inputCity} }
                }
            };

            QueryResponse response = dynamoDBClient.Query(request);
            return response;
        }

        public static UpdateItemResponse UpdateItem(AmazonDynamoDBClient dynamoDBClient, UpdateItemRequest updateItemRequest)
        {
            UpdateItemResponse response = dynamoDBClient.UpdateItem(updateItemRequest);
            return response;
        }
    }
}
