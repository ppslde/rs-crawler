import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from './auth.service';
import { TokenService } from './token.service';
import { StationService } from './station.service';
import { MetricsService } from './metrics.service';

const components = [
];

const services = [
  AuthService,
  TokenService,
  StationService,
  MetricsService
];

@NgModule({
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  declarations: [components],
  providers: [services],
  exports: [components, FormsModule, ReactiveFormsModule]
})
export class SharedModule { }
