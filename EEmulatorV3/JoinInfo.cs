using System;
using System.IO;
using System.Security.Cryptography;

namespace EEmulatorV3
{
    public class JoinInfo
    {
        public uint GameId { get; set; }
        public string GameConnectId { get; set; }
        public string GameCodeId { get; set; }
        public string RoomType { get; set; }
        public string ExtendedRoomId { get; set; }
        public string RoomId { get; set; }
        public byte[] RoomData { get; set; }
        public uint RoomFlags { get; set; }
        public string ConnectUserId { get; set; }
        public string PlayerIOToken { get; set; }
        public bool Visible { get; set; }
        public bool AllowWebRequests => (this.RoomFlags & 1u) == 1u;
        public string PartnerId { get; set; }
        public uint UserId { get; set; }
        public int GameCodeVersion { get; set; }

        public static JoinInfo Decode(string joinKey, string myServerId, byte[] encryptionKey, bool isDevelopmentServer, bool ignoreExpiredTime, out string errorMessage)
        {
            var bytes = Convert.FromBase64String(joinKey);
            byte[] decrypted;
            try { decrypted = Decrypt(bytes, encryptionKey); }
            catch (Exception e)
            {
                errorMessage = "Unable to decrypt the join key -- most likely it was encrypted for another server. If you're on the development server, you've probably logged into the wrong account in the development server. Actual Error Message: " + e.Message;
                return null;
            }

            JoinInfo result;
            try
            {
                using (var mem = new MemoryStream(decrypted))
                {
                    using (var reader = new BinaryReader(mem))
                    {
                        var version = reader.ReadInt32();
                        var generated = new DateTime(reader.ReadInt64());
                        var serverId = reader.ReadString();
                        if (!ignoreExpiredTime && DateTime.UtcNow.AddMinutes(-5.0) > generated)
                        {
                            errorMessage = "Got join key that was expired (" + (DateTime.UtcNow - generated).TotalMinutes + " minutes old). Disconnecting user.";
                            result = null;
                        }
                        else if (serverId != myServerId && !isDevelopmentServer)
                        {
                            errorMessage = string.Concat(new string[] {
                                "Got join key that was meant for another server (got:",
                                serverId,
                                ", me: ",
                                myServerId,
                                "). Disconnecting user."
                            });

                            result = null;
                        }
                        else
                        {
                            var joinInfo = new JoinInfo
                            {
                                GameConnectId = reader.ReadString(),
                                GameCodeId = reader.ReadString(),
                                RoomType = reader.ReadString(),
                                RoomId = reader.ReadString(),
                                RoomData = reader.ReadBytes(reader.ReadInt32()),
                                ExtendedRoomId = reader.ReadString(),
                                ConnectUserId = reader.ReadString(),
                                PlayerIOToken = reader.ReadString(),
                                Visible = reader.ReadBoolean(),
                                RoomFlags = reader.ReadUInt32(),
                                PartnerId = ((version >= 2) ? reader.ReadString() : null),
                                UserId = ((version >= 3) ? reader.ReadUInt32() : 0u),
                                GameCodeVersion = ((version >= 4) ? reader.ReadInt32() : 0)
                            };

                            try { joinInfo.GameId = uint.Parse(joinInfo.ExtendedRoomId.Split('/')[0]); }
                            catch { }

                            errorMessage = null;
                            result = joinInfo;
                        }
                    }
                }
            }
            catch (Exception e2)
            {
                errorMessage = "Unable to parse decrypted join key: " + joinKey + ", Actual Error Message: " + e2.Message;
                result = null;
            }

            return result;
        }

        public string Encode(string serverId, string serverType, byte[] encryptionKey)
        {
            string result;
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(memoryStream))
                {
                    binaryWriter.Write(4);
                    binaryWriter.Write(DateTime.UtcNow.AddYears(10).Ticks);
                    binaryWriter.Write(serverId);
                    binaryWriter.Write(this.GameConnectId);
                    binaryWriter.Write(this.GameCodeId);
                    binaryWriter.Write(serverType);
                    binaryWriter.Write(this.RoomId);
                    binaryWriter.Write(this.RoomData.Length);
                    binaryWriter.Write(this.RoomData);
                    binaryWriter.Write(this.ExtendedRoomId);
                    binaryWriter.Write(this.ConnectUserId);
                    binaryWriter.Write(this.PlayerIOToken);
                    binaryWriter.Write(this.Visible);
                    binaryWriter.Write(this.RoomFlags);
                    binaryWriter.Write(this.PartnerId ?? "");
                    binaryWriter.Write(this.UserId);
                    binaryWriter.Write(this.GameCodeVersion);
                }

                result = Convert.ToBase64String(Encrypt(memoryStream.ToArray(), encryptionKey));
            }

            return result;
        }

        public static string Create(byte[] encryptionKey, string serverId, uint gameId, string gameConnectId, string gameCodeId, string serverType, string roomId, byte[] roomData, string extendedRoomId, string connectUserId, string playerIoToken, bool visible, uint roomFlags, string partnerId, uint userId, int gameCodeVersion)
        {
            string result;
            using (var output = new MemoryStream())
            {
                using (var writer = new BinaryWriter(output))
                {
                    writer.Write(4);
                    writer.Write(DateTime.UtcNow.AddDays(50).Ticks);
                    writer.Write(serverId);
                    writer.Write(gameConnectId);
                    writer.Write(gameCodeId);
                    writer.Write(serverType);
                    writer.Write(roomId);
                    writer.Write(roomData.Length);
                    writer.Write(roomData);
                    writer.Write(extendedRoomId);
                    writer.Write(connectUserId);
                    writer.Write(playerIoToken);
                    writer.Write(visible);
                    writer.Write(roomFlags);
                    writer.Write(partnerId ?? "");
                    writer.Write(userId);
                    writer.Write(gameCodeVersion);
                }

                result = Convert.ToBase64String(Encrypt(output.ToArray(), encryptionKey));
            }

            return result;
        }

        private static readonly Random random = new Random();
        public static byte[] Encrypt(byte[] bytes, byte[] encryptionKey)
        {
            byte[] result;
            using (var mem = new MemoryStream())
            {
                var iv = new byte[16];
                lock (random)
                {
                    random.NextBytes(iv);
                }

                mem.Write(iv, 0, iv.Length);
                var encryptor = new RijndaelManaged { Mode = CipherMode.CBC }.CreateEncryptor(encryptionKey, iv);
                var cryptoStream = new CryptoStream(mem, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
                result = mem.ToArray();
            }

            return result;
        }

        public static byte[] Decrypt(byte[] bytes, byte[] encryptionKey)
        {
            byte[] result2;
            using (var mem = new MemoryStream(bytes))
            {
                var iv = new byte[16];
                mem.Read(iv, 0, iv.Length);
                var decryptor = new RijndaelManaged { Mode = CipherMode.CBC }.CreateDecryptor(encryptionKey, iv);
                var cryptoStream = new CryptoStream(mem, decryptor, CryptoStreamMode.Read);

                var resultPos = 0;
                int decryptedByteCount;
                var result = new byte[10240];
                while ((decryptedByteCount = cryptoStream.Read(result, resultPos, result.Length - resultPos)) != 0)
                {
                    resultPos += decryptedByteCount;
                    if (result.Length - resultPos < 1024)
                    {
                        var newResult = new byte[result.Length * 2];
                        Array.Copy(result, newResult, result.Length);
                        result = newResult;
                    }
                }

                var finalResult = new byte[resultPos];
                Array.Copy(result, finalResult, finalResult.Length);
                result2 = finalResult;
            }

            return result2;
        }
    }
}
