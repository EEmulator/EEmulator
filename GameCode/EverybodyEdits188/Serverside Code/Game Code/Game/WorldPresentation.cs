using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    internal class WorldPresentation
    {
        private readonly Client _client;
        private DatabaseObject _presentationDbo;

        private uint viewportHeight = 5;
        private uint viewportWidth = 5;

        public WorldPresentation(Client c)
        {
            this._client = c;
        }

        public uint ViewportX { get; set; }
        public uint ViewportY { get; set; }

        public uint ViewportWidth
        {
            get { return 5; }
        }

        public uint ViewportHeight
        {
            get { return 5; }
        }

        public void load(string id, Callback onSuccess)
        {
            this._client.BigDB.LoadOrCreate("WorldPresentation", id, delegate(DatabaseObject o)
            {
                this._presentationDbo = o;
                onSuccess();
            });
        }

        public void save( /*List<DBBrick> mapData*/)
        {
            var viewport = new DatabaseObject();
            viewport.Set("x", this.ViewportX);
            viewport.Set("y", this.ViewportY);
            viewport.Set("width", this.viewportWidth);
            viewport.Set("height", this.viewportHeight);

            this._presentationDbo.Set("viewport", viewport);
            /*
            if (presentationDbo != null)
            {
                presentationDbo.Set("presentationMap", map
            }
             * */
        }
    }
}