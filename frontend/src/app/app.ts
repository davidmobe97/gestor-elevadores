import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

/** Estructura general: menú lateral + contenido de la página actual. */
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly menu = [
    { ruta: '/panel', texto: 'Panel', icono: '▦' },
    { ruta: '/clientes', texto: 'Clientes', icono: '⌂' },
    { ruta: '/elevadores', texto: 'Elevadores', icono: '⇕' },
    { ruta: '/incidencias', texto: 'Incidencias', icono: '⚠' },
  ];
}
