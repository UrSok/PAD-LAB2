using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieAPI.Repositories;
using MovieAPI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //este o clasa ce mosteneste de la controllerbase
    public class MovieController : ControllerBase
    {
        private readonly IMongoRepository<Movie> _movieRepository;
        private readonly ISyncService<Movie> _movieSyncService;

        public MovieController(IMongoRepository<Movie> movieRepository,//injectam repository
        ISyncService<Movie> movieSyncService)
        {
            _movieRepository = movieRepository;
            _movieSyncService = movieSyncService;
        }

        // extragerea tuturor filmelor
        [HttpGet]
        //va returna o lista de movie
        public MovieApiResponse GetAllMovies()
        {
            var records = _movieRepository.GetAllRecords();
            var response = new MovieApiResponse(HttpContext.Request.Host.Value, records);

            return response;
        }

        //extragerea unui movie dupa id
        [HttpGet("{id}")]
        public MovieApiResponse GetMovieById(Guid id)
        {
            var record = _movieRepository.GetRecordByID(id);
            var response = new MovieApiResponse(HttpContext.Request.Host.Value, record);
            return response;
        }

        //extragerea movies o cantitate anumita
        [HttpGet("take/{amount}")]
        public IActionResult GetASpecificAmount(int amount)
        {
            if (amount <= 0)
            {
                var responseError = new MovieApiResponse(HttpContext.Request.Host.Value, "Amount can't be 0 or negative");
                return BadRequest(responseError);
            }

            var records = _movieRepository.GetAllRecords().Take(amount).ToList();
            var response = new MovieApiResponse(HttpContext.Request.Host.Value, records);
            return Ok(response);
        }

        // crearea unui movie nou
        [HttpPost]
        // o sa returneze un cod http
        public IActionResult Create(Movie movie)
        {
            movie.LastChangedAt = DateTime.UtcNow;// de mutat int-un serviciu aparte
            var inserted = _movieRepository.InsertRecord(movie);

            _movieSyncService.Upsert(movie);
            var response = new MovieApiResponse(HttpContext.Request.Host.Value, inserted);

            return Ok(response);
        }

        [HttpPut]
        public IActionResult Upsert(Movie movie)
        {
            if (movie.Id == Guid.Empty)
            {
                var responseError = new MovieApiResponse(HttpContext.Request.Host.Value, "Empty ID");
                return BadRequest(responseError);
            }
            movie.LastChangedAt = DateTime.UtcNow;// de mutat int-un serviciu aparte
            _movieRepository.UpsertRecord(movie);

            _movieSyncService.Upsert(movie);

            var response = new MovieApiResponse(HttpContext.Request.Host.Value, movie);

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {    // extragerea unui movie dupa id
            var movie = _movieRepository.GetRecordByID(id);

            if (movie == null)//verifica, daca exista asa movie dupa id
            {
                //daca nu -> atunci returnam mesajul de mai jos
                var responseError = new MovieApiResponse(HttpContext.Request.Host.Value, "Movie does not exist.");
                return BadRequest(responseError);
            }

            _movieRepository.DeleteRecord(id);//stergerea doc din bd

            movie.LastChangedAt = DateTime.UtcNow;
            _movieSyncService.Delete(movie);


            var response = new MovieApiResponse(HttpContext.Request.Host.Value, $"Deleted {id}");

            return Ok(response); //returmeaza mesaj ca a fost sters cu success
        }

        [HttpPut("sync")]
        public IActionResult UpsertSync(Movie movie)
        {
            var existingMovie = _movieRepository.GetRecordByID(movie.Id);

            if (existingMovie == null || movie.LastChangedAt > existingMovie.LastChangedAt)
            {
                _movieRepository.UpsertRecord(movie);
            }

            return Ok();
        }

        [HttpDelete("sync")]
        public IActionResult DeleteSync(Movie movie)
        {
            var existingMovie = _movieRepository.GetRecordByID(movie.Id);

            if (existingMovie != null || movie.LastChangedAt > existingMovie.LastChangedAt)
            {
                _movieRepository.DeleteRecord(movie.Id);
            }

            return Ok();
        }



    }
}
