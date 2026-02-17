import { Routes } from '@angular/router';
import { authGuard } from './auth/auth.guard';


export const routes: Routes = [
	{
		path: '',
		canMatch: [authGuard],
		loadComponent: () => import('./components/dashboard/dashboard').then((m) => m.Dashboard)
	}
];
