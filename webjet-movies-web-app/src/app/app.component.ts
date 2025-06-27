import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MoviesListComponent } from './movies-list/movies-list.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MoviesListComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
  standalone: true
})
export class AppComponent {
  title = 'ebjet-movies-web-app';
}
