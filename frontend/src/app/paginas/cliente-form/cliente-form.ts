import { Component, OnInit, inject, input, numberAttribute, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ApiService } from '../../core/api.service';
import { ClienteGuardar } from '../../core/modelos';
import { textoError } from '../../core/errores';

/** Alta y edición de un cliente (la misma pantalla para las dos cosas). */
@Component({
  selector: 'app-cliente-form',
  imports: [ReactiveFormsModule, RouterLink],
  template: `
    <div class="cabecera">
      <div>
        <h1>{{ id() ? 'Editar cliente' : 'Nuevo cliente' }}</h1>
        <p class="subtitulo">Datos del taller</p>
      </div>
    </div>

    <form class="tarjeta" [formGroup]="form" (ngSubmit)="guardar()">
      @if (error()) { <div class="aviso aviso--error">{{ error() }}</div> }

      <div class="formulario">
        <div class="campo campo--ancho">
          <label for="nombre">Nombre del taller *</label>
          <input id="nombre" formControlName="nombreTaller" maxlength="150">
          @if (invalido('nombreTaller')) { <span class="error-campo">El nombre es obligatorio.</span> }
        </div>
        <div class="campo"><label for="cif">CIF</label><input id="cif" formControlName="cif" maxlength="20"></div>
        <div class="campo"><label for="contacto">Persona de contacto</label><input id="contacto" formControlName="personaContacto"></div>
        <div class="campo"><label for="tel">Teléfono</label><input id="tel" type="tel" formControlName="telefono" maxlength="20"></div>
        <div class="campo">
          <label for="email">Email</label>
          <input id="email" type="email" formControlName="email">
          @if (invalido('email')) { <span class="error-campo">El email no es válido.</span> }
        </div>
        <div class="campo campo--ancho"><label for="dir">Dirección</label><input id="dir" formControlName="direccion"></div>
        <div class="campo">
          <label for="pob">Población *</label>
          <input id="pob" formControlName="poblacion">
          @if (invalido('poblacion')) { <span class="error-campo">Obligatorio.</span> }
        </div>
        <div class="campo">
          <label for="prov">Provincia *</label>
          <input id="prov" formControlName="provincia">
          @if (invalido('provincia')) { <span class="error-campo">Obligatorio.</span> }
        </div>
      </div>

      <div class="acciones">
        <button class="boton" type="submit" [disabled]="guardando()">{{ guardando() ? 'Guardando…' : 'Guardar' }}</button>
        <a class="boton boton--secundario" [routerLink]="id() ? ['/clientes', id()] : '/clientes'">Cancelar</a>
      </div>
    </form>
  `,
})
export class ClienteForm implements OnInit {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder).nonNullable;

  /** Viene de la URL /clientes/:id/editar; en /clientes/nuevo no existe. */
  readonly id = input(0, { transform: numberAttribute });

  protected readonly guardando = signal(false);
  protected readonly error = signal('');

  protected readonly form = this.fb.group({
    nombreTaller: ['', [Validators.required, Validators.maxLength(150)]],
    cif: [''],
    personaContacto: [''],
    telefono: [''],
    email: ['', Validators.email],
    direccion: [''],
    poblacion: ['', Validators.required],
    provincia: ['', Validators.required],
  });

  ngOnInit(): void {
    if (this.id()) {
      this.api.cliente(this.id()).subscribe({
        next: c => this.form.patchValue({ ...c, cif: c.cif ?? '', personaContacto: c.personaContacto ?? '', telefono: c.telefono ?? '', email: c.email ?? '', direccion: c.direccion ?? '' }),
        error: e => this.error.set(textoError(e)),
      });
    }
  }

  protected invalido(campo: keyof typeof this.form.controls): boolean {
    const c = this.form.controls[campo];
    return c.invalid && (c.touched || c.dirty);
  }

  protected guardar(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }

    // Los campos vacíos opcionales se envían como null.
    const v = this.form.getRawValue();
    const nulo = (s: string) => s.trim() || null;
    const datos: ClienteGuardar = {
      ...v, cif: nulo(v.cif), personaContacto: nulo(v.personaContacto),
      telefono: nulo(v.telefono), email: nulo(v.email), direccion: nulo(v.direccion),
    };

    this.guardando.set(true);
    const alFallar = (e: unknown) => { this.error.set(textoError(e)); this.guardando.set(false); };

    if (this.id()) {
      this.api.actualizarCliente(this.id(), datos).subscribe({
        next: () => this.router.navigate(['/clientes', this.id()]),
        error: alFallar,
      });
    } else {
      this.api.crearCliente(datos).subscribe({
        next: creado => this.router.navigate(['/clientes', creado.id]),
        error: alFallar,
      });
    }
  }
}
