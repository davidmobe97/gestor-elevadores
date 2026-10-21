import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

export type Rol = 'Administrador' | 'Tecnico';

export interface Sesion {
  token: string;
  expira: string;
  nombre: string;
  email: string;
  rol: Rol;
}

const CLAVE = 'gestor-elevadores.sesion';

/**
 * Guarda la sesión (token JWT + datos del usuario) en un signal y en localStorage,
 * para que siga abierta al recargar la página.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly sesion = signal<Sesion | null>(this.leerGuardada());

  readonly usuario = this.sesion.asReadonly();
  readonly autenticado = computed(() => this.sesion() !== null);
  readonly esAdmin = computed(() => this.sesion()?.rol === 'Administrador');
  readonly token = computed(() => this.sesion()?.token ?? null);

  login(email: string, password: string): Observable<Sesion> {
    return this.http.post<Sesion>(`${environment.apiUrl}/auth/login`, { email, password }).pipe(
      tap(s => {
        this.sesion.set(s);
        localStorage.setItem(CLAVE, JSON.stringify(s));
      }),
    );
  }

  logout(volverA?: string): void {
    this.sesion.set(null);
    localStorage.removeItem(CLAVE);
    this.router.navigate(['/login'], { queryParams: volverA ? { volver: volverA } : {} });
  }

  /** Recupera la sesión guardada, descartándola si el token ya ha caducado. */
  private leerGuardada(): Sesion | null {
    try {
      const s = JSON.parse(localStorage.getItem(CLAVE) ?? 'null') as Sesion | null;
      if (s && new Date(s.expira) > new Date()) return s;
    } catch { /* datos corruptos: se ignoran */ }
    localStorage.removeItem(CLAVE);
    return null;
  }
}
