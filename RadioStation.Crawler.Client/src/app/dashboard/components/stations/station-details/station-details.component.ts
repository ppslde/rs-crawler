import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Station } from 'src/app/core';
import { StationService } from 'src/app/shared/station.service';

@Component({
  selector: 'app-station-details',
  templateUrl: './station-details.component.html',
  styleUrls: ['./station-details.component.css']
})
export class StationDetailsComponent implements OnInit, OnChanges {

  constructor(private stationSrv: StationService, private fb: FormBuilder) { }

  @Input() station: Station;
  @Output() stationChange = new EventEmitter<Station>();
  
  stationForm: FormGroup;

  ngOnInit(): void { }

  ngOnChanges(changes: SimpleChanges): void {


    this.stationForm = this.fb.group({
      id: new FormControl(changes.station.currentValue?.id || '', Validators.required),
      name: new FormControl(changes.station.currentValue?.name || '', Validators.required),
      title: new FormControl(changes.station.currentValue?.title || '', Validators.required),
      playlistUrl: new FormControl(changes.station.currentValue?.playlistUrl || '', Validators.required)
    });
  }

  OnSubmit() {

    this.stationSrv.update(this.stationForm.getRawValue()).subscribe(a => {
      console.log(a);
      this.stationChange.emit(a.data);
    });

  }



}
