import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SearchComponent } from './search/search.component';
import { SearchResultsComponent } from './search-results/search-results.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,
    SearchComponent,
    SearchResultsComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'SWStarter';
}
