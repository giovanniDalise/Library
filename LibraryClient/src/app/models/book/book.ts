import { Author } from "../author/author";
import { Editor } from "../editor/editor/editor";

export interface Book {
    bookId: number;
    title: string;
    isbn: string;
    authors: Author[];
    editor: Editor;
    coverReference: string;
    coverFile: File | null;
}
