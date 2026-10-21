import { Component, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { textoError } from '../../core/errores';

/** Pantalla de inicio de sesión. */
@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],
  template: `
    <div class="fondo">
      <form class="caja" [formGroup]="form" (ngSubmit)="entrar()">
        <div class="marca"><span>⇕</span> Gestor de elevadores</div>
        <h1>Iniciar sesión</h1>

        @if (error()) { <div class="aviso aviso--error">{{ error() }}</div> }

        <div class="campo">
          <label for="email">Email</label>
          <input id="email" type="email" formControlName="email" autocomplete="username">
        </div>
        <div class="campo">
          <label for="pass">Contraseña</label>
          <input id="pass" type="password" formControlName="password" autocomplete="current-password">
        </div>

        <button class="boton" type="submit" [disabled]="form.invalid || entrando()">
          {{ entrando() ? 'Entrando…' : 'Entrar' }}
        </button>

        <details class="demo">
          <summary>Usuarios de demostración</summary>
          <button type="button" class="boton boton--secundario boton--pequeno" (click)="rellenar('admin@demo.local', 'Admin2026!')">Administrador</button>
          <button type="button" class="boton boton--secundario boton--pequeno" (click)="rellenar('tecnico@demo.local', 'Tecnico2026!')">Técnico</button>
        </details>
      </form>
    </div>
  `,
  styles: `
    .fondo { min-height: 100vh; display: grid; place-items: center; padding: 1rem;
             background: linear-gradient(135deg, #1c2128 0%, #24344d 100%); }
    .caja { background: #fff; border-radius: 14px; padding: 2rem; width: 100%; max-width: 380px;
            display: flex; flex-direction: column; gap: 1rem; box-shadow: 0 20px 50px rgba(0,0,0,.25); }
    .marca { display: flex; align-items: center; gap: .6rem; font-weight: 600; color: var(--suave);
             span { background: var(--primario); color: #fff; width: 32px; height: 32px; border-radius: 8px;
                    display: grid; place-items: center; } }
    h1 { margin: 0; }
    .boton[type=submit] { justify-content: center; padding: .7rem; }
    .demo { font-size: .85rem; color: var(--suave);
            summary { cursor: pointer; margin-bottom: .5rem; }
            .boton { margin-right: .4rem; } }
  `,
})
export class Login {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder).nonNullable;

  /** Página a la que volver después de entrar (?volver=/clientes/2). */
  readonly volver = input<string>();

  protected readonly entrando = signal(false);
  protected readonly error = signal('');

  protected readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
  });

  protected rellenar(email: string, password: string): void {
    this.form.setValue({ email, password });
  }

  protected entrar(): void {
    this.entrando.set(true);
    this.error.set('');
    const { email, password } = this.form.getRawValue();
    this.auth.login(email, password).subscribe({
      next: () => this.router.navigateByUrl(this.volver() || '/panel'),
      error: e => { this.error.set(textoError(e)); this.entrando.set(false); },
    });
  }
}
