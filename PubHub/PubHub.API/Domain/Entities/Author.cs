﻿namespace PubHub.API.Domain.Entities
{
    public class Author
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public required string Name { get; set; }
        public bool IsDeleted { get; set; }

        #region Navs
        public IList<Book> Books { get; set; } = new List<Book>();
        #endregion 
    }
}
