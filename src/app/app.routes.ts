import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/planner-setup/planner-setup').then(
        (module) => module.PlannerSetup,
      ),
  },
];
