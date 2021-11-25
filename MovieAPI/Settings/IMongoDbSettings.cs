
namespace MovieAPI.Settings
{ //pentru a extrage datele din appsettings.json
    public interface IMongoDbSettings
     { //unde se afla nodul nostru si care BD noi vom folosi
          string DatabaseName { get; set; }
        //ptu legatura cu baza de date
          string ConnectionString { get; set; }

     }
}
