using System;
using System.Collections.Generic;
using System.Text;

namespace Core.CrossCuttingConcerns.Logging.Serilog.Loggers.ApacheKafka
{
    public class ApacheKafkaCustomerProjectLogger : BaseKafkaLogger
    {
        public ApacheKafkaCustomerProjectLogger() : base(2)
        {

        }
    }
}
