import { Component, OnInit, inject, input, numberAttribute, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ClienteDetalle, TEXTO_TIPO, TipoElevador } from '../../core/modelos';
import { textoError } from '../../core/errores';
import { Etiqueta } from '../../compartido/etiqueta';

/** Ficha de un taller con sus elevadores y alta de elevadores nuevos. */
@Component({
  selector: 'app-cliente-detalle',
  imports: [RouterLink, DatePipe, ReactiveFormsModule, Etiqueta],
  template: `
    @if (error()) { <div class="aviso aviso--error">{{ error() }}</div> }

    @if (cliente(); as c) {
      <div class="cabecera">
        <div>
          <a routerLink="/clientes">← Clientes</a>
          <h1>{{ c.nombreTaller }}</h1>
          <p class="subtitulo">{{ c.poblacion }} ({{ c.provincia }}) · cliente desde {{ c.fechaAlta | date: 'MMMM yyyy' }}</p>
        </div>
        <div class="acciones">
          <a class="boton boton--secundario" [routerLink]="['/clientes', c.id, 'editar']">Editar</a>
          <button class="boton boton--peligro" (click)="borrar()">Borrar</button>
        </div>
      </div>

      <section class="tarjeta">
        <dl class="datos">
          <div><dt>Contacto</dt><dd>{{ c.personaContacto || '—' }}</dd></div>
          <div><dt>Teléfono</dt><dd>{{ c.telefono || '—' }}</dd></div>
          <div><dt>Email</dt><dd>{{ c.email || '—' }}</dd></div>
          <div><dt>CIF</dt><dd>{{ c.cif || '—' }}</dd></div>
          <div><dt>Dirección</dt><dd>{{ c.direccion || '—' }}</dd></div>
        </dl>
      </section>

      <section class="tarjeta">
        <div class="cabecera">
          <h2>Elevadores ({{ c.elevadores.length }})</h2>
          <button class="boton boton--pequeno" (click)="mostrarAlta.set(!mostrarAlta())">
            {{ mostrarAlta() ? 'Cancelar' : '+ Añadir elevador' }}
          </button>
        </div>

        @if (mostrarAlta()) {
          <form class="alta" [formGroup]="formElevador" (ngSubmit)="anadirElevador()">
            <div class="formulario">
              <div class="campo"><label for="marca">Marca *</label><input id="marca" formControlName="marca"></div>
              <div class="campo"><label for="modelo">Modelo *</label><input id="modelo" formControlName="modelo"></div>
              <div class="campo"><label for="serie">Nº de serie *</label><input id="serie" formControlName="numeroSerie"></div>
              <div class="campo">
                <label for="tipo">Tipo</label>
                <select id="tipo" formControlName="tipo">
                  @for (t of tiposLista; track t[0]) { <option [value]="t[0]">{{ t[1] }}</option> }
                </select>
              </div>
              <div class="campo"><label for="cap">Capacidad (kg)</label><input id="cap" type="number" formControlName="capacidadKg" min="500" max="20000"></div>
              <div class="campo"><label for="fecha">Fecha de instalación *</label><input id="fecha" type="date" formControlName="fechaInstalacion"></div>
            </div>
            <div class="acciones">
              <button class="boton" type="submit" [disabled]="formElevador.invalid">Guardar elevador</button>
            </div>
          </form>
        }

        <div class="tabla-scroll">
          <table class="tabla">
            <thead><tr><th>Elevador</th><th>Nº serie</th><th>Tipo</th><th>Próxima revisión</th><th>Incidencias</th></tr></thead>
            <tbody>
              @for (e of c.elevadores; track e.id) {
                <tr>
                  <td><a [routerLink]="['/elevadores', e.id]"><strong>{{ e.marca }} {{ e.modelo }}</strong></a></td>
                  <td>{{ e.numeroSerie }}</td>
                  <td>{{ tipos[e.tipo] }}</td>
                  <td>{{ e.proximaRevision | date: 'dd/MM/yyyy' }} <app-etiqueta [valor]="e.revisionVencida ? 'Vencida' : 'AlDia'" /></td>
                  <td>{{ e.incidenciasAbiertas ? e.incidenciasAbiertas + ' abierta(s)' : '—' }}</td>
                </tr>
              } @empty {
                <tr><td colspan="5" class="vacio">Este taller todavía no tiene elevadores.</td></tr>
              }
            </tbody>
          </table>
        </div>
      </section>
    } @else if (!error()) {
      <p class="cargando">Cargando…</p>
    }
  `,
  styles: `
    .alta { background: #f8fafc; border: 1px dashed var(--borde); border-radius: 8px; padding: 1rem; margin-bottom: 1rem; }
    .cabecera h2 { margin: 0; }
    .cabecera { margin-bottom: 1rem; }
  `,
})
export class ClienteDetallePagina implements OnInit {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder).nonNullable;

  readonly id = input.required({ transform: numberAttribute });

  protected readonly cliente = signal<ClienteDetalle | null>(null);
  protected readonly error = signal('');
  protected readonly mostrarAlta = signal(false);
  protected readonly tipos = TEXTO_TIPO;
  protected readonly tiposLista = Object.entries(TEXTO_TIPO);

  protected readonly formElevador = this.fb.group({
    marca: ['', Validators.required],
    modelo: ['', Validators.required],
    numeroSerie: ['', Validators.required],
    tipo: ['DosColumnas' as TipoElevador],
    capacidadKg: [3000, [Validators.required, Validators.min(500), Validators.max(20000)]],
    fechaInstalacion: [new Date().toISOString().slice(0, 10), Validators.required],
  });

  ngOnInit(): void { this.cargar(); }

  private cargar(): void {
    this.api.cliente(this.id()).subscribe({
      next: c => this.cliente.set(c),
      error: e => this.error.set(textoError(e)),
    });
  }

  protected anadirElevador(): void {
    this.error.set('');
    this.api.crearElevador({ ...this.formElevador.getRawValue(), clienteId: this.id() }).subscribe({
      next: () => { this.mostrarAlta.set(false); this.formElevador.reset(); this.cargar(); },
      error: e => this.error.set(textoError(e)),
    });
  }

  protected borrar(): void {
    if (!confirm('¿Seguro que quieres borrar este cliente?')) return;
    this.api.borrarCliente(this.id()).subscribe({
      next: () => this.router.navigate(['/clientes']),
      // Si tiene elevadores, la API responde 409 con el motivo y lo mostramos.
      error: e => this.error.set(textoError(e)),
    });
  }
}
