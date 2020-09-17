using System;
using System.Collections.Generic;
using System.Linq;

namespace EverybodyEdits.Game
{
    public class BlockMap
    {
        protected int[] beta = { 37, 38, 39, 40, 41, 42 };
        // Subset of all blocks, which are allowed in open worlds

        protected int[] buildersclub = { 200, 201 /*, (int)ItemTypes.TextSign*/};

        protected int[] freeForAll =
        {
            // Gravity
            0, 1, 2, 3, 4, 411, 412, 413, 414,
            // Crown
            5,
            // Keys
            6, 7, 8, 408, 409, 410,
            // Basic 
            9, 10, 11, 12, 13, 14, 15, 182,
            // Brick 
            16, 17, 18, 19, 20, 21,
            // Doors
            23, 24, 25, 1005, 1006, 1007,
            // Gates
            26, 27, 28, 1008, 1009, 1010,
            // Metal
            29, 30, 31,
            // Grass
            34, 35, 36,
            // Special
            22, 32, 33,
            // Coins
            100, 101,
            // Basic bg
            500, 501, 502, 503, 504, 505, 506,
            // Brick bg
            507, 508, 509, 510, 511, 512
        };

        protected int[] openworlds_subset =
        {
            // Gravity
            0, 1, 2, 3, 4, 411, 412, 413, 414,
            // Basic 
            9, 10, 11, 12, 13, 14, 15, 182,
            // Crown
            5,
            // Metal
            29, 30, 31
        };

        protected Dictionary<int, string> vaultEnabled = new Dictionary<int, string>();

        public BlockMap()
        {
            this.populateVaultEnabled();
        }

        public int getBlockLayerById(int id)
        {
            // In the client, a brick is placed in the background if id is between 500 and 700
            if (id >= 500 && id < 700) return 1;
            return 0;
        }

        private string getBlockVaultIdById(int id)
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

        public bool blockIsLegitOpenWorld(Player player, int blockid)
        {
            if (this.openworlds_subset.Contains(blockid))
            {
                return this.blockIsLegit(player, blockid);
            }
            return false;
        }

        public bool blockIsLegit(Player player, int blockid)
        {
            if (this.freeForAll.Contains(blockid))
            {
                return true;
            }
            if (this.beta.Contains(blockid)/*&& player.haveSmileyPackage*/)
            {
                return true;
            }
            if (this.buildersclub.Contains(blockid)/* && player.isClubMember*/)
            {
                return true;
            }
            var blockNameInVault = this.getBlockVaultIdById(blockid);

            //Console.WriteLine("Has " + blockNameInVault + "? " + player.hasBrickPack(blockNameInVault));
            if (player.hasBrickPack(blockNameInVault))
            {
                return true;
            }

            if (blockNameInVault != String.Empty) Console.WriteLine("Not available to user: " + blockid);

            return false;
        }

