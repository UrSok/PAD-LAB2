using Common.Models;
using Common.Utilities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MovieAPI.Settings
{
     public class SyncService<T> : ISyncService<T> where T : MongoDocument
     {
          private readonly ISyncServiceSettigns _settings;
          // private readonly IHttpContextAccessor _httpContext;
          private readonly string _origin;

          public SyncService(ISyncServiceSettigns settings, IHttpContextAccessor httpContext)
          {
               _settings = settings;
               _origin = httpContext.HttpContext.Request.Host.ToString();
          }

          public HttpResponseMessage Delete(T record)
          {
               var syncType = _settings.DeleteHttpMethod;
               var json = ToSyncEntityJson(record, syncType);

              var response = HttpClientUtility.SendJson(json, _settings.Host, "POST");

               return response;
          }

          public HttpResponseMessage Upsert(T record)
          {
               var syncType = _settings.UpsertHttpMethod;
               var json = ToSyncEntityJson(record, syncType);

               var response = HttpClientUtility.SendJson(json, _settings.Host, "POST");

               return response;
          }

          // metoda care transforma o itentitate tip record in sync entity
          // si va da toata informatia gen de unde a venit, unde trebuie transmisa etc...
          private string ToSyncEntityJson(T record, string syncType)
          {
               var objectType = typeof(T); 
               var syncEntity = new SyncEntity() {
                    JsonData = JsonSerializer.Serialize(record),
                    SyncType = syncType,
                    ObjectType = objectType.Name, //pentru ca sumk node sa stie la care api sa se adreseze
                    Id = record.Id,
                    LastChangeAt = record.LastChangedAt,
                    Origin = _origin // necesar ca nodul de sinc sa stie la care alte noduri sa stransmita mesajul
               };

               var json = JsonSerializer.Serialize(syncEntity);

               return json;
          }
     }
}
