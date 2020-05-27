using System.Collections.Generic;
using System.Linq;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby.Invitations
{
    internal class InvitationBlocking
    {
        private const string BlockingTable = "InvitationBlocking";

        public static void IsUserBlocked(BigDB bigDb, string blockingUser, string name, Callback<bool> callback)
        {
            IsBlocked(bigDb, blockingUser, name, "BlockAll", "Block", callback);
        }

        public static void IsCrewBlocked(BigDB bigDb, string blockingUser, string crewId, Callback<bool> callback)
        {
            IsBlocked(bigDb, blockingUser, crewId, "BlockAllCrews", "BlockCrew", callback);
        }

        private static void IsBlocked(BigDB bigDb, string blockingUser, string blockedId, string blockAllProp,
            string blockProp, Callback<bool> callback)
        {
            bigDb.Load(BlockingTable, blockingUser, blockingTable =>
            {
                if (blockingTable == null)
                {
                    callback(false);
                    return;
                }

                if (blockingTable.GetBool(blockAllProp, false))
                {
                    callback(true);
                    return;
                }

                var blockingObject = blockingTable.GetObject(blockProp);
                if (blockingObject == null)
                {
                    callback(false);
                    return;
                }

                callback(blockingObject.GetBool(blockedId, false));
            });
        }

        public static void IsBlockingAllUsers(BigDB bigDb, string userId, Callback<bool> callback)
        {
            IsBlockingAll(bigDb, userId, "BlockAll", callback);
        }

        public static void IsBlockingAllCrews(BigDB bigDb, string userId, Callback<bool> callback)
        {
            IsBlockingAll(bigDb, userId, "BlockAllCrews", callback);
        }

        private static void IsBlockingAll(BigDB bigDb, string userId, string blockAllProp, Callback<bool> callback)
        {
            bigDb.Load(BlockingTable, userId,
                blockingTable => { callback(blockingTable != null && blockingTable.GetBool(blockAllProp, false)); });
        }

        public static void BlockUser(BigDB bigDb, string userId, string username, bool blockUser, Callback callback)
        {
            Block(bigDb, userId, username, blockUser, "Block", callback);
        }

        public static void BlockCrew(BigDB bigDb, string userId, string crewId, bool blockCrew, Callback callback)
        {
            Block(bigDb, userId, crewId, blockCrew, "BlockCrew", callback);
        }

        private static void Block(BigDB bigDb, string userId, string blockId, bool block, string blockProp,
            Callback callback)
        {
            bigDb.LoadOrCreate(BlockingTable, userId, blockingTable =>
            {
                var blockingObject = blockingTable.GetObject(blockProp);

                if (blockingObject == null)
                {
                    blockingObject = new DatabaseObject();
                    blockingTable.Set(blockProp, blockingObject);
                }

                if (block)
                {
                    blockingObject.Set(blockId, true);
                }
                else
                {
                    blockingObject.Remove(blockId);
                }

                blockingTable.Save();
                callback();
            });
        }

        public static void BlockAllFriends(BigDB bigDb, string userId, bool block)
        {
            BlockAll(bigDb, userId, block, "BlockAll");
        }

        public static void BlockAllCrews(BigDB bigDb, string userId, bool block)
        {
            BlockAll(bigDb, userId, block, "BlockAllCrews");
        }

        public static void BlockAll(BigDB bigDb, string userId, bool block, string blockProp)
        {
            bigDb.LoadOrCreate(BlockingTable, userId, blockingTable =>
            {
                blockingTable.Set(blockProp, block);
                blockingTable.Save();
            });
        }

        public static void GetBlockedUsers(BigDB bigDb, string userId, Callback<List<string>> callback)
        {
            GetBlocked(bigDb, userId, "Block", callback);
        }

        public static void GetBlockedCrews(BigDB bigDb, string userId, Callback<List<string>> callback)
        {
            GetBlocked(bigDb, userId, "BlockCrew", callback);
        }

        private static void GetBlocked(BigDB bigDb, string userId, string blockProp, Callback<List<string>> callback)
        {
            bigDb.Load(BlockingTable, userId, blockingTable =>
            {
                var rtn = new List<string>();
                if (blockingTable == null)
                {
                    callback(rtn);
                    return;
                }

                var blockingObject = blockingTable.GetObject(blockProp);
                if (blockingObject == null || blockingObject.Count == 0)
                {
                    callback(rtn);
                    return;
                }

                rtn.AddRange(from blockedUser in blockingObject where (bool)blockedUser.Value select blockedUser.Key);

                callback(rtn);
            });
        }
    }
}