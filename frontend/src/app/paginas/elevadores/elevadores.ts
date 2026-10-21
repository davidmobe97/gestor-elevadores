import { Component, effect, inject, input, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ElevadorResumen, TEXTO_TIPO } from '../../core/modelos';
import { textoError } from '../../core/errores';
import { Etiqueta } from '../../compartido/etiqueta';

/** Todos los elevadores, ordenados por la fecha de su próxima revisión. */
@Component({
  selector: 'app-elevadores',
  imports: [RouterLink, DatePipe, Etiqueta],
  template: `
    <div class="cabecera">
      <div>
        <h1>Elevadores</h1>
        <p class="subtitulo">Ordenados por la fecha de la próxima revisión</p>
      </div>
      <label class="filtro">
        <input type="checkbox" [checked]="soloVencidos()" (change)="cambiarFiltro($any($event.target).checked)">
        Solo con la revisión vencida
      </label>
    </div>

    @if (error()) { <div class="aviso aviso--error">{{ error() }}</div> }

    <div class="tarjeta">
      @if (cargando()) {
        <p class="cargando">Cargando…</p>
      } @else {
        <div class="tabla-scroll">
          <table class="tabla">
            <thead><tr><th>Elevador</th><th>Taller</th><th>Tipo</th><th>Próxima revisión</th><th>Incidencias</th></tr></thead>
            <tbody>
              @for (e of elevadores(); track e.id) {
                <tr>
                  <td><a [routerLink]="['/elevadores', e.id]"><strong>{{ e.marca }} {{ e.modelo }}</strong></a><br><small>Nº {{ e.numeroSerie }}</small></td>
                  <td><a [routerLink]="['/clientes', e.clienteId]">{{ e.nombreTaller }}</a></td>
                  <td>{{ tipos[e.tipo] }}</td>
                  <td>{{ e.proximaRevision | date: 'dd/MM/yyyy' }} <app-etiqueta [valor]="e.revisionVencida ? 'Vencida' : 'AlDia'" /></td>
                  <td>{{ e.incidenciasAbiertas ? e.incidenciasAbiertas + ' abierta(s)' : '—' }}</td>
                </tr>
              } @empty {
                <tr><td colspan="5" class="vacio">No hay elevadores con ese filtro.</td></tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>
  `,
  styles: `.filtro { display: flex; gap: .5rem; align-items: center; cursor: pointer; font-weight: 500; }`,
})
export class Elevadores {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  /** ?vencidos=1 en la URL (por ejemplo, desde el panel). */
  readonly vencidos = input<string>();

  protected readonly elevadores = signal<ElevadorResumen[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');
  protected readonly soloVencidos = signal(false);
  protected readonly tipos = TEXTO_TIPO;

  constructor() {
    // Se vuelve a ejecutar cada vez que cambia el parámetro de la URL.
    effect(() => {
      const vencidos = this.vencidos() === '1';
      this.soloVencidos.set(vencidos);
      this.cargar(vencidos);
    });
  }

  protected cambiarFiltro(marcado: boolean): void {
    this.router.navigate([], { queryParams: { vencidos: marcado ? 1 : null } });
  }

  private cargar(soloVencidos: boolean): void {
    this.cargando.set(true);
    this.api.elevadores({ soloVencidos }).subscribe({
      next: l => { this.elevadores.set(l); this.cargando.set(false); },
      error: e => { this.error.set(textoError(e)); this.cargando.set(false); },
    });
  }
}
