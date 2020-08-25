import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './core';
import { AuthGuard } from './auth.guard';
import { DashboardComponent, TaggingComponent, StationsComponent, OverviewComponent } from './dashboard';
export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'home' },
  {
    path: 'home',
    component: HomeComponent
  },
  {
    path: 'dashboard',
    component: DashboardComponent,
    canActivate: [AuthGuard],
    // data: { roles: [Role.Admin] }
    children: [
      {path: '', pathMatch: 'full', redirectTo: 'overview'},
      {path: 'overview', component: OverviewComponent},
      {path: 'stations', component: StationsComponent},
      {path: 'tagging', component: TaggingComponent},
      // {path: 'albums', component: ArtistAlbumListComponent},
    ]
  },
  {
    path: '**',
    component: NotFoundComponent
  },

];
