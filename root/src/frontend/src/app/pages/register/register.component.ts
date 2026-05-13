import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ApiError } from '../../core/api-error';
import type { DocumentedUserRegistration } from '../../models/auth.model';
import { AuthApiService } from '../../services/auth-api.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent implements OnInit {
  private readonly authApi = inject(AuthApiService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  username = '';
  email = '';
  phone = '';
  password = '';
  confirm = '';
  roleStr: 'Customer' | 'Manager' | 'Admin' = 'Customer';
  firstname = '';
  lastname = '';
  city = '';
  street = '';
  number = 0;
  zipcode = '';
  lat = '';
  long = '';

  showPassword = false;
  showConfirm = false;

  readonly error = signal<string | null>(null);
  readonly loading = signal(false);

  private readonly phonePattern = /^\+?[1-9]\d{1,14}$/;

  ngOnInit(): void {
    if (this.auth.isAuthenticated()) {
      void this.router.navigateByUrl('/sales', { replaceUrl: true });
    }
  }

  clearForm(): void {
    this.error.set(null);
    this.username = '';
    this.email = '';
    this.phone = '';
    this.password = '';
    this.confirm = '';
    this.roleStr = 'Customer';
    this.firstname = '';
    this.lastname = '';
    this.city = '';
    this.street = '';
    this.number = 0;
    this.zipcode = '';
    this.lat = '';
    this.long = '';
    this.showPassword = false;
    this.showConfirm = false;
  }

  async submit(): Promise<void> {
    this.error.set(null);
    const u = this.username.trim();
    if (u.length < 3 || u.length > 50) {
      this.error.set('Nome de usuário: entre 3 e 50 caracteres');
      return;
    }
    if (this.password.length < 6) {
      this.error.set('Senha com no mínimo 6 caracteres');
      return;
    }
    if (this.password !== this.confirm) {
      this.error.set('As senhas não coincidem');
      return;
    }
    const ph = this.phone.trim().replace(/\s/g, '');
    if (!this.phonePattern.test(ph)) {
      this.error.set(
        'Telefone inválido. Use formato internacional, ex.: +5511999999999'
      );
      return;
    }

    const body: DocumentedUserRegistration = {
      email: this.email.trim(),
      username: u,
      password: this.password,
      name: {
        firstname: this.firstname.trim(),
        lastname: this.lastname.trim(),
      },
      address: {
        city: this.city.trim(),
        street: this.street.trim(),
        number: Number(this.number) || 0,
        zipcode: this.zipcode.trim(),
        geolocation: { lat: this.lat, long: this.long },
      },
      phone: ph,
      status: 'Active',
      role: this.roleStr,
    };

    this.loading.set(true);
    try {
      await firstValueFrom(this.authApi.registerUser(body));
      await this.router.navigateByUrl('/login?registered=1', {
        replaceUrl: true,
      });
    } catch (err) {
      this.error.set(
        err instanceof ApiError
          ? err.message
          : 'Não foi possível concluir o cadastro.'
      );
    } finally {
      this.loading.set(false);
    }
  }
}
