import { Component } from '@angular/core';
import { NgForOf, NgIf } from '@angular/common'; 
import { ApiService } from '../../services/ApiService';

@Component({
  selector: 'app-search-results',
  imports: [NgForOf, NgIf],
  templateUrl: './search-results.component.html',
  styleUrl: './search-results.component.scss'
})
export class SearchResultsComponent {

  results: { name: string; resources: string }[] = [];
  isVisible = false;
  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.apiService.filteredResults$.subscribe({
      next: (data) => {
        this.isVisible = data.length === 0 ? false : true;
        this.results = data; // Update results whenever the shared state changes
      },
      error: (error) => console.error('Error subscribing to results:', error),
    });
  }
}
