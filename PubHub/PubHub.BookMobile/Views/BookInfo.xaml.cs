﻿using PubHub.BookMobile.ViewModels;

namespace PubHub.BookMobile.Pages;

public partial class BookInfo : ContentPage
{
    private readonly BookInfoViewModel _viewModel;
    public BookInfo(BookInfoViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;

        BindingContext = _viewModel;
    }
}
