import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { EstadoIncidencia, Incidencia, TEXTO_ESTADO } from '../../core/modelos';
import { textoError } from '../../core/errores';
import { Etiqueta } from '../../compartido/etiqueta';
import { CerrarIncidencia } from '../../compartido/cerrar-incidencia';

/** Todas las incidencias, con filtro por estado. Las urgentes salen primero. */
@Component({
  selector: 'app-incidencias',
  imports: [RouterLink, DatePipe, Etiqueta, CerrarIncidencia],
  template: `
    <div class="cabecera">
      <div>
        <h1>Incidencias</h1>
        <p class="subtitulo">Averías y avisos de los clientes</p>
      </div>
    </div>

    <div class="pestanas" role="tablist">
      @for (f of filtros; track f.valor) {
        <button role="tab" [class.activa]="filtro() === f.valor" (click)="filtro.set(f.valor)">
          {{ f.texto }} <span>{{ contar(f.valor) }}</span>
        </button>
      }
    </div>

    @if (error()) { <div class="aviso aviso--error">{{ error() }}</div> }

    <div class="tarjeta">
      @if (cargando()) {
        <p class="cargando">Cargando…</p>
      } @else {
        @for (i of visibles(); track i.id) {
          <article class="item">
            <div class="item__cabecera">
              <div>
                <strong>{{ i.titulo }}</strong>
                <small><a [routerLink]="['/elevadores', i.elevadorId]">{{ i.elevadorNombre }}</a> · {{ i.nombreTaller }} · {{ i.fechaApertura | date: 'dd/MM/yyyy' }}</small>
              </div>
              <span><app-etiqueta [valor]="i.prioridad" /> <app-etiqueta [valor]="i.estado" /></span>
            </div>
            @if (i.estado === 'Cerrada') {
              <p class="solucion">✔ {{ i.solucion }}</p>
            } @else {
              <app-cerrar-incidencia [incidenciaId]="i.id" (cerrada)="cargar()" />
            }
          </article>
        } @empty { <p class="vacio">No hay incidencias en este estado.</p> }
      }
    </div>
  `,
  styles: `
    .pestanas { display: flex; gap: .4rem; margin-bottom: 1rem; flex-wrap: wrap; }
    .pestanas button {
      font: inherit; border: 1px solid var(--borde); background: #fff; border-radius: 999px;
      padding: .4rem .9rem; cursor: pointer; font-weight: 500;
      span { background: #eceff3; border-radius: 999px; padding: 0 .45rem; margin-left: .3rem; font-size: .8rem; }
      &.activa { background: var(--oscuro); color: #fff; border-color: var(--oscuro); span { background: rgba(255,255,255,.2); } }
    }
    .item {
      border-bottom: 1px solid var(--borde); padding: .9rem 0;
      &:last-child { border-bottom: 0; }
      &__cabecera { display: flex; justify-content: space-between; gap: 1rem; flex-wrap: wrap; }
      small { display: block; color: var(--suave); }
    }
    .solucion { color: var(--ok); margin: .4rem 0 0; }
  `,
})
export class Incidencias implements OnInit {
  private readonly api = inject(ApiService);

  protected readonly incidencias = signal<Incidencia[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal('');
  protected readonly filtro = signal<EstadoIncidencia | 'Pendientes' | 'Todas'>('Pendientes');

  protected readonly filtros: { valor: EstadoIncidencia | 'Pendientes' | 'Todas'; texto: string }[] = [
    { valor: 'Pendientes', texto: 'Pendientes' },
    { valor: 'Abierta', texto: TEXTO_ESTADO.Abierta },
    { valor: 'EnCurso', texto: TEXTO_ESTADO.EnCurso },
    { valor: 'Cerrada', texto: TEXTO_ESTADO.Cerrada },
    { valor: 'Todas', texto: 'Todas' },
  ];

  // Se cargan todas una vez y se filtran en el navegador (son pocas).
  protected readonly visibles = computed(() => this.aplicar(this.filtro()));

  ngOnInit(): void { this.cargar(); }

  cargar(): void {
    this.api.incidencias().subscribe({
      next: l => { this.incidencias.set(l); this.cargando.set(false); },
      error: e => { this.error.set(textoError(e)); this.cargando.set(false); },
    });
  }

  protected contar(f: EstadoIncidencia | 'Pendientes' | 'Todas'): number {
    return this.aplicar(f).length;
  }

  private aplicar(f: EstadoIncidencia | 'Pendientes' | 'Todas'): Incidencia[] {
    const todas = this.incidencias();
    if (f === 'Todas') return todas;
    if (f === 'Pendientes') return todas.filter(i => i.estado !== 'Cerrada');
    return todas.filter(i => i.estado === f);
  }
}
