export interface PagedResponse<T> { 
  items: T[];
  totalRecords: number;
}