import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { DJProfile } from '../models/dj.model';

@Injectable({
  providedIn: 'root'
})
export class DjService {
  private apiUrl = 'http://localhost:5275/api/DjProfiles';

  constructor(private http: HttpClient) {}

  // 1. Fetch all DJs from your .NET API
  getAllDjs(): Observable<DJProfile[]> {
    return this.http.get<DJProfile[]>(this.apiUrl);
  }

  // 2. Filter featured DJs from API results
  getFeaturedDjs(): Observable<DJProfile[]> {
    return this.getAllDjs().pipe(
      map((djs) => djs.filter((dj) => dj.featured))
    );
  }

  // 3. Dynamically extract all unique genres across all DJs from API
  getAllGenres(): Observable<string[]> {
    return this.getAllDjs().pipe(
      map((djs) => {
        const genresSet = new Set<string>();
        djs.forEach((dj) => {
          if (dj.genres && Array.isArray(dj.genres)) {
            dj.genres.forEach((g) => genresSet.add(g));
          }
        });
        return Array.from(genresSet);
      })
    );
  }

  // 4. Fetch a single DJ by ID
  getDjById(id: string): Observable<DJProfile> {
    return this.http.get<DJProfile>(`${this.apiUrl}/${id}`);
  }

  // 5. Create a new DJ Profile in SQLite via POST
  createDj(dj: DJProfile): Observable<DJProfile> {
    return this.http.post<DJProfile>(this.apiUrl, dj);
  }
}