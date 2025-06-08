import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import { User } from '../../models/user.model';

/**
 * User Avatar Component - the visual representation of a fitness community member.
 * Think of this as a customizable profile picture that shows a user's identity
 * and fitness journey progress throughout the app.
 */
@Component({
  selector: 'fv-user-avatar',
  templateUrl: './user-avatar.component.html',
  styleUrls: ['./user-avatar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserAvatarComponent {
  /**
   * User object containing avatar and profile information.
   * Like the member's profile card with photo and details.
   */
  @Input() user: User | null = null;
  
  /**
   * Size of the avatar - like choosing between a small ID photo
   * and a large profile portrait.
   */
  @Input() size: 'xs' | 'sm' | 'md' | 'lg' | 'xl' = 'md';
  
  /**
   * Whether to show the user's level badge on the avatar.
   * Like displaying rank insignia on a uniform.
   */
  @Input() showLevel: boolean = false;
  
  /**
   * Whether to show online status indicator.
   * Like a green light showing someone is currently at the gym.
   */
  @Input() showOnlineStatus: boolean = false;
  
  /**
   * Whether the user is currently online.
   */
  @Input() isOnline: boolean = false;
  
  /**
   * Custom CSS classes to apply.
   */
  @Input() customClass: string = '';
  
  /**
   * Whether the avatar should be clickable.
   * Like making the profile picture a button to view full profile.
   */
  @Input() clickable: boolean = false;
  
  /**
   * Alt text for accessibility.
   */
  @Input() alt: string = '';

  /**
   * Get the display name for the user.
   * Like reading the name tag on a member's profile.
   */
  get displayName(): string {
    if (!this.user) return 'Unknown User';
    return `${this.user.firstName} ${this.user.lastName}`;
  }

  /**
   * Get initials for fallback when no avatar image is available.
   * Like creating a monogram when there's no photo.
   */
  get initials(): string {
    if (!this.user) return 'U';
    
    const firstInitial = this.user.firstName?.charAt(0).toUpperCase() || '';
    const lastInitial = this.user.lastName?.charAt(0).toUpperCase() || '';
    
    return firstInitial + lastInitial || 'U';
  }

  /**
   * Check if user has a custom avatar image.
   * Like checking if someone has uploaded a profile photo.
   */
  get hasAvatarImage(): boolean {
    return !!(this.user?.avatarUrl && this.user.avatarUrl.trim());
  }

  /**
   * Get CSS classes for the avatar container.
   * Like choosing the right frame style and size for a photo.
   */
  get avatarClasses(): string {
    const classes = [
      'user-avatar',
      `user-avatar--${this.size}`,
      this.customClass
    ];
    
    if (this.clickable) {
      classes.push('user-avatar--clickable');
    }
    
    if (this.showLevel && this.user?.level) {
      classes.push('user-avatar--with-level');
    }
    
    if (this.showOnlineStatus) {
      classes.push('user-avatar--with-status');
    }
    
    return classes.filter(Boolean).join(' ');
  }

  /**
   * Get the level color based on user's fitness level.
   * Like choosing different colored badges for different experience levels.
   */
  get levelColor(): string {
    if (!this.user?.level) return '#gray';
    
    // Color progression based on level
    if (this.user.level < 5) return '#10b981'; // Green for beginners
    if (this.user.level < 15) return '#3b82f6'; // Blue for intermediate
    if (this.user.level < 30) return '#8b5cf6'; // Purple for advanced
    if (this.user.level < 50) return '#f59e0b'; // Orange for expert
    return '#ef4444'; // Red for legendary
  }

  /**
   * Get alt text for the avatar image.
   * Like providing a description for screen readers.
   */
  get avatarAlt(): string {
    if (this.alt) return this.alt;
    return `${this.displayName}'s profile picture`;
  }

  /**
   * Handle avatar click event.
   * Like responding when someone taps on a profile picture.
   */
  onAvatarClick(): void {
    if (this.clickable && this.user) {
      // Emit event or handle navigation
      // This would typically emit an event or navigate to user profile
      console.log('Avatar clicked for user:', this.user.id);
    }
  }

  /**
   * Handle image load error.
   * Like falling back to initials when a profile photo fails to load.
   */
  onImageError(): void {
    // Could implement fallback logic here
    console.log('Avatar image failed to load for user:', this.user?.id);
  }

  /**
   * Get fitness level icon based on user's level.
   * Like choosing the right achievement symbol.
   */
  get fitnessLevelIcon(): string {
    if (!this.user?.fitnessLevel) return 'fitness_center';
    
    switch (this.user.fitnessLevel) {
      case 'Beginner': return 'trending_up';
      case 'Intermediate': return 'local_fire_department';
      case 'Advanced': return 'military_tech';
      case 'Expert': return 'stars';
      default: return 'fitness_center';
    }
  }

  /**
   * Get the user's progress percentage to next level.
   * Like showing how close someone is to their next achievement.
   */
  get progressToNextLevel(): number {
    if (!this.user) return 0;
    
    const currentLevel = this.user.level;
    const currentXP = this.user.experiencePoints;
    
    // Calculate XP needed for current level and next level
    const currentLevelRequiredXP = currentLevel === 1 ? 0 : 
      Array.from({length: currentLevel - 1}, (_, i) => (i + 1) * 100)
        .reduce((sum, xp) => sum + xp, 0);
    
    const nextLevelRequiredXP = currentLevelRequiredXP + (currentLevel * 100);
    const progressInCurrentLevel = currentXP - currentLevelRequiredXP;
    const progressNeeded = currentLevel * 100;
    
    return Math.min(100, Math.max(0, (progressInCurrentLevel / progressNeeded) * 100));
  }
}
