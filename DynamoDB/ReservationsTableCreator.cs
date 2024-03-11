using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicacaoAWS
{
    public class ReservationsTableCreator
    {
        public static readonly string ReservationsTableName = "Reservations";
        public static readonly string CityDateIndexName = "ReservationsByCityDate";
        public static AmazonDynamoDBClient dynamoDBClient = null;

        public static void Principal()
        {
            Init();

            // In this sample, we will use the Document API to create our designed DynamoDB table.

            try
            {
                RemoveReservationsTableIfExists();
                CreateReservationTable();

            }
            catch (AmazonServiceException ase)
            {
            }
            catch (AmazonClientException ace)
            {

            }

        }

        private static void CreateReservationTable()
        {
            //Create an instance of a GlobalSecondaryIndex class
            GlobalSecondaryIndex gsi = new GlobalSecondaryIndex
            {
                IndexName = CityDateIndexName,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5L,
                    WriteCapacityUnits = 5L
                },
                Projection = new Projection { ProjectionType = "ALL" }
            };

            var indexKeySchema = new List<KeySchemaElement>
            {
                { new KeySchemaElement{AttributeName="City", KeyType="HASH"} },
                { new KeySchemaElement{AttributeName="Date", KeyType="RANGE"} }
            };
            gsi.KeySchema = indexKeySchema;

            //Create attribute definitions for CustomerId, City, Date.
            var attributeDefinitions = new List<AttributeDefinition>()
            {
                {
                    new AttributeDefinition
                    {
                        AttributeName = "CustomerID",
                        AttributeType = "S",
                    }
                },
                {
                    new AttributeDefinition
                    {
                        AttributeName = "City",
                        AttributeType = "S",
                    }
                },
                {
                    new AttributeDefinition
                    {
                        AttributeName = "Date",
                        AttributeType = "S",
                    }
                }
            };
            // Table key schema
            var tableKeySchema = new List<KeySchemaElement>()
            {
                {
                    new KeySchemaElement
                    {
                        AttributeName = "CustomerID",
                        KeyType = "HASH"
                    }
                }
            };

            Debug.WriteLine("Creatind DynamoDB table.");
            CreateReservationsTableWithIndex(attributeDefinitions, tableKeySchema, gsi);

            string status = null;
            do
            {
                status = GetTableStatus();

            } while (status != TableStatus.ACTIVE);

            Debug.WriteLine("Reservations table successfully created.");
        }

        private static void RemoveReservationsTableIfExists()
        {
            try
            {
                string tableName = ReservationsTableName;
                Debug.WriteLine("Attempting to delete DynamoDV Reservation table if one already exists.");

                var request = new DeleteTableRequest { TableName = tableName };
                dynamoDBClient.DeleteTable(request);

            }
            catch (ResourceNotFoundException e)
            {
                Debug.Write($"{ReservationsTableName} : {e.Message}");
            }
        }

        private static void Init()
        {
            dynamoDBClient = new AmazonDynamoDBClient();
        }

        //Check the table's status using the DescribeTable method Wait for the table to become active
        private static string GetTableStatus()
        {
            string status = null;
            System.Threading.Thread.Sleep(5000);
            try
            {
                var res = dynamoDBClient.DescribeTable(new DescribeTableRequest
                {
                    TableName = ReservationsTableName
                });
                status = res.Table.TableStatus;
            }
            catch (AmazonDynamoDBException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return status;
        }

        // Create an instance of CreateTableRequest class, Create table using the request defined
        private static void CreateReservationsTableWithIndex(List<AttributeDefinition> attributeDefinitions, List<KeySchemaElement> tableKeySchema, GlobalSecondaryIndex gsi)
        {
            CreateTableRequest request = new CreateTableRequest
            {
                TableName = ReservationsTableName,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = 5L,
                    WriteCapacityUnits = 10L
                },
                AttributeDefinitions = attributeDefinitions,
                KeySchema = tableKeySchema,
                GlobalSecondaryIndexes = { gsi }
            };
            dynamoDBClient.CreateTable(request);
        }
    }
}
