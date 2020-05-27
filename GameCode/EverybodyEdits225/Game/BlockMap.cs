using System.Collections.Generic;
using System.Linq;
using System;

namespace EverybodyEdits.Game
{
    public class BlockMap
    {
        private readonly int[] _beta = { 37, 38, 1019, 39, 40, 1020, 41, 42, 1021, 1089, 743, 744, 745, 746, 747, 748, 749, 750, 751, 752 };

        private readonly int[] _goldmembers = {1065, 1066, 1067, 1068, 1069, 709, 710, 711, 200, 201};

        private readonly int[] _freeForAll =
        {
            // Gravity
            0, 1, 2, 3, 4, 411, 412, 413, 414, 459, 460, 1518, 1519,
            // Crown
            5,
            // Keys
            6, 7, 8, 408, 409, 410,
            // Spawn
            255,
            // Basic 
            9, 10, 11, 12, 1018, 13, 14, 15, 182, 1088,
            // Brick 
            1022, 16, 1023, 17, 18, 19, 20, 21, 1024, 1090,
            // Checker
            186, 187, 188, 189, 190, 191, 192, 1025, 1026, 1091,
            // Doors
            23, 24, 25, 1005, 1006, 1007,
            // Gates
            26, 27, 28, 1008, 1009, 1010,
            // Metal
            29, 30, 31,
            // Grass
            34, 35, 36,
            // Generic
            22, 32, 33, 1057, 1058,
            // Coins
            100, 101,
            // Basic bg
            500, 501, 502, 503, 644, 504, 505, 506, 645, 715,
            // Brick bg
            646, 507, 508, 647, 509, 510, 511, 512, 648, 716,
            // Normal bg
            610, 611, 612, 613, 614, 615, 616, 653, 654, 717,
            // Checker bg
            513, 514, 515, 516, 517, 518, 519, 649, 650, 718,
            // Windows
            262, 263, 264, 265, 266, 267, 268, 269, 270,
            // Factory
            45, 46, 47, 48, 49,
            // Environment
            678, 679, 680, 681, 682, 1030, 1031, 1032, 1033, 1034,
            // Dark Backgrounds
            520, 521, 522, 523, 651, 524, 525, 526, 652, 719, 
            // Arctic
            1059, 1060, 1061, 1062, 1063, 702, 703, 
            // Desert
            177, 178, 179, 180, 181, 336, 425, 426, 427, 699, 700, 701,
            // Clay
            594, 595, 596, 597, 598,
            // Water
            119, 300, 574, 575, 576, 577, 578,
            // Secret Blocks
            44, 50, 136, 243,
            // Cloud
            143, 311, 312, 313, 314, 315, 316, 317, 318,
            // Dark Cloud
            1126, 1523, 1524, 1525, 1526, 1527, 1528, 1529, 1530,
            // Glass
            51, 52, 53, 54, 55, 56, 57, 58,
            // Pastel
            527, 528, 529, 530, 531, 532, 676, 677,
            // Outer Space
            172, 173, 174, 175, 176, 1029, 601, 602, 603, 604, 331, 332, 333, 334, 335, 428, 429, 430,
            // Jungle
            193, 194, 195, 196, 197, 198, 617, 618, 619, 620, 98, 99, 88, 199, 621, 622, 623, 357, 358, 359,
            // Pipes
            166, 167, 168, 169, 170, 171,
            // Construction
            1096, 1097, 1098, 1099, 1100, 1503, 1504, 1505, 728, 729, 730, 731, 732, 1128, 1129, 1130, 1131, 1532, 1533, 753, 754, 755, 756
        };

        // Bricks not allowed in open worlds
        private readonly int[] _openworldsAntiSubset =
        {
            421, 418, 417, 453, 461, 420, 419, 422, 207, 206, 385, 242, 381, 374, 255, 360, 121,
            466, 397, 241, 423, 1027, 1028, 156, 157, 200, 201, 460, 411, 412, 413, 414, 43, 165,
            213, 214, 467, 113, 1080, 185, 1079, 184, 1000, 368, 361, 416, 1011, 1012, 136, 50,
            243, 337
        };

        private readonly Dictionary<int, string> _vaultEnabled = new Dictionary<int, string>();

        public BlockMap()
        {
            this.PopulateVaultEnabled();
        }

        public int GetBlockLayerById(int id)
        {
            // In the client, a brick is placed in the background if id is between 500 and 1000
            return id >= 500 && id < 1000 || (id > 2506 && id < 2514) ? 1 : 0; //math later
        }

