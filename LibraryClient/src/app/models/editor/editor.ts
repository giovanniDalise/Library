import { Book } from "../book/book";

export interface Editor {
    editorId:number;
    name:string;
    books:Book[];
}
