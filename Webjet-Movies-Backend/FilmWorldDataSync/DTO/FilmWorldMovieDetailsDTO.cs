﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmWorldDataSync.DTO
{
    public class FilmWorldMovieDetailsDTO
    {
        public string ID { get; set; }
        public string Title { get; set; }

        public string Year { get; set; }

        public string Rated { get; set; }

        public string Released { get; set; }

        public string Runtime { get; set; }

        public string Genre { get; set; }

        public string Director { get; set; }

        public string Writer { get; set; }

        public string Actores { get; set; }

        public string Plot { get; set; }

        public string Language { get; set; }

        public string Country { get; set; }

        public string Awards { get; set; }

        public string Poster { get; set; }

        public string Metascore { get; set; }

        public string Rating { get; set; }

        public string Votes { get; set; }

        public string Type { get; set; }

        public string Price { get; set; }
        public string Provider { get; set; } = "FilmWorld";

    }
}
