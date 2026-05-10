import { Author } from "../../author/author/author";
import { Editor } from "../../editor/editor/editor";

export interface BookDetail {
  id: number;
  title: string;
  isbn: string;
  coverReference: string;
  coverFile: File | null;
  authors: Author[];
  editor: Editor;
}