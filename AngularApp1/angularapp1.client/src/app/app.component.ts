import { Component, inject } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: false
})
export class AppComponent {
  authService = inject(AuthService);
  private router = inject(Router);

  viewTitle = '';

  private readonly routeTitles: Record<string, string> = {
    '/cashier': 'Panel de Cajero',
    '/manager': 'Escritorio de Gerente',
    '/admin/companies': 'Administración › Empresas',
    '/admin/stores': 'Administración › Puntos de Venta',
    '/admin/banks': 'Administración › Bancos',
    '/admin/users': 'Administración › Usuarios',
    '/login': 'Acceso al Sistema'
  };

  get isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  get currentUser() {
    return this.authService.getCurrentUser();
  }

  get userRole(): string {
    return this.currentUser?.roleName ?? '';
  }

  get isAdmin(): boolean {
    return this.authService.hasRole('Administrador');
  }

  get isManagerOrAbove(): boolean {
    return this.authService.hasAnyRole(['Gerente Sucursal', 'Administrador']);
  }

  get isCashier(): boolean {
    return this.authService.hasRole('Cajero');
  }

  ngOnInit() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.viewTitle = this.routeTitles[event.urlAfterRedirects] ?? '';
    });
  }

  logout() {
    this.authService.logout();
  }
}
