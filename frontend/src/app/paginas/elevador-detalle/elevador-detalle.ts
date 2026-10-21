import { Component, OnInit, inject, input, numberAttribute, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ElevadorDetalle, PrioridadIncidencia, ResultadoRevision, TEXTO_RESULTADO, TEXTO_TIPO } from '../../core/modelos';
import { textoError } from '../../core/errores';
import { Etiqueta } from '../../compartido/etiqueta';
import { CerrarIncidencia } from '../../compartido/cerrar-incidencia';

/** Ficha de un elevador: datos, historial de revisiones e incidencias. */
@Component({
  selector: 'app-elevador-detalle',
  imports: [RouterLink, DatePipe, DecimalPipe, ReactiveFormsModule, Etiqueta, CerrarIncidencia],
  template: `
    @if (error()) { <div class="aviso aviso--error">{{ error() }}</div> }

    @if (elevador(); as e) {
      <div class="cabecera">
        <div>
          <a [routerLink]="['/clientes', e.clienteId]">← {{ e.nombreTaller }}</a>
          <h1>{{ e.marca }} {{ e.modelo }}</h1>
          <p class="subtitulo">{{ tipos[e.tipo] }} · Nº de serie {{ e.numeroSerie }}</p>
        </div>
        <div class="revision" [class.revision--vencida]="e.revisionVencida">
          <span>Próxima revisión</span>
          <strong>{{ e.proximaRevision | date: 'dd/MM/yyyy' }}</strong>
          <app-etiqueta [valor]="e.revisionVencida ? 'Vencida' : 'AlDia'" />
        </div>
      </div>

      <section class="tarjeta">
        <dl class="datos">
          <div><dt>Capacidad</dt><dd>{{ e.capacidadKg | number }} kg</dd></div>
          <div><dt>Instalado</dt><dd>{{ e.fechaInstalacion | date: 'dd/MM/yyyy' }}</dd></div>
          <div><dt>Revisiones</dt><dd>{{ e.revisiones.length }}</dd></div>
          <div><dt>Incidencias</dt><dd>{{ e.incidencias.length }}</dd></div>
        </dl>
      </section>

      <div class="columnas">
        <!-- INCIDENCIAS -->
        <section class="tarjeta">
          <div class="cabecera">
            <h2>Incidencias</h2>
            <button class="boton boton--pequeno" (click)="panel.set(panel() === 'incidencia' ? '' : 'incidencia')">
              {{ panel() === 'incidencia' ? 'Cancelar' : '+ Nueva' }}
            </button>
          </div>

          @if (panel() === 'incidencia') {
            <form class="alta" [formGroup]="formIncidencia" (ngSubmit)="crearIncidencia()">
              <div class="campo"><label for="titulo">Qué le pasa *</label><input id="titulo" formControlName="titulo" placeholder="Ej.: pierde aceite el cilindro"></div>
              <div class="campo"><label for="desc">Descripción</label><textarea id="desc" formControlName="descripcion"></textarea></div>
              <div class="dos">
                <div class="campo">
                  <label for="prio">Prioridad</label>
                  <select id="prio" formControlName="prioridad">
                    @for (p of prioridades; track p) { <option [value]="p">{{ p }}</option> }
                  </select>
                </div>
                <div class="campo"><label for="tec">Técnico</label><input id="tec" formControlName="tecnicoAsignado"></div>
              </div>
              <button class="boton" type="submit" [disabled]="formIncidencia.invalid">Abrir incidencia</button>
            </form>
          }

          @for (i of e.incidencias; track i.id) {
            <article class="item">
              <div class="item__cabecera">
                <strong>{{ i.titulo }}</strong>
                <span><app-etiqueta [valor]="i.prioridad" /> <app-etiqueta [valor]="i.estado" /></span>
              </div>
              <small>Abierta el {{ i.fechaApertura | date: 'dd/MM/yyyy HH:mm' }}{{ i.tecnicoAsignado ? ' · ' + i.tecnicoAsignado : '' }}</small>
              @if (i.descripcion) { <p>{{ i.descripcion }}</p> }
              @if (i.estado === 'Cerrada') {
                <p class="solucion">✔ {{ i.solucion }} <small>({{ i.fechaCierre | date: 'dd/MM/yyyy' }})</small></p>
              } @else {
                <app-cerrar-incidencia [incidenciaId]="i.id" (cerrada)="cargar()" />
              }
            </article>
          } @empty { <p class="vacio">Sin incidencias registradas.</p> }
        </section>

        <!-- REVISIONES -->
        <section class="tarjeta">
          <div class="cabecera">
            <h2>Revisiones</h2>
            <button class="boton boton--pequeno" (click)="panel.set(panel() === 'revision' ? '' : 'revision')">
              {{ panel() === 'revision' ? 'Cancelar' : '+ Registrar' }}
            </button>
          </div>

          @if (panel() === 'revision') {
            <form class="alta" [formGroup]="formRevision" (ngSubmit)="crearRevision()">
              <div class="dos">
                <div class="campo"><label for="rfecha">Fecha *</label><input id="rfecha" type="date" formControlName="fecha" [max]="hoy"></div>
                <div class="campo"><label for="rtec">Técnico *</label><input id="rtec" formControlName="tecnico"></div>
              </div>
              <div class="campo">
                <label for="res">Resultado</label>
                <select id="res" formControlName="resultado">
                  @for (r of resultados; track r[0]) { <option [value]="r[0]">{{ r[1] }}</option> }
                </select>
              </div>
              <div class="campo"><label for="obs">Observaciones</label><textarea id="obs" formControlName="observaciones"></textarea></div>
              <button class="boton" type="submit" [disabled]="formRevision.invalid">Guardar revisión</button>
            </form>
          }

          @for (r of e.revisiones; track r.id) {
            <article class="item">
              <div class="item__cabecera">
                <strong>{{ r.fecha | date: 'dd/MM/yyyy' }}</strong>
                <app-etiqueta [valor]="r.resultado" />
              </div>
              <small>Técnico: {{ r.tecnico }}</small>
              @if (r.observaciones) { <p>{{ r.observaciones }}</p> }
            </article>
          } @empty { <p class="vacio">Todavía no tiene revisiones.</p> }
        </section>
      </div>
    } @else if (!error()) {
      <p class="cargando">Cargando…</p>
    }
  `,
  styles: `
    .columnas { display: grid; grid-template-columns: 1fr 1fr; gap: 1.5rem; align-items: start; }
    .columnas .tarjeta { margin: 0; min-width: 0; }
    .tarjeta .cabecera { margin-bottom: 1rem; h2 { margin: 0; } }
    .revision {
      background: var(--tarjeta); border: 1px solid var(--borde); border-left: 4px solid var(--ok);
      border-radius: var(--radio); padding: .8rem 1.1rem; display: flex; flex-direction: column; gap: .2rem;
      span { color: var(--suave); font-size: .8rem; }
      strong { font-size: 1.3rem; }
      &--vencida { border-left-color: var(--peligro); }
    }
    .alta {
      background: #f8fafc; border: 1px dashed var(--borde); border-radius: 8px; padding: 1rem;
      margin-bottom: 1rem; display: flex; flex-direction: column; gap: .8rem;
    }
    .dos { display: grid; grid-template-columns: 1fr 1fr; gap: .8rem; }
    .item {
      border-bottom: 1px solid var(--borde); padding: .8rem 0;
      &:last-child { border-bottom: 0; }
      &__cabecera { display: flex; justify-content: space-between; gap: .5rem; align-items: center; flex-wrap: wrap; }
      small { color: var(--suave); }
      p { margin: .4rem 0 0; }
    }
    .solucion { color: var(--ok); }
    @media (max-width: 900px) { .columnas, .dos { grid-template-columns: 1fr; } }
  `,
})
export class ElevadorDetallePagina implements OnInit {
  private readonly api = inject(ApiService);
  private readonly fb = inject(FormBuilder).nonNullable;

