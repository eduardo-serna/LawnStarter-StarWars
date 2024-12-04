import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, map, Observable, of, tap } from "rxjs";

@Injectable({
    providedIn: 'root'
  })
export class ApiService {
  apiUrlPeople = 'http://localhost:5002/get';
  apiUrlFilm = 'http://localhost:5003/get';
  private cacheCharacters: { name: string; resources: string }[] | null = null; // Store all cached data
  private cacheFilms: { name: string; resources: string }[] | null = null; // Store all cached data

  // Shared state for filtered results
  private filteredResultsSubject = new BehaviorSubject<{ name: string; resources: string }[]>([]);
  filteredResults$ = this.filteredResultsSubject.asObservable(); // Observable for other components

  constructor(private http: HttpClient) { }

  getAllCharacters(): Observable<any[]> {
    const apiUrl = this.apiUrlPeople; // THIS CAN BE HANDLE BETTER IN A ENVIROMENT VARIABLE
    if (this.cacheCharacters) {
      // If cache is available, return it as an Observable
      return of(this.cacheCharacters);
    }

    return this.http.get<any[]>(`${apiUrl}`).pipe(
        map((response) =>  response.map((item) => ({
            name: item.name,
            resources: item.films
          }))
        ),
        tap((data) => (this.cacheCharacters = data)) // Cache the data
      );
  }

  getAllFilms(): Observable<any[]> {
    const apiUrl = this.apiUrlFilm; // THIS CAN BE HANDLE BETTER IN A ENVIROMENT VARIABLE
    if (this.cacheFilms) {
      // If cache is available, return it as an Observable
      return of(this.cacheFilms);
    }

    return this.http.get<any[]>(`${apiUrl}`).pipe(
        map((response) =>  response.map((item) => ({
            name: item.title,
            resources: item.films
          }))
        ),
        tap((data) => (this.cacheFilms = data)) // Cache the data
      );
  }

  /**
   * Filters the Characters cached data based on the query.
   * If the cache is empty, it fetches data from the API first.
   */
  filterCharacters(query: string): Observable<{ name: string; resources: string; }[]> {
    if (this.cacheCharacters) {
      var filteredResults: {
        name: string;
        resources: string;
      }[] = [];
      
      // Perform filtering on the cache
      filteredResults = query.trim() === '' ? filteredResults : this.cacheCharacters.filter((item) =>
        item.name.toLowerCase().includes(query.toLowerCase())
      );
      this.filteredResultsSubject.next(filteredResults); // Update the shared state
      return of(filteredResults);
    }

    return this.getAllCharacters().pipe(
      map((data) =>
        data.filter((item) => item.name.toLowerCase().includes(query.toLowerCase()))
      ),
      tap((filteredResults) => this.filteredResultsSubject.next(filteredResults)) // Update the shared state
    );
  }

    /**
   * Filters the Characters cached data based on the query.
   * If the cache is empty, it fetches data from the API first.
   */
    filterFilms(query: string): Observable<{ name: string; resources: string; }[]> {
      if (this.cacheFilms) {
        var filteredResults: {
          name: string;
          resources: string;
        }[] = [];
        
        // Perform filtering on the cache
        filteredResults = query.trim() === '' ? filteredResults : this.cacheFilms.filter((item) =>
          item.name.toLowerCase().includes(query.toLowerCase())
        );
        this.filteredResultsSubject.next(filteredResults); // Update the shared state
        return of(filteredResults);
      }
  
      return this.getAllFilms().pipe(
        map((data) =>
          data.filter((item) => item.name.toLowerCase().includes(query.toLowerCase()))
        ),
        tap((filteredResults) => this.filteredResultsSubject.next(filteredResults)) // Update the shared state
      );
    }
}