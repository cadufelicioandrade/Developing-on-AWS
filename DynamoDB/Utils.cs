using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicacaoAWS
{
    public static class Utils
    {
        public static readonly string LabS3BucketName = "aws-tc-largeobjects";
        public static readonly string LabS3BucketRegion = "us-west-2";
        public static readonly string ReservationsDataKeyFile = "AWS-100-DEV/v3.1/binaries/input/lab-3-dynamoDB/ReservationsData.csv";
        public static readonly string CustomerReportPrefix = "AWS-100-DEV/v3.1/binaries/input/lab-3-dynamoDB/CustomerRecord";
    }
}
