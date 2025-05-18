import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { AuthService, User } from '../../services/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent implements OnInit {
  currentUserValue: User | null = null;
  constructor(
    public router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.currentUserValue = this.authService.currentUserValue;
    this.authService.currentUser$.subscribe(user => {
      this.currentUserValue = user;
    });
  }
  
  logout() {
    this.authService.logout();
  }

  isGatePersonnel(user: User | null): boolean {
    if (!user) return false;
    return this.authService.isGatePersonnel(user);
  }

  isHr(user: User | null): boolean {
    if (!user) return false;
    return this.authService.isHr(user);
  }

  isManager(user: User | null): boolean {
    if (!user) return false;
    return this.authService.isManager(user);
  }

}
