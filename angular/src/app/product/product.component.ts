import { AuthService } from '@abp/ng.core';
import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.scss'],
})
export class ProductComponent {
  blockedPanel: boolean = false;
  items = [];
  get hasLoggedIn(): boolean {
    return this.authService.isAuthenticated;
  }

  constructor(private authService: AuthService) {}

  login() {
    this.authService.navigateToLogin();
  }
}
