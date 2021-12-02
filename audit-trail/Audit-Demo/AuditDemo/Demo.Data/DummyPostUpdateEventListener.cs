using Demo.Audit;
using Demo.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHibernate.Event;
using NHibernate.Intercept;
using NHibernate.Persister.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Data
{
    public class DummyPostUpdateEventListener : IPostUpdateEventListener
    {
        private readonly IAuditService _auditService;
        private readonly ILogger<DummyPostUpdateEventListener> _logger;
        private readonly NHibernate.Cfg.Configuration _nberConfig;

        public DummyPostUpdateEventListener(ILogger<DummyPostUpdateEventListener> logger, IAuditService auditService, NHibernate.Cfg.Configuration nberConfig)
        {
            _logger = logger;
            _auditService = auditService;
            _nberConfig = nberConfig;
        }

        //public void OnPostUpdateCollection(PostCollectionUpdateEvent @event)
        //{
        //    //if (@event.AffectedOwnerOrNull is Audit)
        //    //{
        //    //    return;
        //    //}

        //    //if (@event.AffectedOwnerOrNull is IAuditable)
        //    //{

        //        //var auditableEntity = @event.AffectedOwnerOrNull as IAuditable;
        //        //var entityId = auditableEntity.IdForAuditing;
        //        var entityFullName = @event.AffectedOwnerOrNull.GetType().FullName;

        //        //var fieldName = GetItemTypeFromGenericType(@event.Collection.GetType()).Name;

        //        //if (auditableEntity.LastModifiedBy == null)
        //        //{
        //        //    throw new AuditUpdateLastModifiedByNotSetExceptions(entityFullName, entityId);
        //        //}

        //        var collectionItems = @event.Collection as IEnumerable;
        //        var now = DateTime.Now;
        //        var updateId = Guid.NewGuid();

        //        //foreach (var item in collectionItems)
        //        //{
        //        //    //if (item.GetType() == typeof(IAuditable))
        //        //    //{
        //        //        var w = item as IAuditable;

        //        //        AddRecord(
        //        //            "U",
        //        //            entityFullName,
        //        //            entityId,
        //        //            fieldName + " Collection",
        //        //            "",
        //        //            w.IdForAuditing,
        //        //            now,
        //        //            auditableEntity.LastModifiedBy.Id.ToString(),
        //        //            updateId);
        //        //    }

        //        //}
        //    //}
        //}

        //public Task OnPostUpdateCollectionAsync(PostCollectionUpdateEvent @event, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            throw new NotImplementedException();
        }

        public Task OnPostUpdateAsync(PostUpdateEvent @event, CancellationToken cancellationToken)
        {
            _logger.LogTrace("DummyPostUpdateEventListener.OnPostUpdateAsync: Started..");
            var o = GetDirtyPropertiesJson(@event);
            _auditService.Publish(o);
            return Task.CompletedTask;
        }

        private string GetDirtyPropertiesJson(PostUpdateEvent @event)
        {
            IEntityPersister persister = @event.Persister;

            var result = new
            {
                TenantInfo = new { 
                    Name = "Test"
                },
                AppInfo = new {
                    Name = "TCP",                    
                },
                RecordInfo = new {
                    EntityType = persister.EntityName,
                    //((NHibernate.Persister.Entity.SingleTableEntityPersister)(@event).Persister).TableName
                    TableName = GetTableName(persister.EntityName),
                },
                Key = persister.EntityMetamodel.IdentifierProperty.Name,
                Value = @event.Id.ToString(),
                UpdateDate = DateTime.Now,
                Data = @event.Entity
            };
            return JsonSerializer.Serialize(result);
        }

        private string GetTableName(string entityType)
        {
            return _nberConfig.GetClassMapping(Type.GetType(entityType)).RootTable.Name;
        }
    }
}
