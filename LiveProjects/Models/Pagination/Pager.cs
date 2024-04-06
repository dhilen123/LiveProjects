using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiveProjects.Models.Pagination
{
    public class Pager
    {
        public Pager(int totalItems, int? page, int pageSize)
        {

            int _totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

            int _currentPage = page != null ? (int)page : 1;

            int _startPage = _currentPage - 5;

            int _endPage = _currentPage + 4;
            if (_startPage <= 0)
            {
                _endPage -= (_startPage - 1);
                _startPage = 1;
            }
            if (_endPage > _totalPages)
            {
                _endPage = _totalPages;
                if (_endPage > 10)
                {
                    _startPage = _endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = _currentPage;
            PageSize = pageSize;
            TotalPages = _totalPages;
            StartPage = _startPage;
            EndPage = _endPage;
        }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }
    }
}