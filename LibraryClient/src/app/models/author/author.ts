import { Book } from "../book/book";

export interface Author {
    authorId:number;
    name:string;
    surname:string;
    books: Book[];
}
