import { Book } from "../book/book";

export interface Author {
    id:number;
    name:string;
    surname:string;
    books: Book[];
}
