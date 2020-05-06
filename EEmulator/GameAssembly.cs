namespace EEmulator
{
    public class GameAssembly
    {
        public string Dll { get; set; }
        public string Pdb { get; set; }

        public GameAssembly(string dll, string pdb)
        {
            this.Dll = dll;
            this.Pdb = pdb;
        }
    }
}
