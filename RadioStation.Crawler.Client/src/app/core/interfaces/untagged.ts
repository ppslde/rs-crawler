export interface IUntagged {
    artist: string;
    track: string;
    count: number;
}

export interface IReCrawl {
    url: string;
    records: IRecord[];
}


export interface IRecord {
    artist: string;
    tracks: IRecordTrack[];
}

export interface IRecordTrack {
    track: string;
    length: string;
}
