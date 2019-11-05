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
            /*
            UserRepository userRepository = new UserRepository();
            
            Asset asset = assetRepository.GetById(4);
            
            List<ITagable> tags = (List<ITagable>) assetRepository.GetTags(asset);

            foreach (var tag in tags)
            {
                Console.WriteLine(tag.TagLabel());
            }

            Tag tag1 = tagRepository.GetById(14);
            Tag tag2 = tagRepository.GetById(15);
            User user1 = userRepository.GetById(1);

            List<ITagable> listOfTags = new List<ITagable> {tag1, tag2, user1};

            assetRepository.AttachTags(asset, listOfTags);
            */
            
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
            tagger.Parent(placering); // Switch to placering
            result = tagger.Suggest("Løve");
            
            foreach (var item in result)
            {
                Console.WriteLine(item.TagLabel());
            }
        }
    }
}
