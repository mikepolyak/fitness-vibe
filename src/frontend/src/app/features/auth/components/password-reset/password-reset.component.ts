import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html',
  styleUrls: ['./password-reset.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PasswordResetComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void { }

  navigateToLogin(): void {
    this.router.navigate(['/auth/login']);
  }
}
