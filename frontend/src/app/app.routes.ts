import { isDevMode } from '@angular/core';
import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    title: 'Skyler Sidner | Portfolio',
    loadComponent: () => import('./features/home/home-page').then((m) => m.HomePageComponent)
  },
  ...(isDevMode()
    ? [
        {
          path: 'library',
          title: 'Component Library',
          loadComponent: () => import('./features/style-guide/style-guide-page').then((m) => m.StyleGuidePageComponent)
        }
      ]
    : []),
  {
    path: '**',
    redirectTo: ''
  }
];
