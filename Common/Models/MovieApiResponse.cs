using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class MovieApiResponse
    {
        public string ServerIp { get; set; }
        public string Message { get; set; }
        public List<Movie> Movies { get; set; }

        public MovieApiResponse()
        {
            this.Movies = new List<Movie>();
        }

        public MovieApiResponse(string serverIp, string message)
        {
            this.ServerIp = serverIp;
            this.Message = message;
        }

        public MovieApiResponse(string serverIp, Movie movie)
        {
            this.ServerIp = serverIp;
            this.Movies = new List<Movie>() { movie };
        }

        public MovieApiResponse(string serverIp, List<Movie> movies)
        {
            this.ServerIp = serverIp;
            this.Movies = movies;
        }
    }
}
