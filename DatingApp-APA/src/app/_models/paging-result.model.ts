export interface Pagination{
    totalItems :number,
    totalPages :number,
    currentPage :number,
    itemsPerPage:number
}

export class PaginatedResult<T>{
    result : T;
    pagination :Pagination
}