import { Routes } from '@angular/router';

export const CARS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./car-list/car-list.component').then(m => m.CarListComponent),
  },
];
