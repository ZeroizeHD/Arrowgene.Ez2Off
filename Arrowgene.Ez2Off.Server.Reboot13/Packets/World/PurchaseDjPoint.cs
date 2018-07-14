/*
 * This file is part of Arrowgene.Ez2Off
 *
 * Arrowgene.Ez2Off is a server implementation for the game "Ez2On".
 * Copyright (C) 2017-2018 Sebastian Heinz
 * Copyright (C) 2017-2018 Halgulaea
 * Copyright (C) 2017-2018 David Via
 *
 * Github: https://github.com/Arrowgene/Arrowgene.Ez2Off
 *
 * Arrowgene.Ez2Off is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Arrowgene.Ez2Off is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Arrowgene.Ez2Off. If not, see <https://www.gnu.org/licenses/>.
 */

using Arrowgene.Ez2Off.Common.Models;
using Arrowgene.Ez2Off.Server.Client;
using Arrowgene.Ez2Off.Server.Models;
using Arrowgene.Ez2Off.Server.Packet;
using Arrowgene.Services.Buffers;

namespace Arrowgene.Ez2Off.Server.Reboot13.Packets.World
{
    public class PurchaseDjPoint : Handler<WorldServer>
    {
        public PurchaseDjPoint(WorldServer server) : base(server)
        {
        }
        public override int Id => 0x20;

        public override void Handle(EzClient client, EzPacket packet)
        {
            short songId = packet.Data.ReadInt16(Endianness.Big);

            _logger.Debug("Purchased Song Id: {0}", songId);

            IBuffer DJpoint = EzServer.Buffer.Provide();
            DJpoint.WriteInt16(1); //DJPoint
            DJpoint.WriteInt16(songId); //Disc Num
            Send(client, 0x2C, DJpoint);
        }
    }

}