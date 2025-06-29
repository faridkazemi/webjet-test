import { Component, OnDestroy, OnInit } from '@angular/core';
import { CinemaWorldMovieModel } from '../model/cinema-world-movie.model';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MovieService } from '../service/movie.service';
import { subscribeOn, Subscription } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-movies-list',
  imports: [NgxDatatableModule, CommonModule],
  templateUrl: './movies-list.component.html',
  styleUrl: './movies-list.component.scss',
  standalone: true
})
export class MoviesListComponent implements OnInit, OnDestroy {
  cinemaWorldMovies: CinemaWorldMovieModel[] = [];
  isLoading: boolean = false;
  constructor(private movieService: MovieService) {
    
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe;
  }
  private subscription = new Subscription()
  
  ngOnInit(): void {
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

  // cinemaWorldMovies: CinemaWorldMovieModel[] = [
  //   {id: "abc", poster: "", title: "title1", type: "type 1", year: "1978"},
  //   {id: "abc2", poster: "", title: "title2", type: "type 2", year: "2002"}]

  
  
}
