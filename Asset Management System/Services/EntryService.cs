using System.Drawing.Imaging;
using System.Windows.Controls;
using Asset_Management_System.Database.Repositories;
using Asset_Management_System.Logging;
using Asset_Management_System.Services.Interfaces;
using Asset_Management_System.ViewModels;
using Asset_Management_System.Views;

namespace Asset_Management_System.Services
{
    public class EntryService : IEntryService
    {
        
        private ILogRepository _rep;

        public EntryService(ILogRepository rep)
        {
            _rep = rep;
        }

        public ISearchableRepository<Entry> GetSearchableRepository() => _rep;
        
        public IRepository<Entry> GetRepository() => _rep;

        public Page GetManagerPage(MainViewModel main, Entry inputAsset = default, bool addMultiple = false)
        {
            // Entry does not have a manager page, and thus this should never be called.
            // But if it does it returns logs page
            return new Logs(main, this);
        }

        public string GetName(Entry entry) => entry.ID.ToString();

    }
}