using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using System.Diagnostics;
using System.IO;
using Amazon.DynamoDBv2;
using Amazon;

namespace AplicacaoAWS
{
    public class ReservationsDataUploader
    {
        public static readonly string ReservationsTableName = ReservationsTableCreator.ReservationsTableName;
        public static readonly string S3BucketName = Utils.LabS3BucketName;
        public static readonly string S3BucketRegion = Utils.LabS3BucketRegion;
        public static readonly string ReservationsDataKeyFile = Utils.ReservationsDataKeyFile;

        private static AmazonDynamoDBClient dynamoDBClient = null;
        private static AmazonS3Client s3 = null;

        public static int numberOffFailures = 0;

        public static void Principal()
        {
            Init();
            LoadReservationsData();
        }

        private static void LoadReservationsData()
        {
            string line;
            char[] splitter = { ',' };
            StreamReader reader = null;

            try
            {
                // Retrieve the reservations data file from the S3 bucket
                GetObjectRequest requestFromS3 = null;
                requestFromS3 = new GetObjectRequest()
                {
                    BucketName = S3BucketName,
                    Key = ReservationsDataKeyFile
                };

                using (var responseFromS3 = s3.GetObject(requestFromS3))
                {
                    reader = new StreamReader(responseFromS3.ResponseStream);
                    reader.ReadLine();

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] reservationsDataAttrValues = line.Split(splitter);

                        try
                        {
                            if (!reservationsDataAttrValues[0].ToLower().Equals("customerid"))
                            {

                                AddItemToTable(reservationsDataAttrValues);

                            }
                        }
                        catch (AmazonDynamoDBException ex)
                        {
                            numberOffFailures++;
                        }
                    }

                }


            }
            catch (FileNotFoundException e)
            {
                numberOffFailures = 99999;
            }
            catch (IOException e)
            {
                numberOffFailures = 99999;
            }
            finally
            {
                if (reader != null)
                {
                    try
                    {
                        reader.Close();
                    }
                    catch (IOException e)
                    {

                    }
                }
            }

            Debug.WriteLine("REaservations data upload complete.");
        }

        private static void Init()
        {
            dynamoDBClient = new AmazonDynamoDBClient();
            RegionEndpoint region = RegionEndpoint.USWest2;
            s3 = new AmazonS3Client(region);
        }

        // Add the item to the reservations data table
        private static void AddItemToTable(string[] reservationsDataAttrValues)
        {
            Solution.AddItemToTable(dynamoDBClient, reservationsDataAttrValues, ReservationsTableName);
        }
    }
}
