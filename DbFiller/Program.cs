using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace DbFiller
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GetFromImdb();
            //GetFromOmdb();
        }

        private static void GetFromOmdb()
        {
            var baseUrl = "http://www.omdbapi.com/?apikey=3c7bcc59&type=movie&plot=short&t=";
            var client = new HttpClient();

            ConsoleKeyInfo keyInfo;

            do
            {
                Console.Write("Search: ");
                var search = Console.ReadLine();

                var task = client.GetStringAsync(baseUrl + search);
                task.Wait();
                var jsonString = task.Result;

                var omdbMovie = JsonConvert.DeserializeObject<OmdbMovie>(jsonString);

                if (omdbMovie != null)
                {
                    SendToServer(omdbMovie);
                }

                Console.WriteLine("Continue?");
                keyInfo = Console.ReadKey();

            } while (keyInfo.Key == ConsoleKey.Enter);
        }

        private static void GetFromImdb()
        {
            var baseUrl = "https://imdb-api.com/en/API/SearchMovie/k_r1l40j26/";
            var client = new HttpClient();
            do
            {
                Console.Write("Search: ");
                var search = Console.ReadLine();

                var task = client.GetStringAsync(baseUrl + search);
                task.Wait();
                var jsonString = task.Result;

                var imdbResult = JsonConvert.DeserializeObject<ImdbResult>(jsonString);

                if (imdbResult != null)
                {
                    SendToServer(imdbResult.Results);
                }

            } while (true);
        }

        static void SendToServer(List<ImdbMovie> imdbMovies)
        {
            var client = new HttpClient();
            var httpMethod = new HttpMethod("POST");
            var baseUrl = "http://localhost:9000/api/movie";

            var random = new Random();
            foreach (var imdbMovie in imdbMovies)
            {
                var movie = new Movie
                {
                    Name = imdbMovie.Title,
                    Budget = random.Next(99999999),
                    Description = imdbMovie.Description
                };

                var json = JsonConvert.SerializeObject(movie);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(httpMethod, baseUrl)
                {
                    Content = content
                };

                try
                {
                    var task = client.SendAsync(request);
                    task.Wait();

                    Console.WriteLine("Yes");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error, { ex }");
                }
            }
        }

        static void SendToServer(OmdbMovie omdbMovie)
        {

            var movie = new Movie
            {
                Name = omdbMovie.Title,
                Actors = omdbMovie.Actors.Split(", ").ToList(),
                Budget = omdbMovie.BoxOffice != "N/A" ? Convert.ToDecimal(omdbMovie.BoxOffice.Replace("$", "")) : new Nullable<decimal>(),
                Description = omdbMovie.Plot
            };


            try
            {

                var client = new HttpClient();
                var httpMethod = new HttpMethod("POST");


                var json = JsonConvert.SerializeObject(movie);

                var baseUrl = "http://localhost:9000/api/movie";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(httpMethod, baseUrl)
                {
                    Content = content
                };

                var task = client.SendAsync(request);
                task.Wait();

                Console.WriteLine("Yes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error, { ex }");
            }

        }
    }
}
