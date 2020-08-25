import { Component, OnInit } from '@angular/core';
import { IRecord, IReCrawl, IUntagged } from 'src/app/core';
import { TaggingService } from 'src/app/shared/tagging.service';

@Component({
  selector: 'app-tagging',
  templateUrl: './tagging.component.html',
  styleUrls: ['./tagging.component.css']
})
export class TaggingComponent implements OnInit {

  constructor(private tagSrv: TaggingService) { }


  untaggedsongs: IUntagged[] = [];
  recrawl: IReCrawl;

  ngOnInit(): void {
    this.tagSrv.getUntaggedSongs().then(d => {
      this.untaggedsongs = d;
    });
  }

  onItemClicked(item: IUntagged): void {
    this.tagSrv.getSongTagData(item).then(d => {
      this.recrawl = d;
    });
  }

  onRecrawlItem(): void {

  }

}
