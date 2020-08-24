import { Station } from './station';
import { Song } from './song';

export class Play {
    constructor(
        public id: string,
        public started: Date,
        public crawledArtist: string,
        public crawledTrack: string,
        public originalSource: string,
        public stationId: string,
        public trackId: string,
        public lastTagged: Date,
        public station: Station,
        public track: Song
    ) { }
}
