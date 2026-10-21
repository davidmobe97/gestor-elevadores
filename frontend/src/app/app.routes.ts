import { Routes } from '@angular/router';

// Cada página se carga solo cuando se visita (lazy loading).
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'panel' },
  { path: 'panel', title: 'Panel · Gestor de elevadores', loadComponent: () => import('./paginas/panel/panel').then(m => m.Panel) },
  { path: 'clientes', title: 'Clientes · Gestor de elevadores', loadComponent: () => import('./paginas/clientes/clientes').then(m => m.Clientes) },
  { path: 'clientes/nuevo', title: 'Nuevo cliente · Gestor de elevadores', loadComponent: () => import('./paginas/cliente-form/cliente-form').then(m => m.ClienteForm) },
  { path: 'clientes/:id', title: 'Cliente · Gestor de elevadores', loadComponent: () => import('./paginas/cliente-detalle/cliente-detalle').then(m => m.ClienteDetallePagina) },
  { path: 'clientes/:id/editar', title: 'Editar cliente · Gestor de elevadores', loadComponent: () => import('./paginas/cliente-form/cliente-form').then(m => m.ClienteForm) },
  { path: 'elevadores', title: 'Elevadores · Gestor de elevadores', loadComponent: () => import('./paginas/elevadores/elevadores').then(m => m.Elevadores) },
  { path: 'elevadores/:id', title: 'Elevador · Gestor de elevadores', loadComponent: () => import('./paginas/elevador-detalle/elevador-detalle').then(m => m.ElevadorDetallePagina) },
  { path: 'incidencias', title: 'Incidencias · Gestor de elevadores', loadComponent: () => import('./paginas/incidencias/incidencias').then(m => m.Incidencias) },
  { path: '**', redirectTo: 'panel' },
];
