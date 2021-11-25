using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
     //datele trebuie sa fie de tip document mongo, un doc care are id si data cand a fost modificat 
     //aceste date sunt utile pentru sincronizare

     public abstract class MongoDocument
     {
            [BsonId]
            public Guid Id { get; set; }
            public DateTime LastChangedAt { get; set; }
     }
}
