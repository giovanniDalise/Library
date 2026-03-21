import { Author } from "../author/author";
import { Editor } from "../editor/editor/editor";

export interface Book {
    id: number;
    title: string;
    isbn: string;
    authors: Author[];
    editor: Editor;
    coverReference: string;
    coverFile: File | null;
}
