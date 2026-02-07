import { Book } from "./book";

export interface PagedBookResponse {
  bookResponse: Book[];
  totalRecords: number;
}