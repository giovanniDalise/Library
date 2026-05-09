export interface BookRequest {
  id?: number;
  title?: string;
  isbn?: string;
  editor?: {
    editorId?: number;
    name?: string;
  };
  authors?: {
    id?: number;
    name?: string;
    surname?: string;
  }[];
}