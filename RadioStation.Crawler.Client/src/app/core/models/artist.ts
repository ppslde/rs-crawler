export class Artist {
    constructor(
        public id: string,
        public Title: string,
        public Score: number,
        public MusicBrainzId: string,
        public Country: string,
        public City: string,
        public Alias: string,
        public Genres: string,
        public Tags: string
    ) { }
}
