import { Routes } from '@angular/router';

export const GANGS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./gang-list/gang-list.component').then(m => m.GangListComponent),
  },
  // TODO: Add more gang routes
  // { path: 'new', loadComponent: ... },
  // { path: ':id', loadComponent: ... },
];
