using System.Collections.ObjectModel;
using PubHub.Common.Models.Books;
using PubHub.Common.Models.Genres;

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
        public ObservableCollection<GenreInfoModel> Genres => GetGenres();

        private ImageSource GetCoverPhoto()
        {
            var coverImage = ((EBookInStock) ? (EBook!.CoverImage) : (AudioBook!.CoverImage));

            if (coverImage == null)
                return ImageSource.FromFile("stock_image.jpg");

            return ImageSource.FromStream(() => new MemoryStream(coverImage));
        }

        private ObservableCollection<GenreInfoModel> GetGenres()
        {
            var genres = ((EBookInStock) ? (EBook?.Genres) : (AudioBook?.Genres)) ?? throw new NullReferenceException("Genres was null. That's not supposed to happen");

            return new ObservableCollection<GenreInfoModel>(genres);
        }
    }
}
