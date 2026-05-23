import { Routes } from '@angular/router';

export interface AppNavData {
  navLabel: string;
}

export const routes: Routes = [
  {
    path: '',
    data: { navLabel: 'Home' } satisfies AppNavData,
    loadComponent: () =>
      import('./features/home/home').then((module) => module.Home),
  },
  {
    path: 'gac-planner',
    data: { navLabel: 'GAC Planner' } satisfies AppNavData,
    loadComponent: () =>
      import('./features/planner-setup/planner-setup').then((module) => module.PlannerSetup),
  },
  {
    path: 'raid-score-calculator',
    data: { navLabel: 'Raid Score Calculator' } satisfies AppNavData,
    loadComponent: () =>
      import('./features/raid-score-calculator/raid-score-calculator').then((module) => module.RaidScoreCalculator),
  },
  {
    path: 'rise-of-the-empire-rewards',
    data: { navLabel: 'Rise of the Empire Rewards' } satisfies AppNavData,
    loadComponent: () =>
      import('./features/rise-of-the-empire-rewards/rise-of-the-empire-rewards').then(
        (module) => module.RiseOfTheEmpireRewardsPage,
      ),
  },
  {
    path: 'marquee-dates',
    data: { navLabel: 'Marquee Dates' } satisfies AppNavData,
    loadComponent: () =>
      import('./features/marquee-dates/marquee-dates').then((module) => module.MarqueeDatesPage),
  },
  {
    path: 'characters',
    data: { navLabel: 'Characters' } satisfies AppNavData,
    loadComponent: () =>
      import('./features/characters-page/characters-page').then((module) => module.CharactersPage),
  },
  {
    path: 'signal-data-farming',
    data: { navLabel: 'Signal Data Farming' } satisfies AppNavData,
    loadComponent: () =>
      import('./features/signal-data-farming-page/signal-data-farming-page').then(
        (module) => module.SignalDataFarmingPage,
      ),
  },
];
