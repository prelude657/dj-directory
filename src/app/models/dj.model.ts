export interface DJProfile {
  id: string;
  stageName: string;
  realName?: string;
  tagline: string;
  bio: string;
  location: string;
  city: string;
  country: string;
  profileImageUrl: string;
  bannerImageUrl?: string;
  genres: string[];
  equipment: string[];
  hourlyRate?: number;
  featured: boolean;
  rating: number;
  reviewCount: number;
  socialLinks?: {
    instagram?: string;
    soundcloud?: string;
    youtube?: string;
    spotify?: string;
  };
}