export interface PaginatedResult<T>{
    items: T;
    pageNumber: number;
    pageSize: number;
    sortBy: number;
    sortOrder: number;
    totalCount: number;
    totalPages: number;
}