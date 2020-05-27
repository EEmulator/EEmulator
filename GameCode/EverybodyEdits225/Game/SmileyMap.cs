using System;
using System.Collections.Generic;
using System.Linq;
using EverybodyEdits.Common;
using EverybodyEdits.Lobby;

namespace EverybodyEdits.Game
{
    public class SmileyMap
    {
        private readonly Dictionary<int, string> auraColorVaultEnabled = new Dictionary<int, string>();
        private readonly Dictionary<int, string> auraShapeVaultEnabled = new Dictionary<int, string>();
        private readonly int[] beta = {6, 7, 8, 9, 10, 11};

        private readonly int[] freeForAll =
        {
            0, 1, 2, 3, 4, 5, 18, 117,
            14, 24, 19, 56, 53, 130, 51, 68, 64, 131, 36, 21, 138, 108, 116, 69, 76, 65, 58, 61, 107, 55, 109, 57, 52, 67, 66, 152, 163
        };

        private readonly Dictionary<int, string> vaultEnabled = new Dictionary<int, string>();

        public SmileyMap()
        {
            this.PopulateVaultEnabled();
            this.PopulateAuraValutEnabled();
        }

        private string GetSmileyVaultIdById(int id)
        {
            try
            {
                return this.vaultEnabled[id];
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetAuraShapeVaultIdById(int id)
        {
            try
            {
                return this.auraShapeVaultEnabled[id];
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GetAuraColorVaultIdById(int id)
        {
            try
            {
                return this.auraColorVaultEnabled[id];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool SmileyIsLegit(CommonPlayer player, int smileyId, Shop shop)
        {
            if (this.freeForAll.Contains(smileyId))
            {
                return true;
            }

            if (this.GetSmileyVaultIdById(smileyId) == "goldmember" && player.HasGoldMembership)
            {
                return true;
            }

            if (this.beta.Contains(smileyId) && player.HasBeta)
            {
                return true;
            }

            var smileyNameInVault = this.GetSmileyVaultIdById(smileyId);
            var shopItem = shop.GetShopItem(smileyNameInVault);

            return shopItem != null && player.PayVault.Has(smileyNameInVault);
        }

        public bool AuraIsLegit(CommonPlayer player, int auraId, int colorId, Shop shop)
        {
            return this.AuraPayVaultIsLegit(player, this.GetAuraShapeVaultIdById(auraId), shop) &&
                   this.AuraPayVaultIsLegit(player, this.GetAuraColorVaultIdById(colorId), shop);
        }

        private bool AuraPayVaultIsLegit(CommonPlayer player, string payVaultId, Shop shop)
        {
            if (payVaultId == null)
                return false;
            if (payVaultId == "" || payVaultId == "goldmember" && player.HasGoldMembership)
                return true;

            var shopItem = shop.GetShopItem(payVaultId);
            return shopItem != null && player.PayVault.Has(payVaultId);
        }

        private void PopulateVaultEnabled()
        {
            this.vaultEnabled.Add(12, "smileyninja");
            this.vaultEnabled.Add(13, "smileysanta");
            this.vaultEnabled.Add(15, "smileybigspender");
            this.vaultEnabled.Add(16, "smileysuper");
            this.vaultEnabled.Add(17, "smileysupprice");
            this.vaultEnabled.Add(20, "mixednewyear2010");
            this.vaultEnabled.Add(22, "smileywizard");
            this.vaultEnabled.Add(23, "smileyfanboy");
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
            this.vaultEnabled.Add(37, "smileynurse");
            this.vaultEnabled.Add(38, "smileyhw2011vampire");
            this.vaultEnabled.Add(39, "smileyhw2011ghost");
            this.vaultEnabled.Add(40, "smileyhw2011frankenstein");
            this.vaultEnabled.Add(41, "smileywitch");

            this.vaultEnabled.Add(42, "smileytg2011indian");
            this.vaultEnabled.Add(43, "smileytg2011pilgrim");
            this.vaultEnabled.Add(44, "smileypumpkin1");
            this.vaultEnabled.Add(45, "smileypumpkin2");
            this.vaultEnabled.Add(46, "smileyxmassnowman");
            this.vaultEnabled.Add(47, "smileyxmasreindeer");
            this.vaultEnabled.Add(48, "smileyxmasgrinch");
            this.vaultEnabled.Add(49, "bricknode");
            this.vaultEnabled.Add(50, "brickdrums");
            this.vaultEnabled.Add(54, "smileypurpleghost");
            this.vaultEnabled.Add(59, "smileydiver");
            this.vaultEnabled.Add(60, "smileytanned");
            this.vaultEnabled.Add(62, "smileyhardhat");
            this.vaultEnabled.Add(63, "smileygasmask");
            this.vaultEnabled.Add(70, "smileyalien");
            this.vaultEnabled.Add(71, "smileyastronaut");
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
            // 100 - Hologram
            this.vaultEnabled.Add(101, "smileygingerbread");
            this.vaultEnabled.Add(102, "smileycaroler");
            this.vaultEnabled.Add(103, "smileyelf");
            this.vaultEnabled.Add(104, "smileynutcracker");
            this.vaultEnabled.Add(105, "brickvalentines2015");
            this.vaultEnabled.Add(106, "smileyartist");
            this.vaultEnabled.Add(110, "smileyninjared");
            this.vaultEnabled.Add(111, "smiley3dglasses");
            this.vaultEnabled.Add(112, "smileysunburned");
            this.vaultEnabled.Add(113, "smileytourist");
            this.vaultEnabled.Add(114, "smileygraduate");
            this.vaultEnabled.Add(115, "smileysombrero");
            // 117 - Scared
            this.vaultEnabled.Add(118, "smileyghoul");
            this.vaultEnabled.Add(119, "smileymummy");
            this.vaultEnabled.Add(120, "smileybat");
            this.vaultEnabled.Add(121, "smileyeyeball");
            this.vaultEnabled.Add(122, "smileylightwizard");
            this.vaultEnabled.Add(123, "smileyhooded");
            this.vaultEnabled.Add(124, "smileyearmuffs");
            this.vaultEnabled.Add(125, "smileypenguin");
            this.vaultEnabled.Add(126, "goldmember");
            this.vaultEnabled.Add(127, "goldmember");
            this.vaultEnabled.Add(128, "goldmember");
            this.vaultEnabled.Add(129, "goldmember");
            this.vaultEnabled.Add(132, "smileygoofy");
            this.vaultEnabled.Add(133, "smileyraindrop");
            this.vaultEnabled.Add(134, "smileybee");
            this.vaultEnabled.Add(135, "smileybutterfly");
            this.vaultEnabled.Add(136, "smileyseacaptain");
            this.vaultEnabled.Add(137, "smileysodaclerk");
            this.vaultEnabled.Add(138, "smileylifeguard");
            this.vaultEnabled.Add(139, "smileyaviator");
            this.vaultEnabled.Add(140, "smileysleepy");
            this.vaultEnabled.Add(141, "smileyseagull");
            this.vaultEnabled.Add(142, "smileywerewolf");
            this.vaultEnabled.Add(143, "smileyswampcreature");
            this.vaultEnabled.Add(144, "smileyfairy");
            this.vaultEnabled.Add(145, "smileyfirefighter");
            this.vaultEnabled.Add(146, "smileyspy");
            this.vaultEnabled.Add(147, "smileydevilskull");

            this.vaultEnabled.Add(148, "smileyclockwork");
            this.vaultEnabled.Add(149, "smileyteddybear");
            this.vaultEnabled.Add(150, "smileychristmassoldier");
            this.vaultEnabled.Add(151, "smileyscrooge");

            this.vaultEnabled.Add(153, "smileypigtails");
            this.vaultEnabled.Add(154, "smileydoctor");
            this.vaultEnabled.Add(155, "smileyturban");
            this.vaultEnabled.Add(156, "smileyhazmatsuit");
            this.vaultEnabled.Add(157, "smileyleprechaun");
            this.vaultEnabled.Add(158, "smileyangry");
            this.vaultEnabled.Add(159, "smileysmirk");
            this.vaultEnabled.Add(160, "smileysweat");
            this.vaultEnabled.Add(161, "brickguitar");
            this.vaultEnabled.Add(162, "smileythor");
            this.vaultEnabled.Add(164, "smileyraccoon");
            this.vaultEnabled.Add(165, "smileylion");
            this.vaultEnabled.Add(166, "smileylaiika");
            this.vaultEnabled.Add(167, "smileyfishbowl");
            this.vaultEnabled.Add(168, "smileyslime");
            this.vaultEnabled.Add(169, "smileydesigner");
        }

        private void PopulateAuraValutEnabled()
        {
            this.auraShapeVaultEnabled.Add(0, "");
            this.auraShapeVaultEnabled.Add(1, "aurashapepinwheel");
            this.auraShapeVaultEnabled.Add(2, "aurashapetorus");
            this.auraShapeVaultEnabled.Add(3, "goldmember");
            this.auraShapeVaultEnabled.Add(4, "aurashapespiral");
            this.auraShapeVaultEnabled.Add(5, "aurashapestar");
            //this.auraShapeVaultEnabled.Add(6, "aurashapeelectric");

            this.auraColorVaultEnabled.Add(0, "");
            this.auraColorVaultEnabled.Add(1, "aurared");
            this.auraColorVaultEnabled.Add(2, "aurablue");
            this.auraColorVaultEnabled.Add(3, "aurayellow");
            this.auraColorVaultEnabled.Add(4, "auragreen");
            this.auraColorVaultEnabled.Add(5, "aurapurple");
            this.auraColorVaultEnabled.Add(6, "auraorange");
            this.auraColorVaultEnabled.Add(7, "auracyan");
            this.auraColorVaultEnabled.Add(8, "goldmember");
            this.auraColorVaultEnabled.Add(9, "aurapink");
            this.auraColorVaultEnabled.Add(10, "auraindigo");
            this.auraColorVaultEnabled.Add(11, "auralime");
            this.auraColorVaultEnabled.Add(12, "aurablack");
        }
    }
}
