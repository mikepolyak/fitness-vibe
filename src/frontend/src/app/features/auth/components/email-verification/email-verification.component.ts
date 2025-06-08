import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-email-verification',
  templateUrl: './email-verification.component.html',
  styleUrls: ['./email-verification.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmailVerificationComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void { }

  navigateToLogin(): void {
    this.router.navigate(['/auth/login']);
  }
}
