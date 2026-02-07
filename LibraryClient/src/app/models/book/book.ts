import { fromEvent } from "rxjs";
import { Author } from "../author";
import { Editor } from "../editor";

export interface Book {
    bookId: number;
    title: string;
    isbn: string;
    authors: Author[];
    editor: Editor;
    coverReference: string;
    coverFile: File | null;
}
