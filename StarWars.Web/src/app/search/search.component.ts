import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ApiService } from '../../services/ApiService';
import { debounceTime, distinctUntilChanged, Subject, switchMap } from 'rxjs';

@Component({
  selector: 'app-search',
  imports: [],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss'
})
export class SearchComponent implements OnInit {
  @ViewChild('peopleCheckbox')
  peopleCheckbox!: ElementRef;

  private searchSubject = new Subject<string>(); //Observable
  results: { name: string; resources: string }[] = [];
  
  constructor(private apiService: ApiService) {}

  ngOnInit() {
    // Subscribe to searchSubject to trigger API calls
    this.searchSubject
      .pipe(
        debounceTime(300), // Wait 300ms after the last keystroke
        distinctUntilChanged(), // Only proceed if the value has changed
        switchMap((query:string) => this.isPeopleChecked ? this.apiService.filterCharacters(query) : this.apiService.filterFilms(query))
      )
      .subscribe({
        next: (data) => {
          this.results = data;
        },
        error: (error) => {
          console.error('Error fetching search results:', error);
        },
        complete: () => {
          console.log('Search completed');
        }
      });
  }

    // Triggered by input event in the template
  onSearch(event: Event) {
    const input = (event.target as HTMLInputElement).value;
    this.searchSubject.next(input); // Push the input to the Subject
  }

  get isPeopleChecked(): boolean {
    return this.peopleCheckbox.nativeElement.checked;
  }
}
