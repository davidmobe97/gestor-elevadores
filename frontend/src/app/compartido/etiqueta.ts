import { Component, computed, input } from '@angular/core';

type Color = 'verde' | 'ambar' | 'rojo' | 'azul' | 'gris';

// Qué color lleva cada valor posible de los enums de la API.
const COLORES: Record<string, Color> = {
  // Estado de incidencia
  Abierta: 'rojo', EnCurso: 'ambar', Cerrada: 'verde',
  // Prioridad
  Baja: 'gris', Media: 'azul', Alta: 'ambar', Urgente: 'rojo',
  // Resultado de revisión
  Favorable: 'verde', ConDefectosLeves: 'ambar', Desfavorable: 'rojo',
  // Estado de la revisión anual
  Vencida: 'rojo', AlDia: 'verde',
};

const TEXTOS: Record<string, string> = {
  EnCurso: 'En curso',
  ConDefectosLeves: 'Defectos leves',
  AlDia: 'Al día',
};

/**
 * Etiqueta de color para estados, prioridades y resultados.
 * Uso: <app-etiqueta valor="Urgente" />
 */
@Component({
  selector: 'app-etiqueta',
  template: `<span class="etiqueta" [class]="'etiqueta etiqueta--' + color()">{{ texto() }}</span>`,
  styles: `
    .etiqueta {
      display: inline-block;
      padding: .15rem .55rem;
      border-radius: 999px;
      font-size: .78rem;
      font-weight: 600;
      white-space: nowrap;
    }
    .etiqueta--verde { background: #dcf5e3; color: #116329; }
    .etiqueta--ambar { background: #fff1d6; color: #8a5300; }
    .etiqueta--rojo  { background: #ffe1e3; color: #a40e26; }
    .etiqueta--azul  { background: #dde9ff; color: #1748a3; }
    .etiqueta--gris  { background: #eceff3; color: #4b5563; }
  `,
})
export class Etiqueta {
  readonly valor = input.required<string>();
  protected readonly color = computed<Color>(() => COLORES[this.valor()] ?? 'gris');
  protected readonly texto = computed(() => TEXTOS[this.valor()] ?? this.valor());
}
