import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { CinemaWorldMovieModel } from "../model/cinemaWorldMovie.model";
import { CinemaWorldMovieDetailsModel } from "../model/cinemaWorldMovieDetails.model";

@Injectable({
    providedIn: 'root'
  })
  export class MovieService {
    private baseUrl = '/api/v1/movies';
  //'http://localhost:5112/api/v1/movies';
    constructor(private http: HttpClient) {}
  
    getMovies(): Observable<CinemaWorldMovieDetailsModel[]> {
      return this.http.get<CinemaWorldMovieDetailsModel[]>(`${this.baseUrl}`);
    }

  }