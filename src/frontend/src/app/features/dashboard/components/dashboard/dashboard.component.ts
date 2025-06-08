import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  username = 'User'; // This will be replaced with actual user data

  constructor() { }

  ngOnInit(): void {
    // Will fetch user data and initialize dashboard
  }
}
