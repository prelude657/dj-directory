import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { DjService } from '../../services/dj.service';
import { DJProfile } from '../../models/dj.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  private djService = inject(DjService);
  private router = inject(Router);

  featuredDjs: DJProfile[] = [];
  allDjs: DJProfile[] = [];
  filteredDjs: DJProfile[] = [];
  genres: string[] = [];
  searchQuery = '';
  selectedGenre = '';
  loading = true;

  get displayedDjs(): DJProfile[] {
    return (this.searchQuery || this.selectedGenre) ? this.filteredDjs : this.featuredDjs;
  }

  ngOnInit(): void {
    // 1. Fetch featured DJs
    this.djService.getFeaturedDjs().subscribe({
      next: (djs) => {
        this.featuredDjs = djs;
      },
      error: (err) => {
        console.error('Error loading featured DJs:', err);
      }
    });

    // 2. Fetch all DJs for searching & filtering
    this.djService.getAllDjs().subscribe({
      next: (data) => {
        console.log('API Response received:', data);
        this.allDjs = data;
        this.filteredDjs = data;
        this.loading = false;
      },
      error: (err) => {
        console.error('API Error:', err);
        this.loading = false;
      }
    });

    // 3. Fetch genres dropdown list
    this.djService.getAllGenres().subscribe({
      next: (genres) => this.genres = genres,
      error: (err) => console.error('Error loading genres:', err)
    });
  }

  applyFilter(): void {
    const query = this.searchQuery.toLowerCase().trim();

    this.filteredDjs = this.allDjs.filter(dj => {
      const matchesSearch = !query || 
        dj.stageName.toLowerCase().includes(query) ||
        dj.tagline.toLowerCase().includes(query) ||
        dj.location.toLowerCase().includes(query) ||
        (dj.city && dj.city.toLowerCase().includes(query));

      const matchesGenre = !this.selectedGenre || 
        (dj.genres && dj.genres.includes(this.selectedGenre));

      return matchesSearch && matchesGenre;
    });
  }

  selectQuickGenre(genre: string): void {
    this.selectedGenre = this.selectedGenre === genre ? '' : genre;
    this.applyFilter();
  }

  resetFilters(): void {
    this.searchQuery = '';
    this.selectedGenre = '';
    this.filteredDjs = this.allDjs;
  }

  onSearch(): void {
    this.router.navigate(['/directory'], {
      queryParams: {
        q: this.searchQuery || null,
        genre: this.selectedGenre || null
      }
    });
  }
}