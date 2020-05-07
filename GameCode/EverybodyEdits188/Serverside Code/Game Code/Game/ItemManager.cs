using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    internal class ItemManager
    {
        //private static Hashtable payvaultItems = new Hashtable();

        public static Potion GetPotion(int id)
        {
            switch (id)
            {
                case 1:
                {
                    return new Potion(id, "RedArua", "potionaurared", toSeconds(0, 0, 45), "aura");
                }
                case 2:
                {
                    return new Potion(id, "BlueArua", "potionaurablue", toSeconds(0, 0, 15), "aura");
                }
                case 3:
                {
                    return new Potion(id, "YellowArua", "potionaurayellow", toSeconds(0, 0, 30), "aura");
                }
                case 4:
                {
                    return new Potion(id, "Jump", "potionjump", toSeconds(0, 0, 0, 30), "jump");
                }
                case 5:
                {
                    return new Potion(id, "GreenAura", "potionauragreen", toSeconds(0, 0, 20), "aura");
                }
                case 6:
                {
                    return new Potion(id, "Curse", "potioncurse", toSeconds(0, 0, 1), "curse");
                }
                case 7:
                {
                    return new Potion(id, "Fire", "potionfire", toSeconds(0, 0, 10), "fire");
                }
                case 8:
                {
                    return new Potion(id, "Protection", "potionprotection", toSeconds(0, 0, 0, 30), "protection");
                }
                case 9:
                {
                    return new Potion(id, "Zombie", "potionzombie", toSeconds(0, 0, 1, 0), "zombie");
                }
                case 10:
                {
                    return new Potion(id, "Respawn", "potionrespawn", toSeconds(0, 0, 0, 2), "instant");
                }
                case 11:
                {
                    return new Potion(id, "Levitation", "potionlevitation", toSeconds(0, 0, 1, 0), "levitation");
                }
                case 12:
                {
                    return new Potion(id, "Flaunt", "potionflaunt", toSeconds(0, 0, 0, 30), "flaunt");
                }
                case 13:
                {
                    return new Potion(id, "Solitude", "potionsolitude", toSeconds(0, 0, 5, 0), "solitude");
                }
                case 14:
                {
                    return new Potion(id, "Speed", "potionspeed", toSeconds(0, 0, 0, 30), "speed");
                }
            }

            return null;
        }

        public static void AddPotionCountToMessage(Player player, Message m)
        {
            m.Add("ps");

            var i = 1;
            var searching = true;
            while (searching)
            {
                var potion = GetPotion(i);
                if (potion != null)
                {
                    m.Add(potion.id, player.PayVault.Count(potion.payvatultid));
                }
                else
                {
                    searching = false;
                }
                i++;
            }
            m.Add("pe");
        }

        public static int toSeconds(int days, int hours, int minutes, int seconds = 0)
        {
            hours += days * 24;
            minutes += hours * 60;
            seconds += minutes * 60;
            return seconds;
        }

        /*
        public static void getPayvaultItemById(Client client, string payvaultId, Callback<PayvaultItem> callback)
        {
            if (payvaultItems.Count != 0)
            {
                callback( (PayvaultItem)payvaultItems[payvaultId] );
            }

            client.BigDB.LoadRange("PayVaultItems", "PriceCoins", null, null, null, 1000, delegate(DatabaseObject[] ob)
            {

                // Add items to hashtable (cache them) 
                foreach (DatabaseObject pObj in ob)
                {
                    payvaultItems.Add(
                            pObj.Key, new PayvaultItem()
                            {
                                MinClass = pObj.GetInt("MinClass"),

                            }
                        );
                    callback((PayvaultItem)payvaultItems[payvaultId]);
                }
            });

        }
        */
    }


    // *********************************************************************************
    //  PayvaultItem
    // *********************************************************************************
    /*
    public class PayvaultItem {
        public string Key { get; set; }
        public int PriceCoins { get; set; }
        public int PriceEnergy { get; set; }
        public int EnergyPerClick { get; set; }
        public Boolean Reusable { get; set; }
        public Boolean BetaOnly { get; set; }
        public Boolean IsFeatured { get; set; }
        public Boolean IsClassic { get; set; }
        public Boolean Enabled { get; set; }
        public Boolean OnSale { get; set; }
        public int Span { get; set; }
        public int Header { get; set; }
        public int Body { get; set; }
        public int BitmapSheetId { get; set; }

        public int MinClass { get; set; }

    }*/

    // *********************************************************************************
    //  Potion
    // *********************************************************************************

    public class Potion : IEquatable<Potion>
    {
        public DateTime activated;
        public bool broadcasted;
        public int duration;
        // Potions in the same group can not be activate at the same time
        public string group;
        public int id;
        public int level;
        public string name;
        public string payvatultid;

        // Used in the game to see if the potion is new (fx. when loaded from another world)

        public Potion(int id, string name, string payvatultid, int duration, string group)
        {
            this.id = id;
            this.payvatultid = payvatultid;
            this.name = name;
            this.duration = duration;
            this.group = group;
        }

        public double timeleft
        {
            get { return (this.activated.AddSeconds(this.duration) - DateTime.Now).TotalSeconds; }
        }

        public bool expired
        {
            get { return this.timeleft <= 0; }
        }

        public bool Equals(Potion other)
        {
            return this.id == other.id;
        }

        public void activate()
        {
            this.activated = DateTime.Now;
        }

        public static Potion createPotion(DatabaseObject data)
        {
            var potion = new Potion(data.GetInt("id", 0), data.GetString("name", ""), data.GetString("payvatultid", ""),
                data.GetInt("duration", 0), data.GetString("group", ""));
            potion.activated = data.GetDateTime("activated");
            return potion;
        }

        public static DatabaseObject createDatabaseObject(Potion potion)
        {
            var data = new DatabaseObject();
            data.Set("id", potion.id);
            data.Set("name", potion.name);
            data.Set("payvatultid", potion.payvatultid);
            data.Set("duration", potion.duration);
            data.Set("activated", potion.activated);
            data.Set("group", potion.group);
            return data;
        }
    }
}