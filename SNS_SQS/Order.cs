using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNS_SQS
{
    public class Order
    {

        public Order()
        {

        }

        public Order(int id, string date, string details)
        {
            this.MyOrderId = id;
            this.MyOrderDate = date;
            this.MyOrderDetails = details;
        }

        public int MyOrderId { get; set; }
        public string MyOrderDate { get; set; }
        public string MyOrderDetails { get; set; }
        public string MySenderId { get; set; }
        public string MySentTimestamp { get; set; }


        public override string ToString()
        {
            return String.Format("Order id: {0}\nSenderId: {1}\nSentTimestamp: {2}\nOderDate: {3}\nOrderDetails: {4}\n", this.MyOrderId, this.MySenderId, this.MySentTimestamp ,this.MyOrderDate, this.MyOrderDetails);
        }

    }
}
