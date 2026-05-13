import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { AdminUsersComponent } from './pages/admin-users/admin-users.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { RootRedirectComponent } from './pages/root-redirect.component';
import { SaleDetailComponent } from './pages/sale-detail/sale-detail.component';
import { SaleFormComponent } from './pages/sale-form/sale-form.component';
import { SalesListComponent } from './pages/sales-list/sales-list.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', component: RootRedirectComponent },
  { path: 'login', component: LoginComponent },
  { path: 'cadastro', component: RegisterComponent },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      { path: 'sales', component: SalesListComponent },
      { path: 'sales/new', component: SaleFormComponent },
      { path: 'sales/:id/edit', component: SaleFormComponent },
      { path: 'sales/:id', component: SaleDetailComponent },
      { path: 'usuarios', component: AdminUsersComponent },
    ],
  },
  { path: '**', redirectTo: '' },
];
