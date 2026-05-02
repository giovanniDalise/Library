import { PagedResponse } from "../../pagination/paged-response";

export interface AuthorDetail{
    id: number;
    name: string;
    surname: string;
    books: PagedResponse<string>;
}