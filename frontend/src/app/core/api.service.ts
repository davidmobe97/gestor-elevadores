import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  ClienteDetalle, ClienteGuardar, ClienteResumen, ElevadorDetalle, ElevadorGuardar,
  ElevadorResumen, EstadoIncidencia, Incidencia, IncidenciaCrear, PanelResumen,
  Revision, RevisionCrear,
} from './modelos';

/**
 * Único punto de contacto con la API .NET.
 * Los componentes no usan HttpClient directamente: así, si cambia la API,
 * solo hay que tocar este archivo.
 */
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);
  private readonly url = environment.apiUrl;

  // Panel
  panel(): Observable<PanelResumen> {
    return this.http.get<PanelResumen>(`${this.url}/panel`);
  }

  // Clientes
  clientes(buscar = ''): Observable<ClienteResumen[]> {
    const params = buscar ? new HttpParams().set('buscar', buscar) : undefined;
    return this.http.get<ClienteResumen[]>(`${this.url}/clientes`, { params });
  }
  cliente(id: number): Observable<ClienteDetalle> {
    return this.http.get<ClienteDetalle>(`${this.url}/clientes/${id}`);
  }
  crearCliente(datos: ClienteGuardar): Observable<ClienteDetalle> {
    return this.http.post<ClienteDetalle>(`${this.url}/clientes`, datos);
  }
  actualizarCliente(id: number, datos: ClienteGuardar): Observable<void> {
    return this.http.put<void>(`${this.url}/clientes/${id}`, datos);
  }
  borrarCliente(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/clientes/${id}`);
  }

  // Elevadores
  elevadores(filtro: { clienteId?: number; soloVencidos?: boolean } = {}): Observable<ElevadorResumen[]> {
    let params = new HttpParams();
    if (filtro.clienteId) params = params.set('clienteId', filtro.clienteId);
    if (filtro.soloVencidos) params = params.set('soloVencidos', true);
    return this.http.get<ElevadorResumen[]>(`${this.url}/elevadores`, { params });
  }
  elevador(id: number): Observable<ElevadorDetalle> {
    return this.http.get<ElevadorDetalle>(`${this.url}/elevadores/${id}`);
  }
  crearElevador(datos: ElevadorGuardar): Observable<ElevadorDetalle> {
    return this.http.post<ElevadorDetalle>(`${this.url}/elevadores`, datos);
  }

  // Incidencias
  incidencias(estado?: EstadoIncidencia): Observable<Incidencia[]> {
    const params = estado ? new HttpParams().set('estado', estado) : undefined;
    return this.http.get<Incidencia[]>(`${this.url}/incidencias`, { params });
  }
  crearIncidencia(datos: IncidenciaCrear): Observable<Incidencia> {
    return this.http.post<Incidencia>(`${this.url}/incidencias`, datos);
  }
  cerrarIncidencia(id: number, solucion: string): Observable<void> {
    return this.http.post<void>(`${this.url}/incidencias/${id}/cerrar`, { solucion });
  }

  // Revisiones
  crearRevision(datos: RevisionCrear): Observable<Revision> {
    return this.http.post<Revision>(`${this.url}/revisiones`, datos);
  }
}
