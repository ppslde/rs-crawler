import { Component, OnInit } from '@angular/core';
import { StationService } from '../shared/station.service';

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
