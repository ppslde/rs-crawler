import { Component, OnInit } from '@angular/core';
import { first } from 'rxjs/operators';

import { StationService } from '../shared/station.service';
import { Station } from '../core';
import { MetricsService } from '../shared/metrics.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  constructor(private stationSrv: StationService) { }

  ngOnInit(): void {
  }
}