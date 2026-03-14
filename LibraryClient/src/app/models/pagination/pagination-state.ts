export class PaginationState {

  pageSize = 10;
  currentPage = 1;
  totalRecords = 0;

  get totalPages(): number {
    return Math.ceil(this.totalRecords / this.pageSize);
  }

  next(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  prev(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  reset(): void {
    this.currentPage = 1;
  }

}