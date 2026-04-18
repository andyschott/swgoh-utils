import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home').then((module) => module.Home),
  },
  {
    path: 'gac-planner',
    loadComponent: () =>
      import('./features/planner-setup/planner-setup').then((module) => module.PlannerSetup),
  },
  {
    path: 'raid-score-calculator',
    loadComponent: () =>
      import('./features/raid-score-calculator/raid-score-calculator').then((module) => module.RaidScoreCalculator),
  },
  {
    path: 'rise-of-the-empire-rewards',
    loadComponent: () =>
      import('./features/rise-of-the-empire-rewards/rise-of-the-empire-rewards').then(
        (module) => module.RiseOfTheEmpireRewardsPage,
      ),
  },
];
