using PubHub.Common.Models.Books;

namespace PubHub.BookMobile.Models
{
    public class BookListingViewModel
    {
        public BookInfoModel? EBook;
        public BookInfoModel? AudioBook;
        public ImageSource ListingCover => GetCoverPhoto();
        public string Title { get; set; } = null!;
        public DateOnly PublicationDate { get; set; }
        public bool EBookInStock => EBook != null;
        public bool AudiobookInStock => AudioBook != null;

        private ImageSource GetCoverPhoto()
        {
            var coverImage = ((EBookInStock) ? (EBook!.CoverImage) : (AudioBook!.CoverImage));

            if (coverImage == null)
                return ImageSource.FromFile("stock_image.jpg");

            return ImageSource.FromStream(() => new MemoryStream(coverImage));
        }
    }
}
