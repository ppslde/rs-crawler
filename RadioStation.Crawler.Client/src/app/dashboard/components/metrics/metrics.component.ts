import { Component, OnInit } from '@angular/core';
import { MetricsService } from 'src/app/shared/metrics.service';
import { StationService } from 'src/app/shared/station.service';
import { Chart } from 'chart.js';
import { Station } from 'src/app/core/models';

@Component({
  selector: 'app-metrics',
  templateUrl: './metrics.component.html',
  styleUrls: ['./metrics.component.css']
})
export class MetricsComponent implements OnInit {

  constructor(public stationSrv: StationService, public metricSrv: MetricsService) { }

  data: any;
  loading: boolean;

  stations: Station[];
  chart: Chart = [];
  selectedStation: string;

  ngOnInit(): void {
    this.loading = true;
    this.metricSrv.loadMetrics().then(v => {
      if (v) {
        this.data = this.metricSrv.metrics;
        this.createChart();
      }
      this.loading = false;
    });
    this.stationSrv.getStations().then(s => this.stations = s);
  }

  createChart(): void {

    if (!this.selectedStation) {
      return;
    }

    const crawlStatusByStation = this.data.crawlStatus.reduce((col, s) => ({
      ...col,
      [s.stationId]: [...(col[s.stationId] || []), s],
    }), {});


    const dates = crawlStatusByStation[this.selectedStation].map(d => d.date);
    const counts = crawlStatusByStation[this.selectedStation].map(d => d.count);


    if ('destroy' in this.chart) {
      this.chart.destroy();
    }
    this.chart = new Chart('canvas', {
      type: 'bar',
      data: {
        labels: dates,
        datasets: [
          {
            data: counts,
            borderColor: '#3cba9f',
            backgroundColor: '#3cba9f',
            fill: true
          },
        ]
      },
      options: {
        global: {
          responsive: true,
          maintainAspectRatio: false
        },
        legend: {
          display: false
        },
        scales: {
          xAxes: [{
            display: true
          }],
          yAxes: [{
            display: true
          }],
        }
      }
    });
  }
}
