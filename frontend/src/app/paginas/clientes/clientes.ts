import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Subject, debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ApiService } from '../../core/api.service';
import { ClienteResumen } from '../../core/modelos';
import { textoError } from '../../core/errores';

/** Listado de talleres con buscador. */
@Component({
  selector: 'app-clientes',
  imports: [RouterLink],
  template: `
    <div class="cabecera">
      <div>
        <h1>Clientes</h1>
        <p class="subtitulo">Talleres con elevadores instalados</p>
      </div>
      <a class="boton" routerLink="/clientes/nuevo">+ Nuevo cliente</a>
    </div>

    <div class="tarjeta">
      <input class="buscador" type="search" placeholder="Buscar por nombre o población…"
             (input)="buscar$.next($any($event.target).value)" aria-label="Buscar cliente">

      @if (error()) { <div class="aviso aviso--error">{{ error() }}</div> }

      @if (cargando()) {
        <p class="cargando">Cargando…</p>
      } @else {
        <div class="tabla-scroll">
          <table class="tabla">
            <thead><tr><th>Taller</th><th>Población</th><th>Teléfono</th><th>Elevadores</th></tr></thead>
            <tbody>
              @for (c of clientes(); track c.id) {
                <tr>
                  <td><a [routerLink]="['/clientes', c.id]"><strong>{{ c.nombreTaller }}</strong></a></td>
                  <td>{{ c.poblacion }} ({{ c.provincia }})</td>
                  <td>{{ c.telefono || '—' }}</td>
                  <td>{{ c.numeroElevadores }}</td>
                </tr>
              } @empty {
                <tr><td colspan="4" class="vacio">No hay clientes que coincidan con la búsqueda.</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
  styles: `
    .buscador {
      width: 100%; max-width: 380px; font: inherit; padding: .6rem .8rem;
      border: 1px solid var(--borde); border-radius: 8px; margin-bottom: 1rem;
    }
  `,
})
export class Clientes implements OnInit {
  private readonly api = inject(ApiService);

  protected readonly clientes = signal<ClienteResumen[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');

  /** Cada tecla pulsada entra aquí; se espera 300 ms antes de llamar a la API. */
  protected readonly buscar$ = new Subject<string>();

  constructor() {
    this.buscar$.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(texto => this.api.clientes(texto.trim())), // cancela la búsqueda anterior si llega otra
      takeUntilDestroyed(),
    ).subscribe({
      next: l => this.clientes.set(l),
      error: e => this.error.set(textoError(e)),
    });
  }

  ngOnInit(): void {
    this.api.clientes().subscribe({
      next: l => { this.clientes.set(l); this.cargando.set(false); },
      error: e => { this.error.set(textoError(e)); this.cargando.set(false); },
    });
  }
}
