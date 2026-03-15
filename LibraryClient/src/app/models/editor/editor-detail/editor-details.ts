import { PagedResponse } from "../../pagination/paged-response";

export interface EditorDetail {
    id: number;
    name: string;
    books: PagedResponse<string>;
}