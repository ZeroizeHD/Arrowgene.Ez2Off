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
using Arrowgene.Ez2Off.Server.Packet;
using Arrowgene.Ez2Off.Server.Reboot13.Packets.Builder;
using Arrowgene.Services.Buffers;

namespace Arrowgene.Ez2Off.Server.Reboot13.Packets.World
{
    public class InventoryMoveItem : Handler<WorldServer>
    {
        public InventoryMoveItem(WorldServer server) : base(server)
        {
        }

        public override int Id => 22;

        public override void Handle(EzClient client, EzPacket packet)
        {
            byte unknown = packet.Data.ReadByte();
            byte sourceSlot = packet.Data.ReadByte();
            int itemId = packet.Data.ReadInt16(Endianness.Big);
            byte destinationSlot = packet.Data.ReadByte();

            InventoryItem source = client.Inventory.GetItem(sourceSlot);
            InventoryItem destination = client.Inventory.GetItem(destinationSlot);

            if (source == null)
            {
                _logger.Error("Source item could not be found");
                return;
            }

            if (!client.Inventory.Move(sourceSlot, destinationSlot))
            {
                _logger.Error("Couldn't move item {0}, destination is occupied", source.Item.Name);
                return;
            }

            if (!Database.UpdateInventoryItem(source))
            {
                _logger.Error("Couldn't save source item to database {0}", source.Item.Name);
                return;
            }

            if (destination != null && !Database.UpdateInventoryItem(destination))
            {
                _logger.Error("Couldn't save destination item to database {0}", source.Item.Name);
                return;
            }

            _logger.Debug("Move ItemId {0} from slot {1} to {2}", itemId, sourceSlot, destinationSlot);
            IBuffer showInventoryPacket = InventoryPacket.ShowInventoryPacket(client.Inventory);
            Send(client, 0x1E, showInventoryPacket);
        }
    }
}