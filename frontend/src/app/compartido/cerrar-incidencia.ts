import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../core/api.service';
import { textoError } from '../core/errores';

/**
 * Botón "Cerrar" de una incidencia que se despliega para escribir la solución.
 * Se usa en la ficha del elevador y en el listado de incidencias.
 * Avisa al componente padre con el evento (cerrada) para que recargue.
 */
@Component({
  selector: 'app-cerrar-incidencia',
  imports: [FormsModule],
  template: `
    @if (!abierto()) {
      <button class="boton boton--secundario boton--pequeno" (click)="abierto.set(true)">Cerrar incidencia</button>
    } @else {
      <form class="cerrar" (ngSubmit)="cerrar()">
        <input [(ngModel)]="solucion" name="solucion" placeholder="Solución aplicada…" required aria-label="Solución aplicada">
        <button class="boton boton--pequeno" type="submit" [disabled]="!solucion.trim() || enviando()">Cerrar</button>
        <button class="boton boton--secundario boton--pequeno" type="button" (click)="abierto.set(false)">✕</button>
      </form>
      @if (error()) { <small class="error">{{ error() }}</small> }
    }
  `,
  styles: `
    :host { display: block; margin-top: .5rem; }
    .cerrar { display: flex; gap: .4rem; }
    input { flex: 1; font: inherit; padding: .35rem .6rem; border: 1px solid var(--borde); border-radius: 8px; min-width: 0; }
    .error { color: var(--peligro); }
  `,
})
export class CerrarIncidencia {
  private readonly api = inject(ApiService);

  readonly incidenciaId = input.required<number>();
  readonly cerrada = output<void>();

  protected readonly abierto = signal(false);
  protected readonly enviando = signal(false);
  protected readonly error = signal('');
  protected solucion = '';

  protected cerrar(): void {
    this.enviando.set(true);
    this.api.cerrarIncidencia(this.incidenciaId(), this.solucion.trim()).subscribe({
      next: () => { this.abierto.set(false); this.solucion = ''; this.enviando.set(false); this.cerrada.emit(); },
      error: e => { this.error.set(textoError(e)); this.enviando.set(false); },
    });
  }
}
