import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CashierComponent } from './pages/cashier/cashier.component';
import { ManagerComponent } from './pages/manager/manager.component';
import { CompanyComponent } from './pages/company/company.component';
import { StoreComponent } from './pages/store/store.component';
import { BankComponent } from './pages/bank/bank.component';
import { UserComponent } from './pages/user/user.component';
import { LoginComponent } from './pages/login/login.component';
import { AuthGuard } from './core/guards/auth.guard';
import { RoleGuard } from './core/guards/role.guard';

const routes: Routes = [
  // Public route
  { path: 'login', component: LoginComponent },

  // Cashier: Cajero, Gerente Sucursal, Administrador
  {
    path: 'cashier',
    component: CashierComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Cajero', 'Gerente Sucursal', 'Administrador'] }
  },

  // Manager: Gerente Sucursal, Administrador
  {
    path: 'manager',
    component: ManagerComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Gerente Sucursal', 'Administrador'] }
  },

  // Admin routes: Administrador only
  {
    path: 'admin/companies',
    component: CompanyComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Administrador'] }
  },
  {
    path: 'admin/stores',
    component: StoreComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Administrador'] }
  },
  {
    path: 'admin/banks',
    component: BankComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Administrador'] }
  },
  {
    path: 'admin/users',
    component: UserComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Administrador'] }
  },

  // Default: redirect to login
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: '**', redirectTo: '/login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
