import { Component, OnInit } from '@angular/core';

interface Badge {
  id: number;
  name: string;
  description: string;
  iconClass: string;
  earned: Date;
  rarity: 'common' | 'rare' | 'epic' | 'legendary';
}

@Component({
  selector: 'app-badge-showcase',
  template: `
    <div class="badges-container">
      <div *ngFor="let badge of recentBadges" class="badge-card" [class]="badge.rarity">
        <div class="badge-icon">
          <i [class]="badge.iconClass"></i>
        </div>
        <div class="badge-info">
          <h4>{{badge.name}}</h4>
          <p>{{badge.description}}</p>
          <span class="earned-date">Earned {{badge.earned | date:'mediumDate'}}</span>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .badges-container {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
    }

    .badge-card {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 16px;
      border-radius: 8px;
      background: white;
      transition: transform 0.2s ease;
    }

    .badge-card:hover {
      transform: translateY(-2px);
    }

    .badge-icon {
      width: 48px;
      height: 48px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 50%;
      font-size: 24px;
    }

    .badge-info {
      flex: 1;
    }

    h4 {
      margin: 0 0 4px 0;
      color: #333;
    }

    p {
      margin: 0 0 8px 0;
      font-size: 0.9em;
      color: #666;
    }

    .earned-date {
      font-size: 0.8em;
      color: #888;
    }

    /* Rarity styles */
    .common .badge-icon {
      background: #E0E0E0;
      color: #757575;
    }

    .rare .badge-icon {
      background: #2196F3;
      color: white;
    }

    .epic .badge-icon {
      background: #9C27B0;
      color: white;
    }

    .legendary .badge-icon {
      background: #FFD700;
      color: #333;
    }
  `]
})
export class BadgeShowcaseComponent implements OnInit {
  recentBadges: Badge[] = [];

  constructor() { }

  ngOnInit(): void {
    // This will be replaced with actual API call
    this.recentBadges = [
      {
        id: 1,
        name: 'Early Bird',
        description: 'Complete 5 workouts before 8 AM',
        iconClass: 'fas fa-sun',
        earned: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
        rarity: 'rare'
      },
      {
        id: 2,
        name: 'Marathon Ready',
        description: 'Run 42.2 km in a single month',
        iconClass: 'fas fa-running',
        earned: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000),
        rarity: 'epic'
      },
      {
        id: 3,
        name: 'Consistency King',
        description: 'Maintain a 7-day streak',
        iconClass: 'fas fa-crown',
        earned: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000),
        rarity: 'common'
      }
    ];
  }
}
