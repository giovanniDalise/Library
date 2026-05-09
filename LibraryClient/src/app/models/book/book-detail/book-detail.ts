export interface BookDetail {
    id: number;
    title: string;
    isbn: string;
    coverReference: string;
    coverFile: File | null;
    // aggiungerei poi due dictionary nome e id per editor author per le tendine delle associazioni
}