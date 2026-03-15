import { Book } from "../../book/book";

export interface Editor {
    id:number;
    name:string;
    books:Book[];
}
