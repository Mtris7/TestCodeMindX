using System.ComponentModel;

namespace TestCodeMindX.Model
{
    public class User : INotifyPropertyChanged
    {
        public string userName { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
