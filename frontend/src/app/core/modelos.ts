// Tipos que coinciden con los DTO de la API .NET (GestorElevadores.Api/Dtos).

export type TipoElevador = 'DosColumnas' | 'CuatroColumnas' | 'Tijera' | 'Parking';
export type PrioridadIncidencia = 'Baja' | 'Media' | 'Alta' | 'Urgente';
export type EstadoIncidencia = 'Abierta' | 'EnCurso' | 'Cerrada';
export type ResultadoRevision = 'Favorable' | 'ConDefectosLeves' | 'Desfavorable';

export interface ClienteResumen {
  id: number;
  nombreTaller: string;
  poblacion: string;
  provincia: string;
  telefono: string | null;
  numeroElevadores: number;
}

export interface ClienteDetalle {
  id: number;
  nombreTaller: string;
  cif: string | null;
  personaContacto: string | null;
  telefono: string | null;
  email: string | null;
  direccion: string | null;
  poblacion: string;
  provincia: string;
  fechaAlta: string;
  elevadores: ElevadorResumen[];
}

export interface ClienteGuardar {
  nombreTaller: string;
  cif: string | null;
  personaContacto: string | null;
  telefono: string | null;
  email: string | null;
  direccion: string | null;
  poblacion: string;
  provincia: string;
}

export interface ElevadorResumen {
  id: number;
  marca: string;
  modelo: string;
  numeroSerie: string;
  tipo: TipoElevador;
  clienteId: number;
  nombreTaller: string;
  proximaRevision: string;
  revisionVencida: boolean;
  incidenciasAbiertas: number;
}

export interface ElevadorDetalle {
  id: number;
  marca: string;
  modelo: string;
  numeroSerie: string;
  tipo: TipoElevador;
  capacidadKg: number;
  fechaInstalacion: string;
  clienteId: number;
  nombreTaller: string;
  proximaRevision: string;
  revisionVencida: boolean;
  revisiones: Revision[];
  incidencias: Incidencia[];
}

export interface ElevadorGuardar {
  marca: string;
  modelo: string;
  numeroSerie: string;
  tipo: TipoElevador;
  capacidadKg: number;
  fechaInstalacion: string;
  clienteId: number;
}

export interface Incidencia {
  id: number;
  titulo: string;
  descripcion: string | null;
  prioridad: PrioridadIncidencia;
  estado: EstadoIncidencia;
  tecnicoAsignado: string | null;
  fechaApertura: string;
  fechaCierre: string | null;
  solucion: string | null;
  elevadorId: number;
  elevadorNombre: string;
  nombreTaller: string;
}

export interface IncidenciaCrear {
  elevadorId: number;
  titulo: string;
  descripcion: string | null;
  prioridad: PrioridadIncidencia;
  tecnicoAsignado: string | null;
}

export interface Revision {
  id: number;
  fecha: string;
  tecnico: string;
  resultado: ResultadoRevision;
  observaciones: string | null;
  elevadorId: number;
}

export interface RevisionCrear {
  elevadorId: number;
  fecha: string;
  tecnico: string;
  resultado: ResultadoRevision;
  observaciones: string | null;
}

export interface PanelResumen {
  totalClientes: number;
  totalElevadores: number;
  incidenciasAbiertas: number;
  incidenciasUrgentes: number;
  revisionesVencidas: number;
  revisionesProximos30Dias: number;
  proximasRevisiones: ElevadorResumen[];
}

// Textos para mostrar los enums en pantalla.
export const TEXTO_TIPO: Record<TipoElevador, string> = {
  DosColumnas: 'Dos columnas',
  CuatroColumnas: 'Cuatro columnas',
  Tijera: 'Tijera',
  Parking: 'Parking',
};

export const TEXTO_ESTADO: Record<EstadoIncidencia, string> = {
  Abierta: 'Abierta',
  EnCurso: 'En curso',
  Cerrada: 'Cerrada',
};

export const TEXTO_RESULTADO: Record<ResultadoRevision, string> = {
  Favorable: 'Favorable',
  ConDefectosLeves: 'Con defectos leves',
  Desfavorable: 'Desfavorable',
};
