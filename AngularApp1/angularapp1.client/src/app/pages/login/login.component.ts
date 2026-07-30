import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  standalone: false
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private messageService = inject(MessageService);

  loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(1)]]
  });

  isLoading = false;
  showPassword = false;
  sessionExpired = false;

  ngOnInit() {
    // If already logged in, redirect
    if (this.authService.isLoggedIn()) {
      this.router.navigate([this.authService.getDefaultRouteForRole()]);
      return;
    }
    // Check if redirected due to session expiry
    this.route.queryParams.subscribe(params => {
      this.sessionExpired = params['reason'] === 'session_expired';
    });
  }

  onSubmit() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.authService.login(this.loginForm.value).subscribe({
      next: (response) => {
        this.isLoading = false;
        this.messageService.add({
          severity: 'success',
          summary: 'Bienvenido',
          detail: `Sesión iniciada como ${response.fullName}`
        });
        this.router.navigate([this.authService.getDefaultRouteForRole()]);
      },
      error: (err) => {
        this.isLoading = false;
        const msg = err.error?.message || 'Credenciales incorrectas. Inténtelo de nuevo.';
        this.messageService.add({
          severity: 'error',
          summary: 'Acceso Denegado',
          detail: msg
        });
      }
    });
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  get emailControl() { return this.loginForm.get('email'); }
  get passwordControl() { return this.loginForm.get('password'); }
}
