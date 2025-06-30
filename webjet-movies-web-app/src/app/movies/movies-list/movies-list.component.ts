import { Component, OnDestroy, OnInit } from '@angular/core';
import { CinemaWorldMovieModel } from '../model/cinemaWorldMovie.model';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MovieService } from '../service/movie.service';
import { subscribeOn, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';
import { CinemaWorldMovieDetailsModel } from '../model/cinemaWorldMovieDetails.model';

@Component({
  selector: 'app-movies-list',
  imports: [NgxDatatableModule, CommonModule],
  templateUrl: './movies-list.component.html',
  styleUrl: './movies-list.component.scss',
  standalone: true
})
export class MoviesListComponent implements OnInit, OnDestroy {
  cinemaWorldMovies: CinemaWorldMovieDetailsModel[] = [];
  isLoading: boolean = false;
  isLocal: boolean = false;
  constructor(private movieService: MovieService) {
    
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe;
  }
  private subscription = new Subscription()
  
  ngOnInit(): void {
    if (this.isLocal) {
      this.cinemaWorldMovies = [
        {id: "abc", poster: "https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTfwOTk@._V1_SX300.jpg",
           title: "title1 ajdfl askfdajlsdkf jalksdfjiejf j", type: "type 1", year: "1978",
          actores:"",
          awards:"",
          country:"",
          director:"",
          genre:"",
          language:"",
          metascore:"",
          plot:"",
          price:213.22,
          rated:"PG",
          rating:"8.8",
          released: "",
          runtime: "",
          votes: "",
          writer: "",
          provider:"Cinema World"
        },
        {id: "abc2", poster: "https://m.media-amazon.com/images/M/MV5BOTIyMDY2NGQtOGJjNi00OTk4LWFhMDgtYmE3M2NiYzM0YTVmXkEyXkFqcGdeQXVyNTU1NTfwOTk@._V1_SX300.jpg", title: "title2", type: "type 2", year: "2002",
          actores:"",
          awards:"",
          country:"",
          director:"",
          genre:"",
          language:"",
          metascore:"",
          plot:"",
          price:223.22,
          rated:"",
          rating:"",
          released: "",
          runtime: "",
          votes: "",
          writer: "",
          provider:"Film World"
        }]
    } else 
    {
      this.isLoading = true;
      this.subscription.add(this.movieService.getMovies().subscribe(result => {
        if (result) {
          console.log("Heyyyyyyyyyyyyyyyyy "+ result.length);
          this.cinemaWorldMovies = result;
        } else {
          console.log("THere is no movie!");
        }
        this.isLoading = false;
      }))
    }
    
  }

  // cinemaWorldMovies: CinemaWorldMovieModel[] = [
  //   {id: "abc", poster: "", title: "title1", type: "type 1", year: "1978"},
  //   {id: "abc2", poster: "", title: "title2", type: "type 2", year: "2002"}]

  
  
}