        private void populateVaultEnabled()
        {
            this.vaultEnabled.Add(60, "brickcandy");
            this.vaultEnabled.Add(61, "brickcandy");
            this.vaultEnabled.Add(62, "brickcandy");
            this.vaultEnabled.Add(63, "brickcandy");
            this.vaultEnabled.Add(64, "brickcandy");
            this.vaultEnabled.Add(65, "brickcandy");
            this.vaultEnabled.Add(66, "brickcandy");
            this.vaultEnabled.Add(67, "brickcandy");
            this.vaultEnabled.Add(227, "brickcandy");
            this.vaultEnabled.Add(539, "brickcandy");
            this.vaultEnabled.Add(540, "brickcandy");

            this.vaultEnabled.Add(70, "brickminiral");
            this.vaultEnabled.Add(71, "brickminiral");
            this.vaultEnabled.Add(72, "brickminiral");
            this.vaultEnabled.Add(73, "brickminiral");
            this.vaultEnabled.Add(74, "brickminiral");
            this.vaultEnabled.Add(75, "brickminiral");
            this.vaultEnabled.Add(76, "brickminiral");

            this.vaultEnabled.Add(68, "brickhw2011");
            this.vaultEnabled.Add(69, "brickhw2011");
            this.vaultEnabled.Add(224, "brickhw2011");
            this.vaultEnabled.Add(225, "brickhw2011");
            this.vaultEnabled.Add(226, "brickhw2011");
            this.vaultEnabled.Add(541, "brickhw2011");
            this.vaultEnabled.Add(542, "brickhw2011");
            this.vaultEnabled.Add(543, "brickhw2011");
            this.vaultEnabled.Add(544, "brickhw2011");

            this.vaultEnabled.Add(223, "brickhwtrophy");

            //XMAS 2011
            this.vaultEnabled.Add(78, "brickxmas2011");
            this.vaultEnabled.Add(79, "brickxmas2011");
            this.vaultEnabled.Add(80, "brickxmas2011");
            this.vaultEnabled.Add(81, "brickxmas2011");
            this.vaultEnabled.Add(82, "brickxmas2011");
            this.vaultEnabled.Add(218, "brickxmas2011");
            this.vaultEnabled.Add(219, "brickxmas2011");
            this.vaultEnabled.Add(220, "brickxmas2011");
            this.vaultEnabled.Add(221, "brickxmas2011");
            this.vaultEnabled.Add(222, "brickxmas2011");

            this.vaultEnabled.Add(84, "brickscifi");
            this.vaultEnabled.Add(85, "brickscifi");
            this.vaultEnabled.Add(86, "brickscifi");
            this.vaultEnabled.Add(87, "brickscifi");
            this.vaultEnabled.Add(88, "brickscifi");
            this.vaultEnabled.Add(89, "brickscifi");
            this.vaultEnabled.Add(90, "brickscifi");
            this.vaultEnabled.Add(91, "brickscifi");

            this.vaultEnabled.Add(545, "brickbgcarnival");
            this.vaultEnabled.Add(546, "brickbgcarnival");
            this.vaultEnabled.Add(547, "brickbgcarnival");
            this.vaultEnabled.Add(548, "brickbgcarnival");
            this.vaultEnabled.Add(549, "brickbgcarnival");

            this.vaultEnabled.Add(256, "brickeaster2012");
            this.vaultEnabled.Add(257, "brickeaster2012");
            this.vaultEnabled.Add(258, "brickeaster2012");
            this.vaultEnabled.Add(259, "brickeaster2012");
            this.vaultEnabled.Add(260, "brickeaster2012");

            this.vaultEnabled.Add(261, "brickprison");
            this.vaultEnabled.Add(92, "brickprison");
            this.vaultEnabled.Add(550, "brickprison");
            this.vaultEnabled.Add(551, "brickprison");
            this.vaultEnabled.Add(552, "brickprison");
            this.vaultEnabled.Add(553, "brickprison");

            this.vaultEnabled.Add(262, "brickwindow");
            this.vaultEnabled.Add(263, "brickwindow");
            this.vaultEnabled.Add(264, "brickwindow");
            this.vaultEnabled.Add(265, "brickwindow");
            this.vaultEnabled.Add(266, "brickwindow");
            this.vaultEnabled.Add(267, "brickwindow");
            this.vaultEnabled.Add(268, "brickwindow");
            this.vaultEnabled.Add(269, "brickwindow");
            this.vaultEnabled.Add(270, "brickwindow");

            this.vaultEnabled.Add(93, "brickpirate");
            this.vaultEnabled.Add(94, "brickpirate");
            this.vaultEnabled.Add(271, "brickpirate");
            this.vaultEnabled.Add(272, "brickpirate");
            this.vaultEnabled.Add(554, "brickpirate");
            this.vaultEnabled.Add(555, "brickpirate");
            this.vaultEnabled.Add(556, "brickpirate");
            this.vaultEnabled.Add(557, "brickpirate");
            this.vaultEnabled.Add(558, "brickpirate");
            this.vaultEnabled.Add(559, "brickpirate");
            this.vaultEnabled.Add(560, "brickpirate");

            this.vaultEnabled.Add(95, "brickviking");
            this.vaultEnabled.Add(273, "brickviking");
            this.vaultEnabled.Add(274, "brickviking");
            this.vaultEnabled.Add(275, "brickviking");
            this.vaultEnabled.Add(561, "brickviking");
            this.vaultEnabled.Add(562, "brickviking");
            this.vaultEnabled.Add(563, "brickviking");

            this.vaultEnabled.Add(96, "brickninja");
            this.vaultEnabled.Add(97, "brickninja");
            this.vaultEnabled.Add(120, "brickninja");
            this.vaultEnabled.Add(276, "brickninja");
            this.vaultEnabled.Add(277, "brickninja");
            this.vaultEnabled.Add(278, "brickninja");
            this.vaultEnabled.Add(279, "brickninja");
            this.vaultEnabled.Add(280, "brickninja");
            this.vaultEnabled.Add(281, "brickninja");
            this.vaultEnabled.Add(282, "brickninja");
            this.vaultEnabled.Add(283, "brickninja");
            this.vaultEnabled.Add(284, "brickninja");
            this.vaultEnabled.Add(564, "brickninja");
            this.vaultEnabled.Add(565, "brickninja");
            this.vaultEnabled.Add(566, "brickninja");
            this.vaultEnabled.Add(567, "brickninja");

            this.vaultEnabled.Add(122, "brickcowboy");
            this.vaultEnabled.Add(123, "brickcowboy");
            this.vaultEnabled.Add(124, "brickcowboy");
            this.vaultEnabled.Add(125, "brickcowboy");
            this.vaultEnabled.Add(126, "brickcowboy");
            this.vaultEnabled.Add(127, "brickcowboy");
            this.vaultEnabled.Add(568, "brickcowboy");
            this.vaultEnabled.Add(569, "brickcowboy");
            this.vaultEnabled.Add(570, "brickcowboy");
            this.vaultEnabled.Add(571, "brickcowboy");
            this.vaultEnabled.Add(572, "brickcowboy");
            this.vaultEnabled.Add(573, "brickcowboy");
            this.vaultEnabled.Add(285, "brickcowboy");
            this.vaultEnabled.Add(286, "brickcowboy");
            this.vaultEnabled.Add(287, "brickcowboy");
            this.vaultEnabled.Add(288, "brickcowboy");
            this.vaultEnabled.Add(289, "brickcowboy");
            this.vaultEnabled.Add(290, "brickcowboy");
            this.vaultEnabled.Add(291, "brickcowboy");
            this.vaultEnabled.Add(292, "brickcowboy");
            this.vaultEnabled.Add(293, "brickcowboy");
            this.vaultEnabled.Add(294, "brickcowboy");
            this.vaultEnabled.Add(295, "brickcowboy");
            this.vaultEnabled.Add(296, "brickcowboy");
            this.vaultEnabled.Add(297, "brickcowboy");
            this.vaultEnabled.Add(298, "brickcowboy");
            this.vaultEnabled.Add(299, "brickcowboy");

            this.vaultEnabled.Add(128, "brickplastic");
            this.vaultEnabled.Add(129, "brickplastic");
            this.vaultEnabled.Add(130, "brickplastic");
            this.vaultEnabled.Add(131, "brickplastic");
            this.vaultEnabled.Add(132, "brickplastic");
            this.vaultEnabled.Add(133, "brickplastic");
            this.vaultEnabled.Add(134, "brickplastic");
            this.vaultEnabled.Add(135, "brickplastic");
            this.vaultEnabled.Add(136, "brickplastic");

            this.vaultEnabled.Add(119, "brickwater");
            this.vaultEnabled.Add(300, "brickwater");
            this.vaultEnabled.Add(574, "brickwater");
            this.vaultEnabled.Add(575, "brickwater");
            this.vaultEnabled.Add(576, "brickwater");
            this.vaultEnabled.Add(577, "brickwater");
            this.vaultEnabled.Add(578, "brickwater");

            this.vaultEnabled.Add(137, "bricksand");
            this.vaultEnabled.Add(138, "bricksand");
            this.vaultEnabled.Add(139, "bricksand");
            this.vaultEnabled.Add(140, "bricksand");
            this.vaultEnabled.Add(141, "bricksand");
            this.vaultEnabled.Add(142, "bricksand");
            this.vaultEnabled.Add(579, "bricksand");
            this.vaultEnabled.Add(580, "bricksand");
            this.vaultEnabled.Add(581, "bricksand");
            this.vaultEnabled.Add(582, "bricksand");
            this.vaultEnabled.Add(583, "bricksand");
            this.vaultEnabled.Add(584, "bricksand");
            this.vaultEnabled.Add(301, "bricksand");
            this.vaultEnabled.Add(302, "bricksand");
            this.vaultEnabled.Add(303, "bricksand");
            this.vaultEnabled.Add(304, "bricksand");
            this.vaultEnabled.Add(305, "bricksand");
            this.vaultEnabled.Add(306, "bricksand");

            this.vaultEnabled.Add(307, "bricksummer2012");
            this.vaultEnabled.Add(308, "bricksummer2012");
            this.vaultEnabled.Add(309, "bricksummer2012");
            this.vaultEnabled.Add(310, "bricksummer2012");

            this.vaultEnabled.Add(143, "brickcloud");
            this.vaultEnabled.Add(311, "brickcloud");
            this.vaultEnabled.Add(312, "brickcloud");
            this.vaultEnabled.Add(313, "brickcloud");
            this.vaultEnabled.Add(314, "brickcloud");
            this.vaultEnabled.Add(315, "brickcloud");
            this.vaultEnabled.Add(316, "brickcloud");
            this.vaultEnabled.Add(317, "brickcloud");
            this.vaultEnabled.Add(318, "brickcloud");

            this.vaultEnabled.Add(144, "brickplateiron");
            this.vaultEnabled.Add(145, "brickplateiron");
            this.vaultEnabled.Add(585, "brickplateiron");
            this.vaultEnabled.Add(586, "brickplateiron");
            this.vaultEnabled.Add(587, "brickplateiron");
            this.vaultEnabled.Add(588, "brickplateiron");
            this.vaultEnabled.Add(589, "brickplateiron");

            this.vaultEnabled.Add(319, "bricksigns");
            this.vaultEnabled.Add(320, "bricksigns");
            this.vaultEnabled.Add(321, "bricksigns");
            this.vaultEnabled.Add(322, "bricksigns");
            this.vaultEnabled.Add(323, "bricksigns");
            this.vaultEnabled.Add(324, "bricksigns");

            this.vaultEnabled.Add(146, "brickindustrial");
            this.vaultEnabled.Add(147, "brickindustrial");
            this.vaultEnabled.Add(148, "brickindustrial");
            this.vaultEnabled.Add(149, "brickindustrial");
            this.vaultEnabled.Add(150, "brickindustrial");
            this.vaultEnabled.Add(151, "brickindustrial");
            this.vaultEnabled.Add(152, "brickindustrial");
            this.vaultEnabled.Add(153, "brickindustrial");

            this.vaultEnabled.Add(154, "bricktimbered");
            this.vaultEnabled.Add(590, "bricktimbered");
            this.vaultEnabled.Add(591, "bricktimbered");
            this.vaultEnabled.Add(592, "bricktimbered");
            this.vaultEnabled.Add(593, "bricktimbered");
            this.vaultEnabled.Add(594, "bricktimbered");
            this.vaultEnabled.Add(595, "bricktimbered");
            this.vaultEnabled.Add(596, "bricktimbered");
            this.vaultEnabled.Add(597, "bricktimbered");
            this.vaultEnabled.Add(598, "bricktimbered");

            this.vaultEnabled.Add(118, "brickcastle");
            this.vaultEnabled.Add(158, "brickcastle");
            this.vaultEnabled.Add(159, "brickcastle");
            this.vaultEnabled.Add(160, "brickcastle");
            this.vaultEnabled.Add(161, "brickcastle");
            this.vaultEnabled.Add(599, "brickcastle");
            this.vaultEnabled.Add(325, "brickcastle");
            this.vaultEnabled.Add(326, "brickcastle");

            this.vaultEnabled.Add(162, "brickmedieval");
            this.vaultEnabled.Add(163, "brickmedieval");
            this.vaultEnabled.Add(327, "brickmedieval");
            this.vaultEnabled.Add(328, "brickmedieval");
            this.vaultEnabled.Add(329, "brickmedieval");
            this.vaultEnabled.Add(330, "brickmedieval");
            this.vaultEnabled.Add(331, "brickmedieval");
            this.vaultEnabled.Add(600, "brickmedieval");

            this.vaultEnabled.Add(166, "brickpipe");
            this.vaultEnabled.Add(167, "brickpipe");
            this.vaultEnabled.Add(168, "brickpipe");
            this.vaultEnabled.Add(169, "brickpipe");
            this.vaultEnabled.Add(170, "brickpipe");
            this.vaultEnabled.Add(171, "brickpipe");

            this.vaultEnabled.Add(172, "brickrocket");
            this.vaultEnabled.Add(173, "brickrocket");
            this.vaultEnabled.Add(174, "brickrocket");
            this.vaultEnabled.Add(175, "brickrocket");
            this.vaultEnabled.Add(601, "brickrocket");
            this.vaultEnabled.Add(602, "brickrocket");
            this.vaultEnabled.Add(603, "brickrocket");
            this.vaultEnabled.Add(604, "brickrocket");
            this.vaultEnabled.Add(332, "brickrocket");
            this.vaultEnabled.Add(333, "brickrocket");
            this.vaultEnabled.Add(334, "brickrocket");
            this.vaultEnabled.Add(335, "brickrocket");

            this.vaultEnabled.Add(176, "brickmars");
            this.vaultEnabled.Add(177, "brickmars");
            this.vaultEnabled.Add(178, "brickmars");
            this.vaultEnabled.Add(179, "brickmars");
            this.vaultEnabled.Add(180, "brickmars");
            this.vaultEnabled.Add(181, "brickmars");
            this.vaultEnabled.Add(605, "brickmars");
            this.vaultEnabled.Add(606, "brickmars");
            this.vaultEnabled.Add(607, "brickmars");
            this.vaultEnabled.Add(336, "brickmars");

            this.vaultEnabled.Add(114, "brickboost");
            this.vaultEnabled.Add(115, "brickboost");
            this.vaultEnabled.Add(116, "brickboost");
            this.vaultEnabled.Add(117, "brickboost");

            this.vaultEnabled.Add(338, "brickmonster");
            this.vaultEnabled.Add(339, "brickmonster");
            this.vaultEnabled.Add(340, "brickmonster");
            this.vaultEnabled.Add(341, "brickmonster");
            this.vaultEnabled.Add(342, "brickmonster");
            this.vaultEnabled.Add(608, "brickmonster");
            this.vaultEnabled.Add(609, "brickmonster");

            this.vaultEnabled.Add(343, "brickfog");
            this.vaultEnabled.Add(344, "brickfog");
            this.vaultEnabled.Add(345, "brickfog");
            this.vaultEnabled.Add(346, "brickfog");
            this.vaultEnabled.Add(347, "brickfog");
            this.vaultEnabled.Add(348, "brickfog");
            this.vaultEnabled.Add(349, "brickfog");
            this.vaultEnabled.Add(350, "brickfog");
            this.vaultEnabled.Add(351, "brickfog");

            this.vaultEnabled.Add(352, "brickhw2012");
            this.vaultEnabled.Add(353, "brickhw2012");
            this.vaultEnabled.Add(354, "brickhw2012");
            this.vaultEnabled.Add(355, "brickhw2012");
            this.vaultEnabled.Add(356, "brickhw2012");

            this.vaultEnabled.Add(610, "brickbgnormal");
            this.vaultEnabled.Add(611, "brickbgnormal");
            this.vaultEnabled.Add(612, "brickbgnormal");
            this.vaultEnabled.Add(613, "brickbgnormal");
            this.vaultEnabled.Add(614, "brickbgnormal");
            this.vaultEnabled.Add(615, "brickbgnormal");
            this.vaultEnabled.Add(616, "brickbgnormal");

            this.vaultEnabled.Add(186, "brickchecker");
            this.vaultEnabled.Add(187, "brickchecker");
            this.vaultEnabled.Add(188, "brickchecker");
            this.vaultEnabled.Add(189, "brickchecker");
            this.vaultEnabled.Add(190, "brickchecker");
            this.vaultEnabled.Add(191, "brickchecker");
            this.vaultEnabled.Add(192, "brickchecker");

            this.vaultEnabled.Add(193, "brickjungleruins");
            this.vaultEnabled.Add(194, "brickjungleruins");
            this.vaultEnabled.Add(195, "brickjungleruins");
            this.vaultEnabled.Add(196, "brickjungleruins");
            this.vaultEnabled.Add(197, "brickjungleruins");
            this.vaultEnabled.Add(198, "brickjungleruins");
            this.vaultEnabled.Add(617, "brickjungleruins");
            this.vaultEnabled.Add(618, "brickjungleruins");
            this.vaultEnabled.Add(619, "brickjungleruins");
            this.vaultEnabled.Add(620, "brickjungleruins");

            this.vaultEnabled.Add(98, "brickjungle");
            this.vaultEnabled.Add(99, "brickjungle");
            this.vaultEnabled.Add(199, "brickjungle");
            this.vaultEnabled.Add(621, "brickjungle");
            this.vaultEnabled.Add(622, "brickjungle");
            this.vaultEnabled.Add(623, "brickjungle");
            this.vaultEnabled.Add(357, "brickjungle");
            this.vaultEnabled.Add(358, "brickjungle");
            this.vaultEnabled.Add(359, "brickjungle");

            this.vaultEnabled.Add(202, "bricklava");
            this.vaultEnabled.Add(203, "bricklava");
            this.vaultEnabled.Add(204, "bricklava");
            this.vaultEnabled.Add(627, "bricklava");
            this.vaultEnabled.Add(628, "bricklava");
            this.vaultEnabled.Add(629, "bricklava");

            this.vaultEnabled.Add(59, "bricksummer2011");
            this.vaultEnabled.Add(228, "bricksummer2011");
            this.vaultEnabled.Add(229, "bricksummer2011");
            this.vaultEnabled.Add(230, "bricksummer2011");
            this.vaultEnabled.Add(231, "bricksummer2011");
            this.vaultEnabled.Add(232, "bricksummer2011");

            this.vaultEnabled.Add(233, "brickspring2011");
            this.vaultEnabled.Add(234, "brickspring2011");
            this.vaultEnabled.Add(235, "brickspring2011");
            this.vaultEnabled.Add(236, "brickspring2011");
            this.vaultEnabled.Add(237, "brickspring2011");
            this.vaultEnabled.Add(238, "brickspring2011");
            this.vaultEnabled.Add(239, "brickspring2011");
            this.vaultEnabled.Add(240, "brickspring2011");

            this.vaultEnabled.Add(243, "bricksecret");

            this.vaultEnabled.Add(244, "mixednewyear2010");
            this.vaultEnabled.Add(245, "mixednewyear2010");
            this.vaultEnabled.Add(246, "mixednewyear2010");
            this.vaultEnabled.Add(247, "mixednewyear2010");
            this.vaultEnabled.Add(248, "mixednewyear2010");

            this.vaultEnabled.Add(249, "brickchristmas2010");
            this.vaultEnabled.Add(250, "brickchristmas2010");
            this.vaultEnabled.Add(251, "brickchristmas2010");
            this.vaultEnabled.Add(252, "brickchristmas2010");
            this.vaultEnabled.Add(253, "brickchristmas2010");
            this.vaultEnabled.Add(254, "brickchristmas2010");

            this.vaultEnabled.Add(624, "brickxmas2012");
            this.vaultEnabled.Add(625, "brickxmas2012");
            this.vaultEnabled.Add(626, "brickxmas2012");
            this.vaultEnabled.Add(362, "brickxmas2012");
            this.vaultEnabled.Add(363, "brickxmas2012");
            this.vaultEnabled.Add(364, "brickxmas2012");
            this.vaultEnabled.Add(365, "brickxmas2012");
            this.vaultEnabled.Add(366, "brickxmas2012");
            this.vaultEnabled.Add(367, "brickxmas2012");

            this.vaultEnabled.Add(44, "brickblackblock");

            this.vaultEnabled.Add(45, "brickfactorypack");
            this.vaultEnabled.Add(46, "brickfactorypack");
            this.vaultEnabled.Add(47, "brickfactorypack");
            this.vaultEnabled.Add(48, "brickfactorypack");
            this.vaultEnabled.Add(49, "brickfactorypack");

            this.vaultEnabled.Add(50, "bricksecret");

            this.vaultEnabled.Add(51, "brickglass");
            this.vaultEnabled.Add(52, "brickglass");
            this.vaultEnabled.Add(53, "brickglass");
            this.vaultEnabled.Add(54, "brickglass");
            this.vaultEnabled.Add(55, "brickglass");
            this.vaultEnabled.Add(56, "brickglass");
            this.vaultEnabled.Add(57, "brickglass");
            this.vaultEnabled.Add(58, "brickglass");

            this.vaultEnabled.Add(513, "brickbgchecker");
            this.vaultEnabled.Add(514, "brickbgchecker");
            this.vaultEnabled.Add(515, "brickbgchecker");
            this.vaultEnabled.Add(516, "brickbgchecker");
            this.vaultEnabled.Add(517, "brickbgchecker");
            this.vaultEnabled.Add(518, "brickbgchecker");
            this.vaultEnabled.Add(519, "brickbgchecker");

            this.vaultEnabled.Add(520, "brickbgdark");
            this.vaultEnabled.Add(521, "brickbgdark");
            this.vaultEnabled.Add(522, "brickbgdark");
            this.vaultEnabled.Add(523, "brickbgdark");
            this.vaultEnabled.Add(524, "brickbgdark");
            this.vaultEnabled.Add(525, "brickbgdark");
            this.vaultEnabled.Add(526, "brickbgdark");

            this.vaultEnabled.Add(527, "brickbgpastel");
            this.vaultEnabled.Add(528, "brickbgpastel");
            this.vaultEnabled.Add(529, "brickbgpastel");
            this.vaultEnabled.Add(530, "brickbgpastel");
            this.vaultEnabled.Add(531, "brickbgpastel");
            this.vaultEnabled.Add(532, "brickbgpastel");

            this.vaultEnabled.Add(533, "brickbgcanvas");
            this.vaultEnabled.Add(534, "brickbgcanvas");
            this.vaultEnabled.Add(535, "brickbgcanvas");
            this.vaultEnabled.Add(536, "brickbgcanvas");
            this.vaultEnabled.Add(537, "brickbgcanvas");
            this.vaultEnabled.Add(538, "brickbgcanvas");

            this.vaultEnabled.Add(369, "brickswamp");
            this.vaultEnabled.Add(370, "brickswamp");
            this.vaultEnabled.Add(630, "brickswamp");
            this.vaultEnabled.Add(371, "brickswamp");
            this.vaultEnabled.Add(372, "brickswamp");
            this.vaultEnabled.Add(373, "brickswamp");

            this.vaultEnabled.Add(206, "brickzombiedoor");
            this.vaultEnabled.Add(207, "brickzombiedoor");
            this.vaultEnabled.Add(374, "brickworldportal");

            this.vaultEnabled.Add(375, "brickscifi2013");
            this.vaultEnabled.Add(376, "brickscifi2013");
            this.vaultEnabled.Add(377, "brickscifi2013");
            this.vaultEnabled.Add(378, "brickscifi2013");
            this.vaultEnabled.Add(379, "brickscifi2013");
            this.vaultEnabled.Add(380, "brickscifi2013");
            this.vaultEnabled.Add(637, "brickscifi2013");

            this.vaultEnabled.Add((int)ItemTypes.PortalInvisible, "brickinvisibleportal");

            this.vaultEnabled.Add(382, "bricksparta");
            this.vaultEnabled.Add(383, "bricksparta");
            this.vaultEnabled.Add(384, "bricksparta");
            this.vaultEnabled.Add(208, "bricksparta");
            this.vaultEnabled.Add(209, "bricksparta");
            this.vaultEnabled.Add(210, "bricksparta");
            this.vaultEnabled.Add(211, "bricksparta");
            this.vaultEnabled.Add(638, "bricksparta");
            this.vaultEnabled.Add(639, "bricksparta");
            this.vaultEnabled.Add(640, "bricksparta");

            // 386, 387, 388, 389, 212
            this.vaultEnabled.Add(386, "brickfarm");
            this.vaultEnabled.Add(387, "brickfarm");
            this.vaultEnabled.Add(388, "brickfarm");
            this.vaultEnabled.Add(389, "brickfarm");
            this.vaultEnabled.Add(212, "brickfarm");

            // Text signs are now in vault
            this.vaultEnabled.Add((int)ItemTypes.TextSign, "bricksign");

            this.vaultEnabled.Add(390, "brickautumn2014");
            this.vaultEnabled.Add(391, "brickautumn2014");
            this.vaultEnabled.Add(392, "brickautumn2014");
            this.vaultEnabled.Add(393, "brickautumn2014");
            this.vaultEnabled.Add(394, "brickautumn2014");
            this.vaultEnabled.Add(395, "brickautumn2014");
            this.vaultEnabled.Add(396, "brickautumn2014");
            this.vaultEnabled.Add(641, "brickautumn2014");
            this.vaultEnabled.Add(642, "brickautumn2014");
            this.vaultEnabled.Add(643, "brickautumn2014");

            this.vaultEnabled.Add((int)ItemTypes.Hologram, "brickhologram");

            this.vaultEnabled.Add(215, "brickchristmas2014");
            this.vaultEnabled.Add(216, "brickchristmas2014");
            this.vaultEnabled.Add(398, "brickchristmas2014");
            this.vaultEnabled.Add(399, "brickchristmas2014");
            this.vaultEnabled.Add(400, "brickchristmas2014");
            this.vaultEnabled.Add(401, "brickchristmas2014");
            this.vaultEnabled.Add(402, "brickchristmas2014");
            this.vaultEnabled.Add(403, "brickchristmas2014");
            this.vaultEnabled.Add(404, "brickchristmas2014");

            this.vaultEnabled.Add(1001, "brickoneway");
            this.vaultEnabled.Add(1002, "brickoneway");
            this.vaultEnabled.Add(1003, "brickoneway");
            this.vaultEnabled.Add(1004, "brickoneway");

            this.vaultEnabled.Add(1011, "brickdeathdoor");
            this.vaultEnabled.Add(1012, "brickdeathdoor");

            this.vaultEnabled.Add(405, "mixedvalentines2015");
            this.vaultEnabled.Add(406, "mixedvalentines2015");
            this.vaultEnabled.Add(407, "mixedvalentines2015");
        }
    }
}