import { Book } from "./book/book";

export class Editor {
    editorId:number;
    name:string;
    books:Book[];
    
    constructor(editorId:number, name:string, books:Book[]){
        this.editorId = editorId;
        this.name = name;
        this.books = books;

    }
}
