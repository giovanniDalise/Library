export interface BookRequest {
  bookId?: number;
  title?: string;
  isbn?: string;
  editor?: {
    editorId?: number;
    name?: string;
  };
  authors?: {
    authorId?: number;
    name?: string;
    surname?: string;
  }[];
}