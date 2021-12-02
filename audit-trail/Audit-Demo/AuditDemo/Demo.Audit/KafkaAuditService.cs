using Confluent.Kafka;
using Demo.Core;
using Demo.MongoDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Audit
{
    public class KafkaAuditService : IAuditService
    {
        private readonly ILogger<KafkaAuditService> _logger;
        private readonly IConfiguration _config;

        private IProducer<Null, string> producer;

        public KafkaAuditService(ILogger<KafkaAuditService> logger, IConfiguration config)
        {
            _logger = logger;
            _logger.LogTrace("KafkaAuditService.constructor: started..");
            _config = config;

            _logger.LogTrace($"KafkaAuditService.constructor: Kafka initialization started..");
            var url = _config["AuditService:KafkaServerUrl"];
            _logger.LogTrace($"KafkaAuditService.constructor: Kafka Url '{url}'");
            var conf = new ProducerConfig { BootstrapServers = url };
            producer = new ProducerBuilder<Null, string>(conf).Build();
            _logger.LogTrace($"KafkaAuditService.constructor: Kafka initialization completed..");
        }

        public void Publish(string msg, Dictionary<string, object> options = null)
        {
            _logger.LogTrace("KafkaAuditService.Publish: started..");
            producer.Produce(_config["AuditService:KafkaTopic"].ToString(), new Message<Null, string> { Value = msg }, DeliveryHandler);
            _logger.LogTrace("KafkaAuditService.Publish: completed..");
        }

        private void DeliveryHandler(DeliveryReport<Null, string> r)
        {
            if (r.Error.IsError)
            {
                _logger.LogError($"ERROR - Kafka Delivery: {r.Error.Reason}");
            }
            else
            {
                _logger.LogTrace($"Delivered message to {r.TopicPartitionOffset}");
            }
        }

    }
}
