import { Routes } from '@angular/router';
import { adminGuard, authGuard } from './core/auth.guard';
import { Shell } from './shell';

const t = (texto: string) => `${texto} · Gestor de elevadores`;

// Cada página se carga solo cuando se visita (lazy loading).
export const routes: Routes = [
  { path: 'login', title: t('Iniciar sesión'), loadComponent: () => import('./paginas/login/login').then(m => m.Login) },
  {
    // Todo lo que va dentro de Shell (menú lateral) exige sesión iniciada.
    path: '',
    component: Shell,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'panel' },
      { path: 'panel', title: t('Panel'), loadComponent: () => import('./paginas/panel/panel').then(m => m.Panel) },
      { path: 'clientes', title: t('Clientes'), loadComponent: () => import('./paginas/clientes/clientes').then(m => m.Clientes) },
      { path: 'clientes/nuevo', title: t('Nuevo cliente'), canActivate: [adminGuard], loadComponent: () => import('./paginas/cliente-form/cliente-form').then(m => m.ClienteForm) },
      { path: 'clientes/:id', title: t('Cliente'), loadComponent: () => import('./paginas/cliente-detalle/cliente-detalle').then(m => m.ClienteDetallePagina) },
      { path: 'clientes/:id/editar', title: t('Editar cliente'), canActivate: [adminGuard], loadComponent: () => import('./paginas/cliente-form/cliente-form').then(m => m.ClienteForm) },
      { path: 'elevadores', title: t('Elevadores'), loadComponent: () => import('./paginas/elevadores/elevadores').then(m => m.Elevadores) },
      { path: 'elevadores/:id', title: t('Elevador'), loadComponent: () => import('./paginas/elevador-detalle/elevador-detalle').then(m => m.ElevadorDetallePagina) },
      { path: 'incidencias', title: t('Incidencias'), loadComponent: () => import('./paginas/incidencias/incidencias').then(m => m.Incidencias) },
    ],
  },
  { path: '**', redirectTo: 'panel' },
];
