import { Component, OnInit } from '@angular/core';
import { Station } from 'src/app/core/models';
import { StationService } from 'src/app/shared/station.service';

@Component({
  selector: 'app-stations',
  templateUrl: './stations.component.html',
  styleUrls: ['./stations.component.css']
})
export class StationsComponent implements OnInit {

  constructor(public stationSrv: StationService) { }

  stations: Station[];
  selectedStation: Station;

  ngOnInit(): void {
    this.stationSrv.getStations().then(s => this.stations = s);
  }

  onStationSelected(s: Station): void {
    this.selectedStation = s;
  }
}
