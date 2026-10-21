import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/** Raíz de la aplicación: solo muestra la ruta actual (login o Shell). */
@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  template: '<router-outlet />',
})
export class App {}
