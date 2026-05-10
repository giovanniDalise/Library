export interface BookDetail {
  id: number;
  title: string;
  isbn: string;
  coverReference: string;
  coverFile: File | null;
  authorName: string;
  authorSurname: string;
  editorName: string;
}