import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { Incidencia, PanelResumen, TEXTO_TIPO } from '../../core/modelos';
import { textoError } from '../../core/errores';
import { Etiqueta } from '../../compartido/etiqueta';

/** Pantalla de inicio: resumen y lo que requiere atención hoy. */
@Component({
  selector: 'app-panel',
  imports: [RouterLink, DatePipe, Etiqueta],
  template: `
    <div class="cabecera">
      <div>
        <h1>Panel</h1>
        <p class="subtitulo">Resumen del mantenimiento de elevadores</p>
      </div>
    </div>

    @if (error()) { <div class="aviso aviso--error">{{ error() }}</div> }

    @if (resumen(); as r) {
      <section class="kpis">
        <a class="kpi" routerLink="/clientes">
          <span class="kpi__valor">{{ r.totalClientes }}</span>
          <span class="kpi__texto">Clientes</span>
        </a>
        <a class="kpi" routerLink="/elevadores">
          <span class="kpi__valor">{{ r.totalElevadores }}</span>
          <span class="kpi__texto">Elevadores</span>
        </a>
        <a class="kpi" [class.kpi--alerta]="r.incidenciasAbiertas > 0" routerLink="/incidencias">
          <span class="kpi__valor">{{ r.incidenciasAbiertas }}</span>
          <span class="kpi__texto">Incidencias abiertas</span>
          @if (r.incidenciasUrgentes) { <small>{{ r.incidenciasUrgentes }} urgente(s)</small> }
        </a>
        <a class="kpi" [class.kpi--alerta]="r.revisionesVencidas > 0" routerLink="/elevadores" [queryParams]="{ vencidos: 1 }">
          <span class="kpi__valor">{{ r.revisionesVencidas }}</span>
          <span class="kpi__texto">Revisiones vencidas</span>
          @if (r.revisionesProximos30Dias) { <small>{{ r.revisionesProximos30Dias }} en los próximos 30 días</small> }
        </a>
      </section>

      <div class="columnas">
        <section class="tarjeta">
          <h2>Próximas revisiones</h2>
          @if (r.proximasRevisiones.length) {
            <div class="tabla-scroll">
              <table class="tabla">
                <thead><tr><th>Elevador</th><th>Taller</th><th>Fecha</th><th></th></tr></thead>
                <tbody>
                  @for (e of r.proximasRevisiones; track e.id) {
                    <tr>
                      <td><a [routerLink]="['/elevadores', e.id]">{{ e.marca }} {{ e.modelo }}</a><br><small>{{ tipos[e.tipo] }}</small></td>
                      <td>{{ e.nombreTaller }}</td>
                      <td>{{ e.proximaRevision | date: 'dd/MM/yyyy' }}</td>
                      <td><app-etiqueta [valor]="e.revisionVencida ? 'Vencida' : 'AlDia'" /></td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          } @else { <p class="vacio">No hay elevadores registrados.</p> }
        </section>

        <section class="tarjeta">
          <h2>Incidencias pendientes</h2>
          @for (i of pendientes(); track i.id) {
            <a class="incidencia" [routerLink]="['/elevadores', i.elevadorId]">
              <div>
                <strong>{{ i.titulo }}</strong>
                <small>{{ i.elevadorNombre }} · {{ i.nombreTaller }}</small>
              </div>
              <app-etiqueta [valor]="i.prioridad" />
            </a>
          } @empty { <p class="vacio">No hay incidencias pendientes. 👍</p> }
        </section>
      </div>
    } @else if (!error()) {
      <p class="cargando">Cargando…</p>
    }
  `,
  styles: `
    .kpis { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 1rem; margin-bottom: 1.5rem; }
    .kpi {
      background: var(--tarjeta); border: 1px solid var(--borde); border-radius: var(--radio);
      padding: 1.1rem 1.25rem; display: flex; flex-direction: column; color: var(--texto); text-decoration: none !important;
      transition: transform .1s, box-shadow .1s;
      &:hover { transform: translateY(-2px); box-shadow: 0 4px 14px rgba(0,0,0,.06); }
      &__valor { font-size: 2rem; font-weight: 700; }
      &__texto { color: var(--suave); }
      small { color: var(--aviso); margin-top: .3rem; font-weight: 600; }
      &--alerta { border-left: 4px solid var(--peligro); }
    }
    .columnas { display: grid; grid-template-columns: 3fr 2fr; gap: 1.5rem; }
    .columnas .tarjeta { margin: 0; }
    .incidencia {
      display: flex; justify-content: space-between; align-items: center; gap: 1rem;
      padding: .7rem 0; border-bottom: 1px solid var(--borde); color: var(--texto); text-decoration: none !important;
      small { display: block; color: var(--suave); }
      &:last-child { border-bottom: 0; }
    }
    @media (max-width: 900px) { .columnas { grid-template-columns: 1fr; } }
  `,
})
export class Panel implements OnInit {
  private readonly api = inject(ApiService);

  protected readonly resumen = signal<PanelResumen | null>(null);
  protected readonly incidencias = signal<Incidencia[]>([]);
  protected readonly error = signal('');
  protected readonly tipos = TEXTO_TIPO;

  // Solo las no cerradas (la API ya las devuelve ordenadas por urgencia).
  protected readonly pendientes = computed(() =>
    this.incidencias().filter(i => i.estado !== 'Cerrada').slice(0, 6));

  ngOnInit(): void {
    this.api.panel().subscribe({
      next: r => this.resumen.set(r),
      error: e => this.error.set(textoError(e)),
    });
    this.api.incidencias().subscribe({ next: l => this.incidencias.set(l) });
  }
}
