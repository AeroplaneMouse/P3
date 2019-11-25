using AMS.Controllers.Interfaces;
using AMS.Interfaces;
using AMS.Models;
using AMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMS.Views
{
    /// <summary>
    /// Interaction logic for AssetPresenter.xaml
    /// </summary>
    public partial class AssetPresenter : Page
    {
        public AssetPresenter(Asset asset, List<ITagable> tagList, ICommentListController commentListController, ILogListController logListController)
        {
            InitializeComponent();
            DataContext = new AssetPresenterViewModel(asset, tagList, commentListController, logListController);
        }
    }
}
