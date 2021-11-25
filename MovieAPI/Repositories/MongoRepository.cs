using Common.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MovieAPI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAPI.Repositories
{ // MongoRepository implementeaza IMongoRepository<T>
     public class MongoRepository<T> : IMongoRepository<T> where T : MongoDocument
     { //legatura cu baza de date
          private readonly IMongoDatabase _db;
          //vom lua referinta la colectie
          private readonly IMongoCollection<T> _collection;
       //conexiune cu BD
          public MongoRepository(IMongoDbSettings dbSettings)
          {
               _db = new MongoClient(dbSettings.ConnectionString).GetDatabase(dbSettings.DatabaseName);// conexiunea cu bd
             
               // in doc based db sunt colectii, de aceea fiecare colectie e ca o lista de doc, pentru a cunoaste cu care lista/colectie vom lucra extragem colection name sau table name
               string tableName = typeof(T).Name.ToLower();
               _collection = _db.GetCollection<T>(tableName);
          }

          public void DeleteRecord(Guid id)
          {
                   //va fi sters doc dupa id
               _collection.DeleteOne(doc => doc.Id == id);
          }
          
          public List<T> GetAllRecords()
          {    
               // afiseaza toata lista de doc
               var records = _collection.Find(new BsonDocument()).ToList();
               return records;

          }

          public T GetRecordByID(Guid id)
          {
               var record = _collection.Find(doc => doc.Id == id).FirstOrDefault(); 
               // va fi afisat doc care are id = cu id transmis
               return record;
          }

          public T InsertRecord(T record) //insereaza un doc 
          {
               _collection.InsertOne(record);
               return record;
          }

          public void UpsertRecord(T record)//daca exista face update , daca nu atunci creaza/introduce un doc nou
          {
               _collection.ReplaceOne(doc => doc.Id == record.Id, record, 
                    new ReplaceOptions() { IsUpsert = true });

          }
     }
}
