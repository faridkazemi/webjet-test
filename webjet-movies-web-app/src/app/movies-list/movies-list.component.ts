import { Component } from '@angular/core';
import { CinemaWorldMovieModel } from './model/cinema-world-movie.model';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

@Component({
  selector: 'app-movies-list',
  imports: [NgxDatatableModule],
  templateUrl: './movies-list.component.html',
  styleUrl: './movies-list.component.scss',
  standalone: true
})
export class MoviesListComponent {

  cinemaWorldMovies: CinemaWorldMovieModel[] = [
    {id: "abc", poster: "", title: "title1", type: "type 1", year: "1978"},
    {id: "abc2", poster: "", title: "title2", type: "type 2", year: "2002"}]
  
}