        private string GetBlockVaultIdById(int id)
        {
            try
            {
                return this._vaultEnabled[id];
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public bool IsBlockBeta(int id)
        {
            return new List<int>(this._beta).Contains(id);
        }

        public bool BlockIsLegitOpenWorld(Player player, int blockid)
        {
            return !this._openworldsAntiSubset.Contains(blockid) && this.BlockIsLegit(player, blockid);
        }

        public bool BlockIsLegit(Player player, int blockid)
        {
            if (player.IsAdmin)
            {
                return true;
            }
            if (this._freeForAll.Contains(blockid))
            {
                return true;
            }
            if (this._beta.Contains(blockid) && player.HasSmileyPackage)
            {
                return true;
            }
            if (this._goldmembers.Contains(blockid) && player.HasGoldMembership)
            {
                return true;
            }
            var blockNameInVault = this.GetBlockVaultIdById(blockid);

            if (player.HasBrickPack(blockNameInVault))
            {
                return true;
            }
            if (blockNameInVault != string.Empty)
            {
                Console.WriteLine("Not available to user: " + blockid);
            }

            return false;
        }

        private void PopulateVaultEnabled()
        {
            this._vaultEnabled.Add(60, "brickcandy");
            this._vaultEnabled.Add(61, "brickcandy");
            this._vaultEnabled.Add(62, "brickcandy");
            this._vaultEnabled.Add(63, "brickcandy");
            this._vaultEnabled.Add(64, "brickcandy");
            this._vaultEnabled.Add(65, "brickcandy");
            this._vaultEnabled.Add(66, "brickcandy");
            this._vaultEnabled.Add(67, "brickcandy");
            this._vaultEnabled.Add(227, "brickcandy");
            this._vaultEnabled.Add(431, "brickcandy");
            this._vaultEnabled.Add(432, "brickcandy");
            this._vaultEnabled.Add(433, "brickcandy");
            this._vaultEnabled.Add(434, "brickcandy");
            this._vaultEnabled.Add(539, "brickcandy");
            this._vaultEnabled.Add(540, "brickcandy");

            this._vaultEnabled.Add(70, "brickminiral");
            this._vaultEnabled.Add(71, "brickminiral");
            this._vaultEnabled.Add(72, "brickminiral");
            this._vaultEnabled.Add(73, "brickminiral");
            this._vaultEnabled.Add(74, "brickminiral");
            this._vaultEnabled.Add(75, "brickminiral");
            this._vaultEnabled.Add(76, "brickminiral");

            this._vaultEnabled.Add(68, "brickhw2011");
            this._vaultEnabled.Add(69, "brickhw2011");
            this._vaultEnabled.Add(224, "brickhw2011");
            this._vaultEnabled.Add(225, "brickhw2011");
            this._vaultEnabled.Add(226, "brickhw2011");
            this._vaultEnabled.Add(541, "brickhw2011");
            this._vaultEnabled.Add(542, "brickhw2011");
            this._vaultEnabled.Add(543, "brickhw2011");
            this._vaultEnabled.Add(544, "brickhw2011");

            this._vaultEnabled.Add(223, "brickhwtrophy");

            //XMAS 2011
            this._vaultEnabled.Add(78, "brickxmas2011");
            this._vaultEnabled.Add(79, "brickxmas2011");
            this._vaultEnabled.Add(80, "brickxmas2011");
            this._vaultEnabled.Add(81, "brickxmas2011");
            this._vaultEnabled.Add(82, "brickxmas2011");
            this._vaultEnabled.Add(218, "brickxmas2011");
            this._vaultEnabled.Add(219, "brickxmas2011");
            this._vaultEnabled.Add(220, "brickxmas2011");
            this._vaultEnabled.Add(221, "brickxmas2011");
            this._vaultEnabled.Add(222, "brickxmas2011");

            this._vaultEnabled.Add(84, "brickscifi");
            this._vaultEnabled.Add(85, "brickscifi");
            this._vaultEnabled.Add(86, "brickscifi");
            this._vaultEnabled.Add(87, "brickscifi");
            this._vaultEnabled.Add(88, "brickscifi");
            this._vaultEnabled.Add(89, "brickscifi");
            this._vaultEnabled.Add(90, "brickscifi");
            this._vaultEnabled.Add(91, "brickscifi");
            this._vaultEnabled.Add(375, "brickscifi");
            this._vaultEnabled.Add(376, "brickscifi");
            this._vaultEnabled.Add(377, "brickscifi");
            this._vaultEnabled.Add(378, "brickscifi");
            this._vaultEnabled.Add(379, "brickscifi");
            this._vaultEnabled.Add(438, "brickscifi");
            this._vaultEnabled.Add(439, "brickscifi");
            this._vaultEnabled.Add(637, "brickscifi");
            this._vaultEnabled.Add(380, "brickscifi");
            this._vaultEnabled.Add(1051, "brickscifi");

            this._vaultEnabled.Add(545, "brickbgcarnival");
            this._vaultEnabled.Add(546, "brickbgcarnival");
            this._vaultEnabled.Add(547, "brickbgcarnival");
            this._vaultEnabled.Add(548, "brickbgcarnival");
            this._vaultEnabled.Add(549, "brickbgcarnival");
            this._vaultEnabled.Add(558, "brickbgcarnival");
            this._vaultEnabled.Add(563, "brickbgcarnival");
            this._vaultEnabled.Add(607, "brickbgcarnival");

            this._vaultEnabled.Add(256, "brickeaster2012");
            this._vaultEnabled.Add(257, "brickeaster2012");
            this._vaultEnabled.Add(258, "brickeaster2012");
            this._vaultEnabled.Add(259, "brickeaster2012");
            this._vaultEnabled.Add(260, "brickeaster2012");

            this._vaultEnabled.Add(261, "brickprison");
            this._vaultEnabled.Add(92, "brickprison");
            this._vaultEnabled.Add(550, "brickprison");
            this._vaultEnabled.Add(551, "brickprison");
            this._vaultEnabled.Add(552, "brickprison");
            this._vaultEnabled.Add(553, "brickprison");

            this._vaultEnabled.Add(93, "brickpirate");
            this._vaultEnabled.Add(94, "brickpirate");
            this._vaultEnabled.Add(154, "brickpirate");
            this._vaultEnabled.Add(271, "brickpirate");
            this._vaultEnabled.Add(272, "brickpirate");
            this._vaultEnabled.Add(435, "brickpirate");
            this._vaultEnabled.Add(436, "brickpirate");
            this._vaultEnabled.Add(554, "brickpirate");
            this._vaultEnabled.Add(555, "brickpirate");
            this._vaultEnabled.Add(559, "brickpirate");
            this._vaultEnabled.Add(560, "brickpirate");

            this._vaultEnabled.Add(95, "brickstone");
            this._vaultEnabled.Add(561, "brickstone");
            this._vaultEnabled.Add(562, "brickstone");
            this._vaultEnabled.Add(688, "brickstone");
            this._vaultEnabled.Add(689, "brickstone");
            this._vaultEnabled.Add(690, "brickstone");
            this._vaultEnabled.Add(691, "brickstone");
            this._vaultEnabled.Add(692, "brickstone");
            this._vaultEnabled.Add(693, "brickstone");
            this._vaultEnabled.Add(1044, "brickstone");
            this._vaultEnabled.Add(1045, "brickstone");
            this._vaultEnabled.Add(1046, "brickstone");

            this._vaultEnabled.Add(96, "brickninja");
            this._vaultEnabled.Add(97, "brickninja");
            this._vaultEnabled.Add(120, "brickninja");
            this._vaultEnabled.Add(276, "brickninja");
            this._vaultEnabled.Add(277, "brickninja");
            this._vaultEnabled.Add(278, "brickninja");
            this._vaultEnabled.Add(279, "brickninja");
            this._vaultEnabled.Add(280, "brickninja");
            this._vaultEnabled.Add(281, "brickninja");
            this._vaultEnabled.Add(282, "brickninja");
            this._vaultEnabled.Add(283, "brickninja");
            this._vaultEnabled.Add(284, "brickninja");
            this._vaultEnabled.Add(564, "brickninja");
            this._vaultEnabled.Add(565, "brickninja");
            this._vaultEnabled.Add(566, "brickninja");
            this._vaultEnabled.Add(567, "brickninja");
            this._vaultEnabled.Add(667, "brickninja");
            this._vaultEnabled.Add(668, "brickninja");
            this._vaultEnabled.Add(669, "brickninja");
            this._vaultEnabled.Add(670, "brickninja");

            this._vaultEnabled.Add(122, "brickcowboy");
            this._vaultEnabled.Add(123, "brickcowboy");
            this._vaultEnabled.Add(124, "brickcowboy");
            this._vaultEnabled.Add(125, "brickcowboy");
            this._vaultEnabled.Add(126, "brickcowboy");
            this._vaultEnabled.Add(127, "brickcowboy");
            this._vaultEnabled.Add(568, "brickcowboy");
            this._vaultEnabled.Add(569, "brickcowboy");
            this._vaultEnabled.Add(570, "brickcowboy");
            this._vaultEnabled.Add(571, "brickcowboy");
            this._vaultEnabled.Add(572, "brickcowboy");
            this._vaultEnabled.Add(573, "brickcowboy");
            this._vaultEnabled.Add(285, "brickcowboy");
            this._vaultEnabled.Add(286, "brickcowboy");
            this._vaultEnabled.Add(287, "brickcowboy");
            this._vaultEnabled.Add(288, "brickcowboy");
            this._vaultEnabled.Add(289, "brickcowboy");
            this._vaultEnabled.Add(290, "brickcowboy");
            this._vaultEnabled.Add(291, "brickcowboy");
            this._vaultEnabled.Add(292, "brickcowboy");
            this._vaultEnabled.Add(293, "brickcowboy");
            this._vaultEnabled.Add(294, "brickcowboy");
            this._vaultEnabled.Add(295, "brickcowboy");
            this._vaultEnabled.Add(296, "brickcowboy");
            this._vaultEnabled.Add(297, "brickcowboy");
            this._vaultEnabled.Add(298, "brickcowboy");
            this._vaultEnabled.Add(299, "brickcowboy");
            this._vaultEnabled.Add(424, "brickcowboy");
            this._vaultEnabled.Add(1521, "brickcowboy");
            this._vaultEnabled.Add(1522, "brickcowboy");

            this._vaultEnabled.Add(128, "brickplastic");
            this._vaultEnabled.Add(129, "brickplastic");
            this._vaultEnabled.Add(130, "brickplastic");
            this._vaultEnabled.Add(131, "brickplastic");
            this._vaultEnabled.Add(132, "brickplastic");
            this._vaultEnabled.Add(133, "brickplastic");
            this._vaultEnabled.Add(134, "brickplastic");
            this._vaultEnabled.Add(135, "brickplastic");

            this._vaultEnabled.Add(137, "bricksand");
            this._vaultEnabled.Add(138, "bricksand");
            this._vaultEnabled.Add(139, "bricksand");
            this._vaultEnabled.Add(140, "bricksand");
            this._vaultEnabled.Add(141, "bricksand");
            this._vaultEnabled.Add(142, "bricksand");
            this._vaultEnabled.Add(579, "bricksand");
            this._vaultEnabled.Add(580, "bricksand");
            this._vaultEnabled.Add(581, "bricksand");
            this._vaultEnabled.Add(582, "bricksand");
            this._vaultEnabled.Add(583, "bricksand");
            this._vaultEnabled.Add(584, "bricksand");
            this._vaultEnabled.Add(301, "bricksand");
            this._vaultEnabled.Add(302, "bricksand");
            this._vaultEnabled.Add(303, "bricksand");
            this._vaultEnabled.Add(304, "bricksand");
            this._vaultEnabled.Add(305, "bricksand");
            this._vaultEnabled.Add(306, "bricksand");

            this._vaultEnabled.Add(307, "bricksummer2012");
            this._vaultEnabled.Add(308, "bricksummer2012");
            this._vaultEnabled.Add(309, "bricksummer2012");
            this._vaultEnabled.Add(310, "bricksummer2012");

            this._vaultEnabled.Add(144, "brickindustrial");
            this._vaultEnabled.Add(145, "brickindustrial");
            this._vaultEnabled.Add(146, "brickindustrial");
            this._vaultEnabled.Add(147, "brickindustrial");
            this._vaultEnabled.Add(148, "brickindustrial");
            this._vaultEnabled.Add(149, "brickindustrial");
            this._vaultEnabled.Add(150, "brickindustrial");
            this._vaultEnabled.Add(151, "brickindustrial");
            this._vaultEnabled.Add(152, "brickindustrial");
            this._vaultEnabled.Add(153, "brickindustrial");
            this._vaultEnabled.Add(319, "brickindustrial");
            this._vaultEnabled.Add(320, "brickindustrial");
            this._vaultEnabled.Add(321, "brickindustrial");
            this._vaultEnabled.Add(322, "brickindustrial");
            this._vaultEnabled.Add(323, "brickindustrial");
            this._vaultEnabled.Add(324, "brickindustrial");
            this._vaultEnabled.Add(585, "brickindustrial");
            this._vaultEnabled.Add(586, "brickindustrial");
            this._vaultEnabled.Add(587, "brickindustrial");
            this._vaultEnabled.Add(588, "brickindustrial");
            this._vaultEnabled.Add(589, "brickindustrial");
            this._vaultEnabled.Add((int)ItemTypes.MetalLadder, "brickindustrial");
            this._vaultEnabled.Add(1127, "brickindustrial");
            this._vaultEnabled.Add(1133, "brickindustrial");
            this._vaultEnabled.Add((int)ItemTypes.IndustrialPipeThin, "brickindustrial");
            this._vaultEnabled.Add((int)ItemTypes.IndustrialPipeThick, "brickindustrial");
            this._vaultEnabled.Add((int)ItemTypes.IndustrialTable, "brickindustrial");

            this._vaultEnabled.Add(118, "brickmedieval");
            this._vaultEnabled.Add(158, "brickmedieval");
            this._vaultEnabled.Add(159, "brickmedieval");
            this._vaultEnabled.Add(160, "brickmedieval");
            this._vaultEnabled.Add(161, "brickmedieval");
            this._vaultEnabled.Add(162, "brickmedieval");
            this._vaultEnabled.Add(163, "brickmedieval");
            this._vaultEnabled.Add(330, "brickmedieval");
            this._vaultEnabled.Add(325, "brickmedieval");
            this._vaultEnabled.Add(326, "brickmedieval");
            this._vaultEnabled.Add(437, "brickmedieval");
            this._vaultEnabled.Add(556, "brickmedieval");
            this._vaultEnabled.Add(590, "brickmedieval");
            this._vaultEnabled.Add(591, "brickmedieval");
            this._vaultEnabled.Add(592, "brickmedieval");
            this._vaultEnabled.Add(593, "brickmedieval");
            this._vaultEnabled.Add(599, "brickmedieval");
            this._vaultEnabled.Add(600, "brickmedieval");
            this._vaultEnabled.Add((int) ItemTypes.MedievalAxe, "brickmedieval");
            this._vaultEnabled.Add((int) ItemTypes.MedievalBanner, "brickmedieval");
            this._vaultEnabled.Add((int) ItemTypes.MedievalCoatOfArms, "brickmedieval");
            this._vaultEnabled.Add((int) ItemTypes.MedievalShield, "brickmedieval");
            this._vaultEnabled.Add((int) ItemTypes.MedievalSword, "brickmedieval");
            this._vaultEnabled.Add((int) ItemTypes.MedievalTimber, "brickmedieval");

            this._vaultEnabled.Add(114, "brickboost");
            this._vaultEnabled.Add(115, "brickboost");
            this._vaultEnabled.Add(116, "brickboost");
            this._vaultEnabled.Add(117, "brickboost");

            this._vaultEnabled.Add(274, "brickmonster");
            this._vaultEnabled.Add(338, "brickmonster");
            this._vaultEnabled.Add(339, "brickmonster");
            this._vaultEnabled.Add(340, "brickmonster");
            this._vaultEnabled.Add(341, "brickmonster");
            this._vaultEnabled.Add(342, "brickmonster");
            this._vaultEnabled.Add(608, "brickmonster");
            this._vaultEnabled.Add(609, "brickmonster");
            this._vaultEnabled.Add(663, "brickmonster");
            this._vaultEnabled.Add(664, "brickmonster");
            this._vaultEnabled.Add(665, "brickmonster");
            this._vaultEnabled.Add(666, "brickmonster");

            this._vaultEnabled.Add(343, "brickfog");
            this._vaultEnabled.Add(344, "brickfog");
            this._vaultEnabled.Add(345, "brickfog");
            this._vaultEnabled.Add(346, "brickfog");
            this._vaultEnabled.Add(347, "brickfog");
            this._vaultEnabled.Add(348, "brickfog");
            this._vaultEnabled.Add(349, "brickfog");
            this._vaultEnabled.Add(350, "brickfog");
            this._vaultEnabled.Add(351, "brickfog");

            this._vaultEnabled.Add(352, "brickhw2012");
            this._vaultEnabled.Add(353, "brickhw2012");
            this._vaultEnabled.Add(354, "brickhw2012");
            this._vaultEnabled.Add(355, "brickhw2012");
            this._vaultEnabled.Add(356, "brickhw2012");

            this._vaultEnabled.Add(202, "bricklava");
            this._vaultEnabled.Add(203, "bricklava");
            this._vaultEnabled.Add(204, "bricklava");
            this._vaultEnabled.Add(627, "bricklava");
            this._vaultEnabled.Add(628, "bricklava");
            this._vaultEnabled.Add(629, "bricklava");

            this._vaultEnabled.Add(59, "bricksummer2011");
            this._vaultEnabled.Add(228, "bricksummer2011");
            this._vaultEnabled.Add(229, "bricksummer2011");
            this._vaultEnabled.Add(230, "bricksummer2011");
            this._vaultEnabled.Add(231, "bricksummer2011");
            this._vaultEnabled.Add(232, "bricksummer2011");

            this._vaultEnabled.Add(233, "brickspring2011");
            this._vaultEnabled.Add(234, "brickspring2011");
            this._vaultEnabled.Add(235, "brickspring2011");
            this._vaultEnabled.Add(236, "brickspring2011");
            this._vaultEnabled.Add(237, "brickspring2011");
            this._vaultEnabled.Add(238, "brickspring2011");
            this._vaultEnabled.Add(239, "brickspring2011");
            this._vaultEnabled.Add(240, "brickspring2011");

            this._vaultEnabled.Add(244, "mixednewyear2010");
            this._vaultEnabled.Add(245, "mixednewyear2010");
            this._vaultEnabled.Add(246, "mixednewyear2010");
            this._vaultEnabled.Add(247, "mixednewyear2010");
            this._vaultEnabled.Add(248, "mixednewyear2010");

            this._vaultEnabled.Add(249, "brickchristmas2010");
            this._vaultEnabled.Add(250, "brickchristmas2010");
            this._vaultEnabled.Add(251, "brickchristmas2010");
            this._vaultEnabled.Add(252, "brickchristmas2010");
            this._vaultEnabled.Add(253, "brickchristmas2010");
            this._vaultEnabled.Add(254, "brickchristmas2010");

            this._vaultEnabled.Add(624, "brickxmas2012");
            this._vaultEnabled.Add(625, "brickxmas2012");
            this._vaultEnabled.Add(626, "brickxmas2012");
            this._vaultEnabled.Add(362, "brickxmas2012");
            this._vaultEnabled.Add(363, "brickxmas2012");
            this._vaultEnabled.Add(364, "brickxmas2012");
            this._vaultEnabled.Add(365, "brickxmas2012");
            this._vaultEnabled.Add(366, "brickxmas2012");
            this._vaultEnabled.Add(367, "brickxmas2012");

            this._vaultEnabled.Add(533, "brickbgcanvas");
            this._vaultEnabled.Add(534, "brickbgcanvas");
            this._vaultEnabled.Add(535, "brickbgcanvas");
            this._vaultEnabled.Add(536, "brickbgcanvas");
            this._vaultEnabled.Add(537, "brickbgcanvas");
            this._vaultEnabled.Add(538, "brickbgcanvas");
            this._vaultEnabled.Add(606, "brickbgcanvas");
            this._vaultEnabled.Add(671, "brickbgcanvas");
            this._vaultEnabled.Add(672, "brickbgcanvas");

            this._vaultEnabled.Add(369, "brickswamp");
            this._vaultEnabled.Add(370, "brickswamp");
            this._vaultEnabled.Add(371, "brickswamp");
            this._vaultEnabled.Add(372, "brickswamp");
            this._vaultEnabled.Add(373, "brickswamp");
            this._vaultEnabled.Add(557, "brickswamp");
            this._vaultEnabled.Add(630, "brickswamp");

            this._vaultEnabled.Add(206, "brickzombiedoor");
            this._vaultEnabled.Add(207, "brickzombiedoor");
            this._vaultEnabled.Add(374, "brickworldportal");

            this._vaultEnabled.Add((int) ItemTypes.PortalInvisible, "brickinvisibleportal");

            this._vaultEnabled.Add(382, "bricksparta");
            this._vaultEnabled.Add(383, "bricksparta");
            this._vaultEnabled.Add(384, "bricksparta");
            this._vaultEnabled.Add(208, "bricksparta");
            this._vaultEnabled.Add(209, "bricksparta");
            this._vaultEnabled.Add(210, "bricksparta");
            this._vaultEnabled.Add(211, "bricksparta");
            this._vaultEnabled.Add(638, "bricksparta");
            this._vaultEnabled.Add(639, "bricksparta");
            this._vaultEnabled.Add(640, "bricksparta");

            this._vaultEnabled.Add(386, "brickfarm");
            this._vaultEnabled.Add(387, "brickfarm");
            this._vaultEnabled.Add(388, "brickfarm");
            this._vaultEnabled.Add(389, "brickfarm");
            this._vaultEnabled.Add(212, "brickfarm");
            this._vaultEnabled.Add(1531, "brickfarm");

            this._vaultEnabled.Add((int) ItemTypes.TextSign, "bricksign");

            this._vaultEnabled.Add(390, "brickautumn2014");
            this._vaultEnabled.Add(391, "brickautumn2014");
            this._vaultEnabled.Add(392, "brickautumn2014");
            this._vaultEnabled.Add(393, "brickautumn2014");
            this._vaultEnabled.Add(394, "brickautumn2014");
            this._vaultEnabled.Add(395, "brickautumn2014");
            this._vaultEnabled.Add(396, "brickautumn2014");
            this._vaultEnabled.Add(641, "brickautumn2014");
            this._vaultEnabled.Add(642, "brickautumn2014");
            this._vaultEnabled.Add(643, "brickautumn2014");

            this._vaultEnabled.Add((int) ItemTypes.Hologram, "brickhologram");

            this._vaultEnabled.Add(215, "brickchristmas2014");
            this._vaultEnabled.Add(216, "brickchristmas2014");
            this._vaultEnabled.Add(398, "brickchristmas2014");
            this._vaultEnabled.Add(399, "brickchristmas2014");
            this._vaultEnabled.Add(400, "brickchristmas2014");
            this._vaultEnabled.Add(401, "brickchristmas2014");
            this._vaultEnabled.Add(402, "brickchristmas2014");
            this._vaultEnabled.Add(403, "brickchristmas2014");
            this._vaultEnabled.Add(404, "brickchristmas2014");

            this._vaultEnabled.Add(1001, "brickoneway");
            this._vaultEnabled.Add(1002, "brickoneway");
            this._vaultEnabled.Add(1003, "brickoneway");
            this._vaultEnabled.Add(1004, "brickoneway");
            this._vaultEnabled.Add(1052, "brickoneway");
            this._vaultEnabled.Add(1053, "brickoneway");
            this._vaultEnabled.Add(1054, "brickoneway");
            this._vaultEnabled.Add(1055, "brickoneway");
            this._vaultEnabled.Add(1056, "brickoneway");
            this._vaultEnabled.Add(1092, "brickoneway");

            this._vaultEnabled.Add(1011, "brickdeathdoor");
            this._vaultEnabled.Add(1012, "brickdeathdoor");

            this._vaultEnabled.Add(405, "brickvalentines2015");
            this._vaultEnabled.Add(406, "brickvalentines2015");
            this._vaultEnabled.Add(407, "brickvalentines2015");

            this._vaultEnabled.Add(1013, "brickmagic");
            this._vaultEnabled.Add(1014, "brickmagic2");
            this._vaultEnabled.Add(1015, "brickmagic3");
            this._vaultEnabled.Add(1016, "brickmagic4");
            this._vaultEnabled.Add(1017, "brickmagic5");
            this._vaultEnabled.Add(1132, "brickmagic6");

            this._vaultEnabled.Add(415, "bricklava");
            this._vaultEnabled.Add(416, "bricklava");

            this._vaultEnabled.Add(417, "brickeffectjump");
            this._vaultEnabled.Add(418, "brickeffectfly");
            this._vaultEnabled.Add(419, "brickeffectspeed");
            this._vaultEnabled.Add(453, "brickeffectlowgravity");
            this._vaultEnabled.Add(420, "brickeffectprotection");
            this._vaultEnabled.Add(421, "brickeffectcurse");
            this._vaultEnabled.Add(422, "brickeffectzombie");
            this._vaultEnabled.Add(461, "brickeffectmultijump");
            this._vaultEnabled.Add((int)ItemTypes.EffectGravity, "brickeffectgravity");

            this._vaultEnabled.Add(655, "brickcave");
            this._vaultEnabled.Add(656, "brickcave");
            this._vaultEnabled.Add(657, "brickcave");
            this._vaultEnabled.Add(658, "brickcave");
            this._vaultEnabled.Add(659, "brickcave");
            this._vaultEnabled.Add(660, "brickcave");
            this._vaultEnabled.Add(661, "brickcave");
            this._vaultEnabled.Add(662, "brickcave");

            this._vaultEnabled.Add(605, "brickneon");
            this._vaultEnabled.Add(673, "brickneon");
            this._vaultEnabled.Add(674, "brickneon");
            this._vaultEnabled.Add(675, "brickneon");
            this._vaultEnabled.Add(697, "brickneon");
            this._vaultEnabled.Add(698, "brickneon");

            this._vaultEnabled.Add(441, "bricksummer2015");
            this._vaultEnabled.Add(442, "bricksummer2015");
            this._vaultEnabled.Add(443, "bricksummer2015");
            this._vaultEnabled.Add(444, "bricksummer2015");
            this._vaultEnabled.Add(445, "bricksummer2015");

            this._vaultEnabled.Add(1035, "brickdomestic");
            this._vaultEnabled.Add(1036, "brickdomestic");
            this._vaultEnabled.Add(1037, "brickdomestic");
            this._vaultEnabled.Add(1038, "brickdomestic");
            this._vaultEnabled.Add(1039, "brickdomestic");
            this._vaultEnabled.Add(1040, "brickdomestic");
            this._vaultEnabled.Add(683, "brickdomestic");
            this._vaultEnabled.Add(684, "brickdomestic");
            this._vaultEnabled.Add(685, "brickdomestic");
            this._vaultEnabled.Add(686, "brickdomestic");
            this._vaultEnabled.Add(687, "brickdomestic");
            this._vaultEnabled.Add(446, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.DomesticLightBulb, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.DomesticTap, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.DomesticPainting, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.DomesticVase, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.DomesticTv, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.DomesticWindow, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.HalfBlockDomesticYellow, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.HalfBlockDomesticBrown, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.HalfBlockDomesticWhite, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.DomesticPipeStraight, "brickdomestic");
            this._vaultEnabled.Add((int) ItemTypes.DomesticPipeT, "brickdomestic");
            this._vaultEnabled.Add((int)ItemTypes.DomesticFrameBorder, "brickdomestic");
            this._vaultEnabled.Add(1539, "brickdomestic");

            this._vaultEnabled.Add(1047, "brickhalloween2015");
            this._vaultEnabled.Add(1048, "brickhalloween2015");
            this._vaultEnabled.Add(1049, "brickhalloween2015");
            this._vaultEnabled.Add(1050, "brickhalloween2015");
            this._vaultEnabled.Add(694, "brickhalloween2015");
            this._vaultEnabled.Add(695, "brickhalloween2015");
            this._vaultEnabled.Add(696, "brickhalloween2015");
            this._vaultEnabled.Add(454, "brickhalloween2015");
            this._vaultEnabled.Add(455, "brickhalloween2015");
            this._vaultEnabled.Add((int) ItemTypes.Halloween2015WindowRect, "brickhalloween2015");
            this._vaultEnabled.Add((int) ItemTypes.Halloween2015WindowCircle, "brickhalloween2015");
            this._vaultEnabled.Add((int) ItemTypes.Halloween2015Lamp, "brickhalloween2015");

            this._vaultEnabled.Add((int) ItemTypes.Ice, "brickice2");

            this._vaultEnabled.Add(462, "bricknewyear2015");
            this._vaultEnabled.Add(463, "bricknewyear2015");
            this._vaultEnabled.Add((int) ItemTypes.NewYear2015Balloon, "bricknewyear2015");
            this._vaultEnabled.Add((int) ItemTypes.NewYear2015Streamer, "bricknewyear2015");

            this._vaultEnabled.Add(1070, "brickfairytale");
            this._vaultEnabled.Add(1071, "brickfairytale");
            this._vaultEnabled.Add(1072, "brickfairytale");
            this._vaultEnabled.Add(1073, "brickfairytale");
            this._vaultEnabled.Add(1074, "brickfairytale");
            this._vaultEnabled.Add(468, "brickfairytale");
            this._vaultEnabled.Add(469, "brickfairytale");
            this._vaultEnabled.Add(470, "brickfairytale");
            this._vaultEnabled.Add(704, "brickfairytale");
            this._vaultEnabled.Add(705, "brickfairytale");
            this._vaultEnabled.Add(706, "brickfairytale");
            this._vaultEnabled.Add(707, "brickfairytale");
            this._vaultEnabled.Add((int) ItemTypes.FairytaleLadder, "brickfairytale");
            this._vaultEnabled.Add((int)ItemTypes.FairytaleFlowers, "brickfairytale");
            this._vaultEnabled.Add((int)ItemTypes.HalfBlockFairytaleOrange, "brickfairytale");
            this._vaultEnabled.Add((int)ItemTypes.HalfBlockFairytaleGreen, "brickfairytale");
            this._vaultEnabled.Add((int)ItemTypes.HalfBlockFairytaleBlue, "brickfairytale");
            this._vaultEnabled.Add((int)ItemTypes.HalfBlockFairytalePink, "brickfairytale");

            this._vaultEnabled.Add(1081, "brickspring2016");
            this._vaultEnabled.Add(1082, "brickspring2016");
            this._vaultEnabled.Add(473, "brickspring2016");
            this._vaultEnabled.Add(474, "brickspring2016");
            this._vaultEnabled.Add((int) ItemTypes.SpringDaisy, "brickspring2016");
            this._vaultEnabled.Add((int) ItemTypes.SpringTulip, "brickspring2016");
            this._vaultEnabled.Add((int) ItemTypes.SpringDaffodil, "brickspring2016");

            this._vaultEnabled.Add(478, "brickspringtrophybronze");
            this._vaultEnabled.Add(479, "brickspringtrophysilver");
            this._vaultEnabled.Add(480, "brickspringtrophygold");

            this._vaultEnabled.Add(484, "bricksummertrophybronze");
            this._vaultEnabled.Add(485, "bricksummertrophysilver");
            this._vaultEnabled.Add(486, "bricksummertrophygold");

            this._vaultEnabled.Add(1083, "bricksummer2016");
            this._vaultEnabled.Add(1084, "bricksummer2016");
            this._vaultEnabled.Add(1085, "bricksummer2016");
            this._vaultEnabled.Add(1086, "bricksummer2016");
            this._vaultEnabled.Add(1087, "bricksummer2016");
            this._vaultEnabled.Add(708, "bricksummer2016");
            this._vaultEnabled.Add(712, "bricksummer2016");
            this._vaultEnabled.Add(713, "bricksummer2016");
            this._vaultEnabled.Add(714, "bricksummer2016");
            this._vaultEnabled.Add((int) ItemTypes.SummerFlag, "bricksummer2016");
            this._vaultEnabled.Add((int) ItemTypes.SummerAwning, "bricksummer2016");
            this._vaultEnabled.Add((int) ItemTypes.SummerIceCream, "bricksummer2016");

            this._vaultEnabled.Add(721, "bricktextile");
            this._vaultEnabled.Add(722, "bricktextile");
            this._vaultEnabled.Add(723, "bricktextile");
            this._vaultEnabled.Add(724, "bricktextile");
            this._vaultEnabled.Add(725, "bricktextile");

            this._vaultEnabled.Add(487, "brickrestaurant");
            this._vaultEnabled.Add(488, "brickrestaurant");
            this._vaultEnabled.Add(489, "brickrestaurant");
            this._vaultEnabled.Add(490, "brickrestaurant");
            this._vaultEnabled.Add(491, "brickrestaurant");
            this._vaultEnabled.Add((int)ItemTypes.RestaurantCup, "brickrestaurant");
            this._vaultEnabled.Add((int)ItemTypes.RestaurantPlate, "brickrestaurant");
            this._vaultEnabled.Add((int)ItemTypes.RestaurantBowl, "brickrestaurant");

            this._vaultEnabled.Add(1093, "brickmine");
            this._vaultEnabled.Add(720, "brickmine");
            this._vaultEnabled.Add(495, "brickmine");
            this._vaultEnabled.Add(496, "brickmine");
            this._vaultEnabled.Add((int)ItemTypes.CaveCrystal, "brickmine");
            this._vaultEnabled.Add((int)ItemTypes.CaveTorch, "brickmine");

            this._vaultEnabled.Add((int)ItemTypes.Halloween2016Rotatable, "brickhalloween2016");
            this._vaultEnabled.Add((int)ItemTypes.Halloween2016Pumpkin, "brickhalloween2016");
            this._vaultEnabled.Add(1501, "brickhalloween2016");
            this._vaultEnabled.Add((int)ItemTypes.Halloween2016Eyes, "brickhalloween2016");
            this._vaultEnabled.Add(726, "brickhalloween2016");
            this._vaultEnabled.Add(727, "brickhalloween2016");

            this._vaultEnabled.Add((int)ItemTypes.CrownDoor, "brickcrowndoor");
            this._vaultEnabled.Add((int)ItemTypes.CrownGate, "brickcrowndoor");

            this._vaultEnabled.Add((int)ItemTypes.HalfBlockChristmas2016PresentRed, "brickchristmas2016");
            this._vaultEnabled.Add((int)ItemTypes.HalfBlockChristmas2016PresentGreen, "brickchristmas2016");
            this._vaultEnabled.Add((int)ItemTypes.HalfBlockChristmas2016PresentWhite, "brickchristmas2016");
            this._vaultEnabled.Add((int)ItemTypes.HalfBlockChristmas2016PresentBlue, "brickchristmas2016");
            this._vaultEnabled.Add((int)ItemTypes.HalfBlockChristmas2016PresentYellow, "brickchristmas2016");
            this._vaultEnabled.Add((int)ItemTypes.Christmas2016LightsDown, "brickchristmas2016");
            this._vaultEnabled.Add((int)ItemTypes.Christmas2016LightsUp, "brickchristmas2016");
            this._vaultEnabled.Add(1508, "brickchristmas2016");
            this._vaultEnabled.Add(1509, "brickchristmas2016");
            this._vaultEnabled.Add((int)ItemTypes.Christmas2016Candle, "brickchristmas2016");

            this._vaultEnabled.Add(1106, "bricktiles");
            this._vaultEnabled.Add(1107, "bricktiles");
            this._vaultEnabled.Add(1108, "bricktiles");
            this._vaultEnabled.Add(1109, "bricktiles");
            this._vaultEnabled.Add(1110, "bricktiles");
            this._vaultEnabled.Add(1111, "bricktiles");
            this._vaultEnabled.Add(1112, "bricktiles");
            this._vaultEnabled.Add(1113, "bricktiles");
            this._vaultEnabled.Add(1114, "bricktiles");
            this._vaultEnabled.Add(1115, "bricktiles");
            this._vaultEnabled.Add(733, "bricktiles");
            this._vaultEnabled.Add(734, "bricktiles");
            this._vaultEnabled.Add(735, "bricktiles");
            this._vaultEnabled.Add(736, "bricktiles");
            this._vaultEnabled.Add(737, "bricktiles");
            this._vaultEnabled.Add(738, "bricktiles");
            this._vaultEnabled.Add(739, "bricktiles");
            this._vaultEnabled.Add(740, "bricktiles");
            this._vaultEnabled.Add(741, "bricktiles");
            this._vaultEnabled.Add(742, "bricktiles");

            this._vaultEnabled.Add(1511, "brickstpatricks2017");
            this._vaultEnabled.Add(1512, "brickstpatricks2017");
            this._vaultEnabled.Add(1513, "brickstpatricks2017");
            this._vaultEnabled.Add(1514, "brickstpatricks2017");
            this._vaultEnabled.Add(1515, "brickstpatricks2017");

            this._vaultEnabled.Add((int)ItemTypes.GodBlock, "brickgodblock");

            this._vaultEnabled.Add(1116, "brickhalfblocks");
            this._vaultEnabled.Add(1117, "brickhalfblocks");
            this._vaultEnabled.Add(1118, "brickhalfblocks");
            this._vaultEnabled.Add(1119, "brickhalfblocks");
            this._vaultEnabled.Add(1120, "brickhalfblocks");
            this._vaultEnabled.Add(1121, "brickhalfblocks");
            this._vaultEnabled.Add(1122, "brickhalfblocks");
            this._vaultEnabled.Add(1123, "brickhalfblocks");
            this._vaultEnabled.Add(1124, "brickhalfblocks");
            this._vaultEnabled.Add(1125, "brickhalfblocks");

            this._vaultEnabled.Add(1540, "brickdesigntrophybronze");
            this._vaultEnabled.Add(1541, "brickdesigntrophysilver");
            this._vaultEnabled.Add(1542, "brickdesigntrophygold");
        }
    }
}