import { Routes } from '@angular/router';

export const ENERGY_ROUTES: Routes = [
  {
    path: 'log',
    loadComponent: () => import('./log-energy/log-energy.component').then(m => m.LogEnergyComponent),
  },
];
