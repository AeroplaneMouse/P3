using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Asset_Management_System.Database;
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
            DataContext = new ViewModels.HomeViewModel(main, InputBox);
            
            QueryGenerator query = new QueryGenerator();
            query.Columns.AddRange(new []{ "id", "name" });
            query.Values.AddRange(new[] { "2", "SejtNavn" });
            
            // Table
            query.AddTable("assets");
            
            /*
            Table table1 = new Table("asset_tags");
            table1.JoinType = "INNER JOIN";
            table1.AddConnection("assets.id", "assets_tags.asset_id");
            query.Tables.Add(table1);
            
            Table table2 = new Table("tags");
            table2.JoinType = "INNER JOIN";
            table2.AddConnection("tags.id", "assets_tags.tag_id");
            query.Tables.Add(table2);
            */
            
            // Where
            query.Where("name", "Thomas");
            query.Where("something", "10");
            
            Statement statement = new Statement();
            statement.AddOrStatement("name", "Thomas");
            statement.AddOrStatement("content", "Thomas");
            query.Where(statement);
            
  
            string queryResult = query.PrepareSelect();
            Console.WriteLine(queryResult);

        }
    }
}
