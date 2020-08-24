import { Time } from '@angular/common';
import { Artist } from './artist';

export class Song {

    constructor(
        public id: string,
        public MusicBrainzId: string,
        public Title: string,
        public FoundTracks: Number,
        public Duration: Time,
        public DurationMax: Time,
        public DurationMin: Time,
        public DurationAvg: Time,
        public ArtistId: string,
        public Artist: Artist
    ) { }

}