import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';

import { routes } from './router';

import { coreDeclarations } from './core';
import { HomeComponent } from './home/home.component';
import { SharedModule } from './shared/shared.module';

import { NgMaterialModule } from './shared/material.module';

import { AuthInterceptorProviders } from './auth.interceptor';
import { DashboardComponent } from './dashboard/dashboard.component';
import { MetricsComponent } from './dashboard/components/metrics/metrics.component';
import { OverviewComponent } from './dashboard/components/overview/overview.component';
import { StationsComponent } from './dashboard/components/stations/stations.component';
import { StationDetailsComponent } from './dashboard/components/stations/station-details/station-details.component';
import { TaggingComponent } from './dashboard/components/tagging/tagging.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    coreDeclarations,
    DashboardComponent,
    MetricsComponent,
    OverviewComponent,
    StationsComponent,
    StationDetailsComponent,
    TaggingComponent,
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(routes),
    HttpClientModule,
    BrowserAnimationsModule,
    NgMaterialModule,
    SharedModule,
  ],
  providers: [AuthInterceptorProviders],
  bootstrap: [AppComponent]
})
export class AppModule { }
