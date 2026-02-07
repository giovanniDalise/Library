import { Book } from "./book/book";

export class Author {
    authorId:number;
    name:string;
    surname:string;
    books: Book[];

    constructor(authorId:number, name:string, surname:string, books:Book[]){
        this.authorId = authorId;
        this.name = name;
        this.surname = surname;
        this.books = books;
    }

}
