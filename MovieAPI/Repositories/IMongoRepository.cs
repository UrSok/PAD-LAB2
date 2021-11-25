using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAPI.Repositories
{
     //repozitoriu este generic, adica lucreaza cu orice tip de date, datele trebuie sa fie doc mongo
     //de aceea impunem regula ca vom lucra doar cu T-urile de tip MongoDocument
     public interface IMongoRepository<T> where T : MongoDocument

     {
          // metoda de a extrage toate recordurile din BD
          List<T> GetAllRecords();
          // metoda de a insera careva date, care primeste ca parametru un record
          T InsertRecord(T record);
          // returnarea unui record dupa id si vom transmite guid-uri
          T GetRecordByID(Guid id);
          void UpsertRecord(T record); // update cu insert, dca nu este identitatea atunci o cream, cand dorim sa facem update si ea nu este, ea va fi creata , dar daca este va face doar update
          void DeleteRecord(Guid id);
    
     }
}