  readonly id = input.required({ transform: numberAttribute });

  protected readonly elevador = signal<ElevadorDetalle | null>(null);
  protected readonly error = signal('');
  /** Qué formulario está abierto: '', 'incidencia' o 'revision'. */
  protected readonly panel = signal<'' | 'incidencia' | 'revision'>('');

  protected readonly tipos = TEXTO_TIPO;
  protected readonly resultados = Object.entries(TEXTO_RESULTADO);
  protected readonly prioridades: PrioridadIncidencia[] = ['Baja', 'Media', 'Alta', 'Urgente'];
  protected readonly hoy = new Date().toISOString().slice(0, 10);

  protected readonly formIncidencia = this.fb.group({
    titulo: ['', [Validators.required, Validators.maxLength(150)]],
    descripcion: [''],
    prioridad: ['Media' as PrioridadIncidencia],
    tecnicoAsignado: [''],
  });

  protected readonly formRevision = this.fb.group({
    fecha: [this.hoy, Validators.required],
    tecnico: ['', Validators.required],
    resultado: ['Favorable' as ResultadoRevision],
    observaciones: [''],
  });

  ngOnInit(): void { this.cargar(); }

  cargar(): void {
    this.api.elevador(this.id()).subscribe({
      next: e => { this.elevador.set(e); this.error.set(''); },
      error: e => this.error.set(textoError(e)),
    });
  }

  protected crearIncidencia(): void {
    const v = this.formIncidencia.getRawValue();
    this.api.crearIncidencia({
      elevadorId: this.id(), titulo: v.titulo, prioridad: v.prioridad,
      descripcion: v.descripcion.trim() || null, tecnicoAsignado: v.tecnicoAsignado.trim() || null,
    }).subscribe({
      next: () => { this.formIncidencia.reset(); this.panel.set(''); this.cargar(); },
      error: e => this.error.set(textoError(e)),
    });
  }

  protected crearRevision(): void {
    const v = this.formRevision.getRawValue();
    this.api.crearRevision({ ...v, elevadorId: this.id(), observaciones: v.observaciones.trim() || null }).subscribe({
      next: () => { this.formRevision.reset(); this.panel.set(''); this.cargar(); },
      error: e => this.error.set(textoError(e)),
    });
  }
}
