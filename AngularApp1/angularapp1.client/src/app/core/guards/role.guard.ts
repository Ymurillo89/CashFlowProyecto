import { Injectable, inject } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  private authService = inject(AuthService);
  private router = inject(Router);

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const allowedRoles: string[] = route.data['roles'] || [];

    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login']);
      return false;
    }

    if (allowedRoles.length === 0 || this.authService.hasAnyRole(allowedRoles)) {
      return true;
    }

    // Redirect to the user's default route based on their role
    this.router.navigate([this.authService.getDefaultRouteForRole()]);
    return false;
  }
}
