using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Functions;
using Asset_Management_System.Models;
using Asset_Management_System.ViewModels;

namespace Asset_Management_System.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home(MainViewModel main)
        {
            InitializeComponent();
            DataContext = new ViewModels.HomeViewModel(main);

            AssetRepository assetRepository = new AssetRepository();
            TagRepository tagRepository = new TagRepository();

            Tagging tagger = new Tagging();
            Tag tag = tagRepository.GetById(1); // User group
            
            tagger.Parent(tag); // Switch to user search
            List<ITagable> result = tagger.Suggest("jo");
            
            foreach (var item in result)
            {
                Console.WriteLine(item.TagLabel());
            }
            
            tagger.Parent(null); // Switch to user search
            result = tagger.Suggest("sw");

            foreach (var item in result)
            {
                Console.WriteLine(item.TagLabel());
            }

            Tag placering = tagRepository.GetById(2);
            Tag lovehuset = tagRepository.GetById(4);
            tagger.Parent(placering); // Switch to placering
            tagger.AddToQuery(lovehuset);
            //tagger.RemoveFromQuery(lovehuset);
            result = tagger.Suggest("Løve");
            
            foreach (var item in result)
            {
                Console.WriteLine(item.TagLabel());
            }
        }
    }
}
