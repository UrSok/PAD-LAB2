using Common.Models;
using Common.Utilities;
using Microsoft.Extensions.Hosting;
using SyncNode.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SyncNode.Services
{
    public class SyncWorkJobService : IHostedService
    {
        //un lucru de background si o coada
        private readonly ConcurrentDictionary<Guid, SyncEntity> documents =
             new ConcurrentDictionary<Guid, SyncEntity>();
        private readonly IMovieAPISettings _settings;
        private Timer _timer;

        public SyncWorkJobService(IMovieAPISettings settings)
        {
            _settings = settings;
        }

        public void AddItem(SyncEntity entity)
        {
            SyncEntity document = null;
            bool isPresent = documents.TryGetValue(entity.Id, out document);

            if (!isPresent || (isPresent && entity.LastChangeAt > document.LastChangeAt))//daca nu facem cu LastchageAt riscam sa nu facem o sincronizazre corecta
            {
                documents[entity.Id] = entity;//adaugam un element in dictionar daca nu este
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DoSendWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));//transmite mesaj de sincronizare fiecare 30 sec

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);//verificam daca este initializat si il stopam

            return Task.CompletedTask;
        }

        private void DoSendWork(object state)
        {
            //extrangem toate mejajele din dictiinar si le vom transmite la toti recieverii nostri
            foreach (var doc in documents)
            {
                SyncEntity entity = null;
                var isPresent = documents.TryRemove(doc.Key, out entity);//incercam sa facem remove si daca e cu succes se transmite la toti receiverii

                if (isPresent)
                {
                    var receivers = _settings.Hosts.Where(x => !x.Contains(entity.Origin)); // extragem vecinii de la care a venit acest mesaj

                    foreach (var receiver in receivers)
                    {
                        // url care va asculta la sincronizari
                        var url = $"{receiver}/{entity.ObjectType}/sync"; // de tip: http:// localhost:9001/api/movie

                        try
                        {
                            var result = HttpClientUtility.SendJson(entity.JsonData, url, entity.SyncType);

                            if (!result.IsSuccessStatusCode)
                            {
                                Console.WriteLine($"Synced with { receiver }!");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Sync error with { receiver }!");
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }
        }
    }
}
