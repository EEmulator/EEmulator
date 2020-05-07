using System;
using System.Collections.Generic;
using System.Linq;
using EverybodyEdits.Lobby;

namespace EverybodyEdits.Game
{
    public class SmileyMap
    {
        protected int[] beta = {6, 7, 8, 9, 10, 11};

        protected int[] buildersclub = {20, 49};
        // These smileys are included when you buy a pack. So builders club have access to them.

        protected Dictionary<int, string> exclusives = new Dictionary<int, string>();

        protected int[] freeForAll = {0, 1, 2, 3, 4, 5, 18 /*, 98*/};
        protected Dictionary<int, string> vaultEnabled = new Dictionary<int, string>();

        public SmileyMap()
        {
            this.populateVaultEnabled();
            this.populateExclusives();
        }

        private string getSmileyVaultIdById(int id)
        {
            try
            {
                return this.vaultEnabled[id];
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public bool smileyIsLegit(Player player, int smileyId, Shop shop)
        {
            if (this.freeForAll.Contains(smileyId))
            {
                return true;
            }
            if (this.beta.Contains(smileyId) && player.haveSmileyPackage)
            {
                return true;
            }
            if (this.buildersclub.Contains(smileyId) && player.isClubMember)
            {
                return true;
            }
            var smileyNameInVault = this.getSmileyVaultIdById(smileyId);
            var shopItem = shop.GetShopItem(smileyNameInVault);

            if (shopItem == null) return false;

            if (shopItem.MinClass <= player.level && player.PayVault.Has(smileyNameInVault))
            {
                return true;
            }

            return false;
        }

        /*
         How are these ever used?
         */

        private void populateExclusives()
        {
            this.exclusives.Add(41, "smileywitch");
            this.exclusives.Add(16, "smileysuper");
            this.exclusives.Add(23, "smileyfanboy");
            this.exclusives.Add(44, "smileypumpkin1");
            this.exclusives.Add(45, "smileypumpkin2");
            this.exclusives.Add(48, "smileyxmasgrinch");
            this.exclusives.Add(22, "smileywizard");
            this.exclusives.Add(32, "smileywizard2");
            this.exclusives.Add(34, "smileypostman");
            this.exclusives.Add(30, "smileybunni");
            this.exclusives.Add(29, "smileybird");
            this.exclusives.Add(94, "smileydarkwizard");
        }

        private void populateVaultEnabled()
        {
            this.vaultEnabled.Add(12, "smileyninja");
            this.vaultEnabled.Add(13, "smileysanta");
            this.vaultEnabled.Add(14, "smileyworker");
            this.vaultEnabled.Add(15, "smileybigspender");
            this.vaultEnabled.Add(16, "smileysuper");
            this.vaultEnabled.Add(17, "smileysupprice");
            this.vaultEnabled.Add(19, "smileygirl");
            this.vaultEnabled.Add(20, "mixednewyear2010");
            this.vaultEnabled.Add(21, "smileycoy");
            this.vaultEnabled.Add(22, "smileywizard");
            this.vaultEnabled.Add(23, "smileyfanboy");
            this.vaultEnabled.Add(24, "smileyterminator");
            this.vaultEnabled.Add(25, "smileyxd");
            this.vaultEnabled.Add(26, "smileybully");
            this.vaultEnabled.Add(27, "smileycommando");
            this.vaultEnabled.Add(28, "smileyvalentines2011");
            this.vaultEnabled.Add(29, "smileybird");
            this.vaultEnabled.Add(30, "smileybunni");

            this.vaultEnabled.Add(32, "smileywizard2");
            this.vaultEnabled.Add(33, "smileyxdp");
            this.vaultEnabled.Add(34, "smileypostman");
            this.vaultEnabled.Add(35, "smileytemplar");
            this.vaultEnabled.Add(36, "smileyangel");
            this.vaultEnabled.Add(37, "smileynurse");
            this.vaultEnabled.Add(38, "smileyhw2011vampire");
            this.vaultEnabled.Add(39, "smileyhw2011ghost");
            this.vaultEnabled.Add(40, "smileyhw2011frankenstein");
            this.vaultEnabled.Add(41, "smileywitch");
            ;
            this.vaultEnabled.Add(42, "smileytg2011indian");
            this.vaultEnabled.Add(43, "smileytg2011pilgrim");
            this.vaultEnabled.Add(44, "smileypumpkin1");
            this.vaultEnabled.Add(45, "smileypumpkin2");
            this.vaultEnabled.Add(46, "smileyxmassnowman");
            this.vaultEnabled.Add(47, "smileyxmasreindeer");
            this.vaultEnabled.Add(48, "smileyxmasgrinch");
            this.vaultEnabled.Add(49, "bricknode");

            this.vaultEnabled.Add(51, "smileysigh");
            this.vaultEnabled.Add(52, "smileyrobber");
            this.vaultEnabled.Add(53, "smileypolice");
            this.vaultEnabled.Add(54, "smileypurpleghost");
            this.vaultEnabled.Add(55, "smileypirate");
            this.vaultEnabled.Add(56, "smileyviking");
            this.vaultEnabled.Add(57, "smileykarate");
            this.vaultEnabled.Add(58, "smileycowboy");
            this.vaultEnabled.Add(59, "smileydiver");
            this.vaultEnabled.Add(60, "smileytanned");
            this.vaultEnabled.Add(61, "smileypropeller");
            this.vaultEnabled.Add(62, "smileyhardhat");
            this.vaultEnabled.Add(63, "smileygasmask");
            this.vaultEnabled.Add(64, "smileyrobot");
            this.vaultEnabled.Add(65, "smileypeasant");
            this.vaultEnabled.Add(66, "smileysoldier");
            this.vaultEnabled.Add(67, "smileyblacksmith");
            this.vaultEnabled.Add(68, "smileylaughing");
            this.vaultEnabled.Add(69, "smileylaika");
            this.vaultEnabled.Add(70, "smileyalien");
            this.vaultEnabled.Add(71, "smileyastronaut");
            this.vaultEnabled.Add(76, "smileycannon");
            this.vaultEnabled.Add(77, "smileymonster");
            this.vaultEnabled.Add(78, "smileyskeleton");
            this.vaultEnabled.Add(79, "smileymadscientist");
            this.vaultEnabled.Add(80, "smileyheadhunter");
            this.vaultEnabled.Add(81, "smileysafari");
            this.vaultEnabled.Add(82, "smileyarchaeologist");
            this.vaultEnabled.Add(83, "smileynewyear2012");
            this.vaultEnabled.Add(84, "smileywinter");
            this.vaultEnabled.Add(85, "smileyfiredeamon");
            this.vaultEnabled.Add(86, "smileybishop");
            this.vaultEnabled.Add(88, "smileyzombieslayer");
            this.vaultEnabled.Add(89, "smileyunit");
            this.vaultEnabled.Add(90, "smileyspartan");
            this.vaultEnabled.Add(91, "smileyhelen");
            this.vaultEnabled.Add(92, "smileycow");
            this.vaultEnabled.Add(93, "smileyscarecrow");
            this.vaultEnabled.Add(94, "smileydarkwizard");
            this.vaultEnabled.Add(95, "smileykungfumaster");
            this.vaultEnabled.Add(96, "smileyfox");
            this.vaultEnabled.Add(97, "smileynightvision");
            this.vaultEnabled.Add(98, "smileysummergirl");
            this.vaultEnabled.Add(99, "smileyfanboy2");
            // 100 is hologram smiley
            this.vaultEnabled.Add(101, "smileygingerbread");
            this.vaultEnabled.Add(102, "smileycaroler");
            this.vaultEnabled.Add(103, "smileyelf");
            this.vaultEnabled.Add(104, "smileynutcracker");
            this.vaultEnabled.Add(105, "brickvalentines2015");
        }
    }
}