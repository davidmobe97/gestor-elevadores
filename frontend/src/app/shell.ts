import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth.service';

/** Estructura de las pantallas con sesión: menú lateral + página actual. */
@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  protected readonly auth = inject(AuthService);

  protected readonly menu = [
    { ruta: '/panel', texto: 'Panel', icono: '▦' },
    { ruta: '/clientes', texto: 'Clientes', icono: '⌂' },
    { ruta: '/elevadores', texto: 'Elevadores', icono: '⇕' },
    { ruta: '/incidencias', texto: 'Incidencias', icono: '⚠' },
  ];
}
