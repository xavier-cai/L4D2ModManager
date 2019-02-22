using System.ComponentModel;

namespace L4D2ModManager
{
    class ViewItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void Invoke(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public ViewItem(L4D2MM.ModInfo mod)
        {
            Mod = mod;
            ID = mod.Mod.FileName + (mod.Mod.Title.Length > 0? (" (" + mod.Mod.Title + ')') : "");
            //Source = mod.Source.GetString();
            //State = mod.ModState.GetString();
            //Size = mod.Mod.FileSize.ToFileSize();
        }

        public string Key { get { return Mod.Key; } }
        public L4D2MM.ModInfo Mod { get; private set; }

        public string _ID;
        public string ID { get { return _ID; } set { _ID = value; Invoke("ID"); } }
        /*
        public string _Source;
        public string Source { get { return _Source; } set { _Source = value; Invoke("Source"); } }
        public string _State;
        public string State { get { return _State; } set { _State = value; Invoke("State"); } }
        public string _Size;
        public string Size { get { return _Size; } set { _Size = value; Invoke("Size"); } }
        */
        public string Source { get { return Mod.Source.GetString(); } }
        public string State { get { return Mod.ModState.GetString(); } }
        public string Size { get { return Mod.Mod.FileSize.ToFileSize(); } }
    }
}
